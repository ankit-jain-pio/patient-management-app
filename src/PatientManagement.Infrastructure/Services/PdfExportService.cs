using Microsoft.Extensions.Configuration;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Consultations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PatientManagement.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    private readonly string _clinicName;
    private readonly string _clinicAddress;
    private readonly string _clinicPhone;
    private readonly string _clinicEmail;

    public PdfExportService(IConfiguration configuration)
    {
        // Load clinic settings from configuration
        _clinicName = configuration["ClinicSettings:Name"] ?? "Health Clinic";
        _clinicAddress = configuration["ClinicSettings:Address"] ?? "";
        _clinicPhone = configuration["ClinicSettings:Phone"] ?? "";
        _clinicEmail = configuration["ClinicSettings:Email"] ?? "";

        // Configure QuestPDF license (Community license for free use)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GeneratePrescriptionPdf(ConsultationDto consultation)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);

                page.Content().Element(content => ComposePrescriptionContent(content, consultation));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateConsultationSummaryPdf(ConsultationDto consultation)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);

                page.Content().Element(content => ComposeConsultationContent(content, consultation));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().BorderBottom(2).BorderColor(Colors.Blue.Medium).PaddingBottom(10).Column(headerColumn =>
            {
                headerColumn.Item().Text(_clinicName)
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                headerColumn.Item().Text(_clinicAddress)
                    .FontSize(10);

                headerColumn.Item().Row(row =>
                {
                    row.AutoItem().Text($"Phone: {_clinicPhone}")
                        .FontSize(10);
                    row.Spacing(20);
                    row.AutoItem().Text($"Email: {_clinicEmail}")
                        .FontSize(10);
                });
            });

            column.Item().PaddingVertical(5);
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(10).Column(column =>
        {
            column.Item().AlignCenter().Text(text =>
            {
                text.Span("Generated on: ").FontSize(9).FontColor(Colors.Grey.Darken1);
                text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontSize(9).FontColor(Colors.Grey.Darken1);
            });

            column.Item().AlignCenter().Text("This is a computer-generated document and does not require a signature")
                .FontSize(8)
                .Italic()
                .FontColor(Colors.Grey.Medium);
        });
    }

    private void ComposePrescriptionContent(IContainer container, ConsultationDto consultation)
    {
        container.Column(column =>
        {
            // Document title
            column.Item().PaddingBottom(10).Text("PRESCRIPTION")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Darken1);

            // Consultation date
            column.Item().PaddingBottom(15).Text($"Date: {consultation.ConsultationDate:dd MMM yyyy}")
                .FontSize(11);

            // Patient information section
            column.Item().PaddingBottom(15).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(patientColumn =>
            {
                patientColumn.Item().Text("Patient Information").FontSize(12).Bold();
                patientColumn.Item().PaddingTop(5).Text($"Name: {consultation.Patient?.FullName ?? "N/A"}");
                patientColumn.Item().Text($"Age: {consultation.Patient?.Age ?? 0} years");
                patientColumn.Item().Text($"Phone: {consultation.Patient?.PhoneNumber ?? "N/A"}");
            });

            // Diagnosis section
            if (!string.IsNullOrWhiteSpace(consultation.Diagnosis))
            {
                column.Item().PaddingTop(15).Column(diagnosisColumn =>
                {
                    diagnosisColumn.Item().Text("Diagnosis").FontSize(12).Bold();
                    diagnosisColumn.Item().PaddingTop(5).Text(consultation.Diagnosis);
                });
            }

            // Prescriptions table
            if (consultation.Prescriptions != null && consultation.Prescriptions.Any())
            {
                column.Item().PaddingTop(20).Column(prescriptionColumn =>
                {
                    prescriptionColumn.Item().Text("Medications").FontSize(12).Bold();
                    prescriptionColumn.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // S.No
                            columns.RelativeColumn(3);   // Medication
                            columns.RelativeColumn(2);   // Dosage
                            columns.RelativeColumn(2);   // Frequency
                            columns.RelativeColumn(1);   // Duration
                        });

                        // Table header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("No.").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Medication").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Dosage").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Frequency").Bold();
                            header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Duration").Bold();
                        });

                        // Table rows
                        int index = 1;
                        foreach (var prescription in consultation.Prescriptions)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(index.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(prescription.MedicationName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(prescription.Dosage);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(prescription.Frequency);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{prescription.DurationInDays} days");
                            index++;
                        }
                    });

                    // Special instructions for each prescription
                    prescriptionColumn.Item().PaddingTop(15).Column(instructionColumn =>
                    {
                        instructionColumn.Item().Text("Instructions").FontSize(11).Bold();
                        foreach (var prescription in consultation.Prescriptions.Where(p => !string.IsNullOrWhiteSpace(p.Instructions)))
                        {
                            instructionColumn.Item().PaddingTop(5).Text(text =>
                            {
                                text.Span($"• {prescription.MedicationName}: ").Bold().FontSize(10);
                                text.Span(prescription.Instructions ?? "").FontSize(10);
                            });
                        }
                    });
                });
            }

            // Follow-up instructions
            if (!string.IsNullOrWhiteSpace(consultation.FollowUpInstructions))
            {
                column.Item().PaddingTop(20).Column(followUpColumn =>
                {
                    followUpColumn.Item().Text("Follow-up Instructions").FontSize(12).Bold();
                    followUpColumn.Item().PaddingTop(5).Text(consultation.FollowUpInstructions);
                });
            }

            // Next visit date
            if (consultation.NextVisitDate.HasValue)
            {
                column.Item().PaddingTop(10).Text($"Next Visit Date: {consultation.NextVisitDate.Value:dd MMM yyyy}")
                    .FontColor(Colors.Red.Medium)
                    .Bold();
            }
        });
    }

    private void ComposeConsultationContent(IContainer container, ConsultationDto consultation)
    {
        container.Column(column =>
        {
            // Document title
            column.Item().PaddingBottom(10).Text("CONSULTATION SUMMARY")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Darken1);

            // Consultation date
            column.Item().PaddingBottom(15).Text($"Date: {consultation.ConsultationDate:dd MMM yyyy, HH:mm}")
                .FontSize(11);

            // Patient information
            column.Item().PaddingBottom(15).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(patientColumn =>
            {
                patientColumn.Item().Text("Patient Information").FontSize(12).Bold();
                patientColumn.Item().PaddingTop(5).Text($"Name: {consultation.Patient?.FullName ?? "N/A"}");
                patientColumn.Item().Text($"Age: {consultation.Patient?.Age ?? 0} years");
                patientColumn.Item().Text($"Phone: {consultation.Patient?.PhoneNumber ?? "N/A"}");
            });

            // Vitals
            if (consultation.Vitals != null)
            {
                column.Item().PaddingTop(15).Column(vitalsColumn =>
                {
                    vitalsColumn.Item().Text("Vital Signs").FontSize(12).Bold();
                    vitalsColumn.Item().PaddingTop(5).Row(row =>
                    {
                        row.AutoItem().Width(200).Text($"Temperature: {consultation.Vitals.Temperature}°C");
                        row.AutoItem().Text($"Blood Pressure: {consultation.Vitals.BloodPressure}");
                    });
                    vitalsColumn.Item().Row(row =>
                    {
                        row.AutoItem().Width(200).Text($"Pulse Rate: {consultation.Vitals.PulseRate} bpm");
                        row.AutoItem().Text($"O2 Saturation: {consultation.Vitals.OxygenSaturation}%");
                    });
                    vitalsColumn.Item().Row(row =>
                    {
                        row.AutoItem().Width(200).Text($"Weight: {consultation.Vitals.Weight} kg");
                        row.AutoItem().Text($"Height: {consultation.Vitals.Height} cm");
                    });
                    if (consultation.Vitals.BMI.HasValue)
                    {
                        vitalsColumn.Item().Text($"BMI: {consultation.Vitals.BMI:F2}");
                    }
                });
            }

            // Chief complaint
            if (!string.IsNullOrWhiteSpace(consultation.ChiefComplaint))
            {
                column.Item().PaddingTop(15).Column(complaintColumn =>
                {
                    complaintColumn.Item().Text("Chief Complaint").FontSize(12).Bold();
                    complaintColumn.Item().PaddingTop(5).Text(consultation.ChiefComplaint);
                });
            }

            // Symptoms
            if (!string.IsNullOrWhiteSpace(consultation.Symptoms))
            {
                column.Item().PaddingTop(15).Column(symptomsColumn =>
                {
                    symptomsColumn.Item().Text("Symptoms").FontSize(12).Bold();
                    symptomsColumn.Item().PaddingTop(5).Text(consultation.Symptoms);
                });
            }

            // Diagnosis
            if (!string.IsNullOrWhiteSpace(consultation.Diagnosis))
            {
                column.Item().PaddingTop(15).Column(diagnosisColumn =>
                {
                    diagnosisColumn.Item().Text("Diagnosis").FontSize(12).Bold();
                    diagnosisColumn.Item().PaddingTop(5).Text(consultation.Diagnosis);
                });
            }

            // Treatment plan
            if (!string.IsNullOrWhiteSpace(consultation.TreatmentPlan))
            {
                column.Item().PaddingTop(15).Column(treatmentColumn =>
                {
                    treatmentColumn.Item().Text("Treatment Plan").FontSize(12).Bold();
                    treatmentColumn.Item().PaddingTop(5).Text(consultation.TreatmentPlan);
                });
            }

            // Clinical notes
            if (!string.IsNullOrWhiteSpace(consultation.ClinicalNotes))
            {
                column.Item().PaddingTop(15).Column(notesColumn =>
                {
                    notesColumn.Item().Text("Clinical Notes").FontSize(12).Bold();
                    notesColumn.Item().PaddingTop(5).Text(consultation.ClinicalNotes);
                });
            }

            // Prescriptions summary
            if (consultation.Prescriptions != null && consultation.Prescriptions.Any())
            {
                column.Item().PaddingTop(15).Column(prescriptionColumn =>
                {
                    prescriptionColumn.Item().Text("Prescriptions").FontSize(12).Bold();
                    foreach (var prescription in consultation.Prescriptions)
                    {
                        prescriptionColumn.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span($"• {prescription.MedicationName}").Bold();
                            text.Span($" - {prescription.Dosage}, {prescription.Frequency} for {prescription.DurationInDays} days");
                        });
                    }
                });
            }

            // Follow-up
            if (!string.IsNullOrWhiteSpace(consultation.FollowUpInstructions) || consultation.NextVisitDate.HasValue)
            {
                column.Item().PaddingTop(15).Column(followUpColumn =>
                {
                    followUpColumn.Item().Text("Follow-up").FontSize(12).Bold();
                    if (!string.IsNullOrWhiteSpace(consultation.FollowUpInstructions))
                    {
                        followUpColumn.Item().PaddingTop(5).Text(consultation.FollowUpInstructions);
                    }
                    if (consultation.NextVisitDate.HasValue)
                    {
                        followUpColumn.Item().PaddingTop(5).Text($"Next Visit: {consultation.NextVisitDate.Value:dd MMM yyyy}")
                            .FontColor(Colors.Red.Medium)
                            .Bold();
                    }
                });
            }
        });
    }
}
