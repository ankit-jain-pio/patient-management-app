using PatientManagement.Application.Common.Interfaces;

namespace PatientManagement.Infrastructure.Services;

public class PdfService : IPdfService
{
    // Phase 1: Stub implementation - will be fully implemented in Phase 4
    public Task<byte[]> GeneratePrescriptionPdfAsync(Guid consultationId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using QuestPDF in Phase 4
        throw new NotImplementedException("PDF generation will be implemented in Phase 4");
    }
    
    public Task<byte[]> GenerateVisitSummaryPdfAsync(Guid consultationId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using QuestPDF in Phase 4
        throw new NotImplementedException("PDF generation will be implemented in Phase 4");
    }
}
