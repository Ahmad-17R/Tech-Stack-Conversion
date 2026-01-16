using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/practice-settings")]
[Authorize(Roles = "Superuser")]
public class PracticeSettingsAdminController : ControllerBase
{
    private readonly IPracticeSettingsAdminService _practiceSettingsAdminService;

    public PracticeSettingsAdminController(IPracticeSettingsAdminService practiceSettingsAdminService)
    {
        _practiceSettingsAdminService = practiceSettingsAdminService;
    }

    /// <summary>
    /// Get all practice settings with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PracticeSettingsAdminDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PracticeSettingsAdminDto>>> GetAllPracticeSettings(
        [FromQuery] string? practice = null)
    {
        var settings = await _practiceSettingsAdminService.GetAllPracticeSettingsAsync(practice);
        return Ok(settings);
    }

    /// <summary>
    /// Get practice settings by ID
    /// </summary>
    [HttpGet("{settingsId:guid}")]
    [ProducesResponseType(typeof(PracticeSettingsAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeSettingsAdminDto>> GetPracticeSettingsById(Guid settingsId)
    {
        var settings = await _practiceSettingsAdminService.GetPracticeSettingsByIdAsync(settingsId);
        return Ok(settings);
    }

    /// <summary>
    /// Create new practice settings
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PracticeSettingsAdminDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PracticeSettingsAdminDto>> CreatePracticeSettings(
        [FromBody] CreatePracticeSettingsRequest request)
    {
        var settings = await _practiceSettingsAdminService.CreatePracticeSettingsAsync(request);
        return CreatedAtAction(nameof(GetPracticeSettingsById), new { settingsId = settings.Uuid }, settings);
    }

    /// <summary>
    /// Update practice settings
    /// </summary>
    [HttpPut("{settingsId:guid}")]
    [ProducesResponseType(typeof(PracticeSettingsAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeSettingsAdminDto>> UpdatePracticeSettings(
        Guid settingsId, 
        [FromBody] UpdatePracticeSettingsRequest request)
    {
        var settings = await _practiceSettingsAdminService.UpdatePracticeSettingsAsync(settingsId, request);
        return Ok(settings);
    }

    /// <summary>
    /// Partially update practice settings
    /// </summary>
    [HttpPatch("{settingsId:guid}")]
    [ProducesResponseType(typeof(PracticeSettingsAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PracticeSettingsAdminDto>> PatchPracticeSettings(
        Guid settingsId, 
        [FromBody] UpdatePracticeSettingsRequest request)
    {
        var settings = await _practiceSettingsAdminService.UpdatePracticeSettingsAsync(settingsId, request);
        return Ok(settings);
    }

    /// <summary>
    /// Delete practice settings
    /// </summary>
    [HttpDelete("{settingsId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePracticeSettings(Guid settingsId)
    {
        await _practiceSettingsAdminService.DeletePracticeSettingsAsync(settingsId);
        return NoContent();
    }
}
