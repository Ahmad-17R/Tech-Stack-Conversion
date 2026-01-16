using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/practices")]
[Authorize(Roles = "Superuser")]
public class PracticeAdminController : ControllerBase
{
    private readonly IPracticeAdminService _practiceAdminService;

    public PracticeAdminController(IPracticeAdminService practiceAdminService)
    {
        _practiceAdminService = practiceAdminService;
    }

    /// <summary>
    /// Get all practices with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PracticeAdminListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PracticeAdminListDto>>> GetAllPractices(
        [FromQuery] bool? hasSettings = null,
        [FromQuery] bool? isSmsMailingEnabled = null,
        [FromQuery] bool? isEmailUpdatesEnabled = null,
        [FromQuery] bool? isArchived = null,
        [FromQuery] string? search = null)
    {
        var practices = await _practiceAdminService.GetAllPracticesAsync(
            hasSettings, isSmsMailingEnabled, isEmailUpdatesEnabled, isArchived, search);
        return Ok(practices);
    }

    /// <summary>
    /// Get practice by ODU ID
    /// </summary>
    [HttpGet("{practiceOduId}")]
    [ProducesResponseType(typeof(PracticeAdminDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeAdminDetailDto>> GetPracticeById(string practiceOduId)
    {
        var practice = await _practiceAdminService.GetPracticeByIdAsync(practiceOduId);
        return Ok(practice);
    }

    /// <summary>
    /// Update practice
    /// </summary>
    [HttpPut("{practiceOduId}")]
    [ProducesResponseType(typeof(PracticeAdminDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeAdminDetailDto>> UpdatePractice(
        string practiceOduId, 
        [FromBody] UpdatePracticeAdminRequest request)
    {
        var practice = await _practiceAdminService.UpdatePracticeAsync(practiceOduId, request);
        return Ok(practice);
    }

    /// <summary>
    /// Partially update practice
    /// </summary>
    [HttpPatch("{practiceOduId}")]
    [ProducesResponseType(typeof(PracticeAdminDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeAdminDetailDto>> PatchPractice(
        string practiceOduId, 
        [FromBody] UpdatePracticeAdminRequest request)
    {
        var practice = await _practiceAdminService.UpdatePracticeAsync(practiceOduId, request);
        return Ok(practice);
    }
}
