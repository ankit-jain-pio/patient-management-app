using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.Patients.Queries.GetAllPatients;
using PatientManagement.Application.Patients.Queries.GetPatientHistory;
using PatientManagement.Application.Appointments.Queries.GetAppointmentsByDate;
using PatientManagement.Application.Consultations.Queries.GetPatientConsultations;

namespace PatientManagement.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ExportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICsvExportService _csvExportService;
    private readonly IPdfExportService _pdfExportService;

    public ExportsController(
        IMediator mediator,
        ICsvExportService csvExportService,
        IPdfExportService pdfExportService)
    {
        _mediator = mediator;
        _csvExportService = csvExportService;
        _pdfExportService = pdfExportService;
    }

    /// <summary>
    /// Export all patients to CSV
    /// </summary>
    [HttpGet("patients/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportPatientsToCsv()
    {
        var query = new GetAllPatientsQuery();
        var patients = await _mediator.Send(query);

        var csvBytes = _csvExportService.ExportPatients(patients);

        return File(csvBytes, "text/csv", $"patients_{DateTime.Now:yyyyMMddHHmmss}.csv");
    }

    /// <summary>
    /// Export appointments for a specific date range to CSV
    /// </summary>
    [HttpGet("appointments/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportAppointmentsToCsv([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var appointments = new List<Application.DTOs.Appointments.AppointmentDto>();

        if (startDate.HasValue && endDate.HasValue)
        {
            // Export appointments within date range
            var currentDate = startDate.Value.Date;
            var end = endDate.Value.Date;

            while (currentDate <= end)
            {
                var query = new GetAppointmentsByDateQuery { Date = currentDate };
                var dailyAppointments = await _mediator.Send(query);
                appointments.AddRange(dailyAppointments);
                currentDate = currentDate.AddDays(1);
            }
        }
        else
        {
            // Export today's appointments
            var query = new GetAppointmentsByDateQuery { Date = DateTime.Today };
            appointments = (await _mediator.Send(query)).ToList();
        }

        var csvBytes = _csvExportService.ExportAppointments(appointments);

        return File(csvBytes, "text/csv", $"appointments_{DateTime.Now:yyyyMMddHHmmss}.csv");
    }

    /// <summary>
    /// Export patient history (consultations) to CSV
    /// </summary>
    [HttpGet("patients/{patientId:guid}/history/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportPatientHistoryToCsv(
        Guid patientId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = new GetPatientHistoryQuery
        {
            PatientId = patientId,
            StartDate = startDate,
            EndDate = endDate
        };

        var history = await _mediator.Send(query);

        var csvBytes = _csvExportService.ExportConsultations(history.Consultations);

        return File(csvBytes, "text/csv", $"patient_{patientId}_history_{DateTime.Now:yyyyMMddHHmmss}.csv");
    }

    /// <summary>
    /// Generate prescription PDF for a consultation
    /// </summary>
    [HttpGet("consultations/{consultationId:guid}/prescription/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePrescriptionPdf(Guid consultationId)
    {
        var query = new Application.Consultations.Queries.GetConsultation.GetConsultationQuery
        {
            Id = consultationId
        };

        var consultation = await _mediator.Send(query);

        if (consultation == null)
        {
            return NotFound($"Consultation with ID {consultationId} not found");
        }

        var pdfBytes = _pdfExportService.GeneratePrescriptionPdf(consultation);

        return File(pdfBytes, "application/pdf", $"prescription_{consultationId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    /// <summary>
    /// Generate consultation summary PDF
    /// </summary>
    [HttpGet("consultations/{consultationId:guid}/summary/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateConsultationSummaryPdf(Guid consultationId)
    {
        var query = new Application.Consultations.Queries.GetConsultation.GetConsultationQuery
        {
            Id = consultationId
        };

        var consultation = await _mediator.Send(query);

        if (consultation == null)
        {
            return NotFound($"Consultation with ID {consultationId} not found");
        }

        var pdfBytes = _pdfExportService.GenerateConsultationSummaryPdf(consultation);

        return File(pdfBytes, "application/pdf", $"consultation_summary_{consultationId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    /// <summary>
    /// Get patient history with date range filtering
    /// </summary>
    [HttpGet("patients/{patientId:guid}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientHistoryDto>> GetPatientHistory(
        Guid patientId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = new GetPatientHistoryQuery
        {
            PatientId = patientId,
            StartDate = startDate,
            EndDate = endDate
        };

        var history = await _mediator.Send(query);

        return Ok(history);
    }
}
