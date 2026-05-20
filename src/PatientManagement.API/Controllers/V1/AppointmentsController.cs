using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Appointments.Commands.CreateAppointment;
using PatientManagement.Application.Appointments.Commands.UpdateAppointment;
using PatientManagement.Application.Appointments.Commands.UpdateAppointmentStatus;
using PatientManagement.Application.Appointments.Queries.GetAppointment;
using PatientManagement.Application.Appointments.Queries.GetAppointmentsByDate;
using PatientManagement.Application.Appointments.Queries.GetPatientAppointments;
using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Common;

namespace PatientManagement.API.Controllers.V1;

[Authorize]
public class AppointmentsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get appointments by date
    /// </summary>
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(ApiResponse<List<AppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetByDate([FromQuery] DateTime date)
    {
        var query = new GetAppointmentsByDateQuery { Date = date };
        var appointments = await _mediator.Send(query);
        return Ok(ApiResponse<List<AppointmentDto>>.SuccessResult(appointments));
    }

    /// <summary>
    /// Get appointments for a specific patient
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetByPatient(Guid patientId)
    {
        var query = new GetPatientAppointmentsQuery { PatientId = patientId };
        var appointments = await _mediator.Send(query);
        return Ok(ApiResponse<List<AppointmentDto>>.SuccessResult(appointments));
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(Guid id)
    {
        var query = new GetAppointmentQuery { Id = id };
        var appointment = await _mediator.Send(query);

        if (appointment == null)
        {
            return NotFound(ApiResponse<AppointmentDto>.FailureResult("Appointment not found"));
        }

        return Ok(ApiResponse<AppointmentDto>.SuccessResult(appointment));
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto)
    {
        var command = new CreateAppointmentCommand
        {
            PatientId = dto.PatientId,
            ScheduledDateTime = dto.ScheduledDateTime,
            Reason = dto.Reason,
            Notes = dto.Notes
        };

        var appointment = await _mediator.Send(command);
        _logger.LogInformation("Appointment created: {AppointmentId} for Patient {PatientId}", appointment.Id, appointment.PatientId);

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, ApiResponse<AppointmentDto>.SuccessResult(appointment, "Appointment created successfully"));
    }

    /// <summary>
    /// Update an existing appointment
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(Guid id, [FromBody] UpdateAppointmentDto dto)
    {
        var command = new UpdateAppointmentCommand
        {
            Id = id,
            ScheduledDateTime = dto.ScheduledDateTime,
            Reason = dto.Reason,
            Notes = dto.Notes
        };

        var appointment = await _mediator.Send(command);
        _logger.LogInformation("Appointment updated: {AppointmentId}", appointment.Id);

        return Ok(ApiResponse<AppointmentDto>.SuccessResult(appointment, "Appointment updated successfully"));
    }

    /// <summary>
    /// Update appointment status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        var command = new UpdateAppointmentStatusCommand
        {
            Id = id,
            Status = dto.Status
        };

        var appointment = await _mediator.Send(command);
        _logger.LogInformation("Appointment status updated: {AppointmentId} - {Status}", appointment.Id, appointment.Status);

        return Ok(ApiResponse<AppointmentDto>.SuccessResult(appointment, "Appointment status updated successfully"));
    }
}
