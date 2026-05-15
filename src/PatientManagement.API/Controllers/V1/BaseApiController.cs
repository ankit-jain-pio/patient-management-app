using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace PatientManagement.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}
