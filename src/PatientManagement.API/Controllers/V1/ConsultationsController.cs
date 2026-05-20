using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Consultations.Commands.AddPrescription;
using PatientManagement.Application.Consultations.Commands.CreateConsultation;
using PatientManagement.Application.Consultations.Commands.UpdateConsultation;
using PatientManagement.Application.Consultations.Queries.GetConsultation;
using PatientManagement.Application.Consultations.Queries.GetPatientConsultations;
using PatientManagement.Application.DTOs.Common;
using PatientManagement.Application.DTOs.Consultations;

namespace PatientManagement.API.Controllers.V1;

[Authorize]
public class ConsultationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConsultationsController> _logger;

    public ConsultationsController(IMediator mediator, ILogger<ConsultationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConsultationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ConsultationDto>>> GetById(Guid id)
    {
        var query = new GetConsultationQuery { Id = id };
        var consultation = await _mediator.Send(query);

        if (consultation == null)
        {
            return NotFound(ApiResponse<ConsultationDto>.FailureResult("Consultation not found"));
        }

        return Ok(ApiResponse<ConsultationDto>.SuccessResult(consultation));
    }

    /// <summary>
    /// Get all consultations for a specific patient
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ConsultationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ConsultationDto>>>> GetByPatient(Guid patientId)
    {
        var query = new GetPatientConsultationsQuery { PatientId = patientId };
        var consultations = await _mediator.Send(query);
        return Ok(ApiResponse<List<ConsultationDto>>.SuccessResult(consultations));
    }

    /// <summary>
    /// Create a new consultation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConsultationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ConsultationDto>>> Create([FromBody] CreateConsultationDto dto)
    {
        var command = new CreateConsultationCommand
        {
            PatientId = dto.PatientId,
            AppointmentId = dto.AppointmentId,
            Vitals = dto.Vitals,
            ChiefComplaint = dto.ChiefComplaint,
            Symptoms = dto.Symptoms
        };

        var consultation = await _mediator.Send(command);
        _logger.LogInformation("Consultation created: {ConsultationId} for Patient {PatientId}", consultation.Id, consultation.PatientId);

        return CreatedAtAction(nameof(GetById), new { id = consultation.Id }, 
            ApiResponse<ConsultationDto>.SuccessResult(consultation, "Consultation created successfully"));
    }

    /// <summary>
    /// Update an existing consultation
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConsultationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ConsultationDto>>> Update(Guid id, [FromBody] UpdateConsultationDto dto)
    {
        var command = new UpdateConsultationCommand
        {
            Id = id,
            Vitals = dto.Vitals,
            ChiefComplaint = dto.ChiefComplaint,
            Symptoms = dto.Symptoms,
            Diagnosis = dto.Diagnosis,
            ClinicalNotes = dto.ClinicalNotes,
            TreatmentPlan = dto.TreatmentPlan,
            FollowUpInstructions = dto.FollowUpInstructions,
            NextVisitDate = dto.NextVisitDate
        };

        var consultation = await _mediator.Send(command);
        _logger.LogInformation("Consultation updated: {ConsultationId}", consultation.Id);

        return Ok(ApiResponse<ConsultationDto>.SuccessResult(consultation, "Consultation updated successfully"));
    }

    /// <summary>
    /// Add prescription to consultation
    /// </summary>
    [HttpPost("{consultationId:guid}/prescriptions")]
    [ProducesResponseType(typeof(ApiResponse<PrescriptionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PrescriptionDto>>> AddPrescription(Guid consultationId, [FromBody] AddPrescriptionDto dto)
    {
        var command = new AddPrescriptionCommand
        {
            ConsultationId = consultationId,
            MedicationName = dto.MedicationName,
            Dosage = dto.Dosage,
            Frequency = dto.Frequency,
            DurationInDays = dto.DurationInDays,
            Instructions = dto.Instructions
        };

        var prescription = await _mediator.Send(command);
        _logger.LogInformation("Prescription added: {PrescriptionId} to Consultation {ConsultationId}", 
            prescription.Id, consultationId);

        return CreatedAtAction(nameof(GetById), new { id = consultationId }, 
            ApiResponse<PrescriptionDto>.SuccessResult(prescription, "Prescription added successfully"));
    }
}
