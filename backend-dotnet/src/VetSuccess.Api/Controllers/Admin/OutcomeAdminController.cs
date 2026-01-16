using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/outcomes")]
[Authorize(Roles = "Superuser")]
public class OutcomeAdminController : ControllerBase
{
    private readonly IOutcomeAdminService _outcomeAdminService;

    public OutcomeAdminController(IOutcomeAdminService outcomeAdminService)
    {
        _outcomeAdminService = outcomeAdminService;
    }

    /// <summary>
    /// Get all outcomes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OutcomeAdminDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OutcomeAdminDto>>> GetAllOutcomes([FromQuery] string? search = null)
    {
        var outcomes = await _outcomeAdminService.GetAllOutcomesAsync(search);
        return Ok(outcomes);
    }

    /// <summary>
    /// Get outcome by ID
    /// </summary>
    [HttpGet("{outcomeId:guid}")]
    [ProducesResponseType(typeof(OutcomeAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OutcomeAdminDto>> GetOutcomeById(Guid outcomeId)
    {
        var outcome = await _outcomeAdminService.GetOutcomeByIdAsync(outcomeId);
        return Ok(outcome);
    }

    /// <summary>
    /// Create a new outcome
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OutcomeAdminDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OutcomeAdminDto>> CreateOutcome([FromBody] CreateOutcomeRequest request)
    {
        var outcome = await _outcomeAdminService.CreateOutcomeAsync(request);
        return CreatedAtAction(nameof(GetOutcomeById), new { outcomeId = outcome.Uuid }, outcome);
    }

    /// <summary>
    /// Update outcome
    /// </summary>
    [HttpPut("{outcomeId:guid}")]
    [ProducesResponseType(typeof(OutcomeAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OutcomeAdminDto>> UpdateOutcome(Guid outcomeId, [FromBody] UpdateOutcomeRequest request)
    {
        var outcome = await _outcomeAdminService.UpdateOutcomeAsync(outcomeId, request);
        return Ok(outcome);
    }

    /// <summary>
    /// Partially update outcome
    /// </summary>
    [HttpPatch("{outcomeId:guid}")]
    [ProducesResponseType(typeof(OutcomeAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OutcomeAdminDto>> PatchOutcome(Guid outcomeId, [FromBody] UpdateOutcomeRequest request)
    {
        var outcome = await _outcomeAdminService.UpdateOutcomeAsync(outcomeId, request);
        return Ok(outcome);
    }

    /// <summary>
    /// Delete outcome
    /// </summary>
    [HttpDelete("{outcomeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOutcome(Guid outcomeId)
    {
        await _outcomeAdminService.DeleteOutcomeAsync(outcomeId);
        return NoContent();
    }
}
