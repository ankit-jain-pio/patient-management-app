using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.DTOs.Common;
using PatientManagement.Application.DTOs.Patients;
using PatientManagement.Application.Patients.Commands.CreatePatient;
using PatientManagement.Application.Patients.Commands.UpdatePatient;
using PatientManagement.Application.Patients.Queries.GetAllPatients;
using PatientManagement.Application.Patients.Queries.GetPatient;
using PatientManagement.Application.Patients.Queries.SearchPatients;

namespace PatientManagement.API.Controllers.V1;

[Authorize]
public class PatientsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IMediator mediator, ILogger<PatientsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all patients with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PatientDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PatientDto>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var query = new GetAllPatientsQuery { PageNumber = pageNumber, PageSize = pageSize };
        var patients = await _mediator.Send(query);
        return Ok(ApiResponse<List<PatientDto>>.SuccessResult(patients));
    }

    /// <summary>
    /// Search patients by name or phone number
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<PatientSearchResultDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PatientSearchResultDto>>>> Search([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new SearchPatientsQuery { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize };
        var results = await _mediator.Send(query);
        return Ok(ApiResponse<List<PatientSearchResultDto>>.SuccessResult(results));
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id)
    {
        var query = new GetPatientQuery { Id = id };
        var patient = await _mediator.Send(query);

        if (patient == null)
        {
            return NotFound(ApiResponse<PatientDto>.FailureResult("Patient not found"));
        }

        return Ok(ApiResponse<PatientDto>.SuccessResult(patient));
    }

    /// <summary>
    /// Create a new patient
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create([FromBody] CreatePatientDto dto)
    {
        var command = new CreatePatientCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = (int)dto.Gender,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Address = dto.Address,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            BloodGroup = dto.BloodGroup,
            Allergies = dto.Allergies,
            MedicalHistory = dto.MedicalHistory
        };

        var patient = await _mediator.Send(command);
        _logger.LogInformation("Patient created: {PatientId} - {FullName}", patient.Id, patient.FullName);

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, ApiResponse<PatientDto>.SuccessResult(patient, "Patient created successfully"));
    }

    /// <summary>
    /// Update an existing patient
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        var command = new UpdatePatientCommand
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = (int)dto.Gender,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Address = dto.Address,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            BloodGroup = dto.BloodGroup,
            Allergies = dto.Allergies,
            MedicalHistory = dto.MedicalHistory
        };

        var patient = await _mediator.Send(command);
        _logger.LogInformation("Patient updated: {PatientId} - {FullName}", patient.Id, patient.FullName);

        return Ok(ApiResponse<PatientDto>.SuccessResult(patient, "Patient updated successfully"));
    }
}
