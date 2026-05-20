using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Application.DTOs.Patients;

namespace PatientManagement.Infrastructure.Services;

public class CsvExportService : ICsvExportService
{
    private readonly CsvConfiguration _csvConfig;

    public CsvExportService()
    {
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = Encoding.UTF8
        };
    }

    public byte[] ExportPatients(List<PatientDto> patients)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, _csvConfig);

        // Map patient data to flat structure for CSV
        var patientRecords = patients.Select(p => new
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
            Age = p.Age,
            Gender = p.Gender,
            PhoneNumber = p.PhoneNumber,
            Email = p.Email,
            Address = p.Address != null ? $"{p.Address.Street}, {p.Address.City}, {p.Address.State} {p.Address.PostalCode}" : "",
            BloodGroup = p.BloodGroup,
            Allergies = p.Allergies,
            MedicalHistory = p.MedicalHistory,
            EmergencyContactName = p.EmergencyContactName,
            EmergencyContactPhone = p.EmergencyContactPhone,
            CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = p.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        csvWriter.WriteRecords(patientRecords);
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportConsultations(List<ConsultationDto> consultations)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, _csvConfig);

        // Map consultation data to flat structure for CSV
        var consultationRecords = consultations.Select(c => new
        {
            ConsultationId = c.Id,
            PatientId = c.PatientId,
            PatientName = c.Patient?.FullName ?? "",
            PatientAge = c.Patient?.Age ?? 0,
            ConsultationDate = c.ConsultationDate.ToString("yyyy-MM-dd HH:mm:ss"),
            ChiefComplaint = c.ChiefComplaint,
            Symptoms = c.Symptoms,
            Diagnosis = c.Diagnosis,
            TreatmentPlan = c.TreatmentPlan,
            // Vitals
            Temperature = c.Vitals?.Temperature,
            BloodPressure = c.Vitals?.BloodPressure,
            PulseRate = c.Vitals?.PulseRate,
            Weight = c.Vitals?.Weight,
            Height = c.Vitals?.Height,
            OxygenSaturation = c.Vitals?.OxygenSaturation,
            RespiratoryRate = c.Vitals?.RespiratoryRate,
            BMI = c.Vitals?.BMI,
            // Follow-up
            FollowUpInstructions = c.FollowUpInstructions,
            NextVisitDate = c.NextVisitDate?.ToString("yyyy-MM-dd"),
            ClinicalNotes = c.ClinicalNotes,
            PrescriptionCount = c.Prescriptions?.Count ?? 0,
            CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = c.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        csvWriter.WriteRecords(consultationRecords);
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportAppointments(List<AppointmentDto> appointments)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, _csvConfig);

        // Map appointment data to flat structure for CSV
        var appointmentRecords = appointments.Select(a => new
        {
            AppointmentId = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient?.FullName ?? "",
            PatientPhone = a.Patient?.PhoneNumber ?? "",
            ScheduledDateTime = a.ScheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = a.Status.ToString(),
            Reason = a.Reason,
            Notes = a.Notes,
            CheckInTime = a.CheckInTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CompletedTime = a.CompletedTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CreatedAt = a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = a.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        csvWriter.WriteRecords(appointmentRecords);
        streamWriter.Flush();

        return memoryStream.ToArray();
    }
}
