using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Outcome;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers;

[ApiController]
[Route("api/v1/call-center/outcomes")]
[Authorize]
public class OutcomeController : ControllerBase
{
    private readonly IOutcomeService _outcomeService;

    public OutcomeController(IOutcomeService outcomeService)
    {
        _outcomeService = outcomeService;
    }

    /// <summary>
    /// Get all outcomes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OutcomeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OutcomeDto>>> GetOutcomes()
    {
        var outcomes = await _outcomeService.GetOutcomesAsync();
        return Ok(outcomes);
    }
}
