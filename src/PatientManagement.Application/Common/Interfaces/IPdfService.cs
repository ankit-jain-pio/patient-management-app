namespace PatientManagement.Application.Common.Interfaces;

public interface IPdfService
{
    Task<byte[]> GeneratePrescriptionPdfAsync(Guid consultationId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateVisitSummaryPdfAsync(Guid consultationId, CancellationToken cancellationToken = default);
}
