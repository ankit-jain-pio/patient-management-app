using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.Application.Common.Interfaces;

public interface IPdfExportService
{
    byte[] GeneratePrescriptionPdf(ConsultationDto consultation);
    byte[] GenerateConsultationSummaryPdf(ConsultationDto consultation);
}
