using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/questions")]
[Authorize(Roles = "Superuser")]
public class QuestionAdminController : ControllerBase
{
    private readonly IQuestionAdminService _questionAdminService;

    public QuestionAdminController(IQuestionAdminService questionAdminService)
    {
        _questionAdminService = questionAdminService;
    }

    /// <summary>
    /// Get all questions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<QuestionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<QuestionDto>>> GetAllQuestions([FromQuery] string? search = null)
    {
        var questions = await _questionAdminService.GetAllQuestionsAsync(search);
        return Ok(questions);
    }

    /// <summary>
    /// Get question by ID
    /// </summary>
    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDto>> GetQuestionById(Guid questionId)
    {
        var question = await _questionAdminService.GetQuestionByIdAsync(questionId);
        return Ok(question);
    }

    /// <summary>
    /// Create a new question
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        var question = await _questionAdminService.CreateQuestionAsync(request);
        return CreatedAtAction(nameof(GetQuestionById), new { questionId = question.Uuid }, question);
    }

    /// <summary>
    /// Update question
    /// </summary>
    [HttpPut("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(Guid questionId, [FromBody] UpdateQuestionRequest request)
    {
        var question = await _questionAdminService.UpdateQuestionAsync(questionId, request);
        return Ok(question);
    }

    /// <summary>
    /// Partially update question
    /// </summary>
    [HttpPatch("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDto>> PatchQuestion(Guid questionId, [FromBody] UpdateQuestionRequest request)
    {
        var question = await _questionAdminService.UpdateQuestionAsync(questionId, request);
        return Ok(question);
    }

    /// <summary>
    /// Delete question
    /// </summary>
    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        await _questionAdminService.DeleteQuestionAsync(questionId);
        return NoContent();
    }
}
