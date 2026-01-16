using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/answers")]
[Authorize(Roles = "Superuser")]
public class AnswerAdminController : ControllerBase
{
    private readonly IAnswerAdminService _answerAdminService;

    public AnswerAdminController(IAnswerAdminService answerAdminService)
    {
        _answerAdminService = answerAdminService;
    }

    /// <summary>
    /// Get all answers with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AnswerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AnswerDto>>> GetAllAnswers(
        [FromQuery] string? practice = null,
        [FromQuery] string? search = null)
    {
        var answers = await _answerAdminService.GetAllAnswersAsync(practice, search);
        return Ok(answers);
    }

    /// <summary>
    /// Get answer by ID
    /// </summary>
    [HttpGet("{answerId:guid}")]
    [ProducesResponseType(typeof(AnswerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnswerDto>> GetAnswerById(Guid answerId)
    {
        var answer = await _answerAdminService.GetAnswerByIdAsync(answerId);
        return Ok(answer);
    }

    /// <summary>
    /// Create a new answer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnswerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnswerDto>> CreateAnswer([FromBody] CreateAnswerRequest request)
    {
        var answer = await _answerAdminService.CreateAnswerAsync(request);
        return CreatedAtAction(nameof(GetAnswerById), new { answerId = answer.Uuid }, answer);
    }

    /// <summary>
    /// Update answer
    /// </summary>
    [HttpPut("{answerId:guid}")]
    [ProducesResponseType(typeof(AnswerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnswerDto>> UpdateAnswer(Guid answerId, [FromBody] UpdateAnswerRequest request)
    {
        var answer = await _answerAdminService.UpdateAnswerAsync(answerId, request);
        return Ok(answer);
    }

    /// <summary>
    /// Partially update answer
    /// </summary>
    [HttpPatch("{answerId:guid}")]
    [ProducesResponseType(typeof(AnswerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnswerDto>> PatchAnswer(Guid answerId, [FromBody] UpdateAnswerRequest request)
    {
        var answer = await _answerAdminService.UpdateAnswerAsync(answerId, request);
        return Ok(answer);
    }

    /// <summary>
    /// Delete answer
    /// </summary>
    [HttpDelete("{answerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnswer(Guid answerId)
    {
        await _answerAdminService.DeleteAnswerAsync(answerId);
        return NoContent();
    }
}
