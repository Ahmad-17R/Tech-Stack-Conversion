using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Practice;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers;

[ApiController]
[Route("api/v1/call-center/practices")]
[Authorize]
public class PracticeController : ControllerBase
{
    private readonly IPracticeService _practiceService;

    public PracticeController(IPracticeService practiceService)
    {
        _practiceService = practiceService;
    }

    /// <summary>
    /// Get all active practices
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PracticeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PracticeDto>>> GetPractices()
    {
        var practices = await _practiceService.GetPracticesAsync();
        return Ok(practices);
    }

    /// <summary>
    /// Get FAQs for a specific practice
    /// </summary>
    [HttpGet("{oduId}/faq")]
    [ProducesResponseType(typeof(List<FAQDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FAQDto>>> GetPracticeFAQs(string oduId)
    {
        var faqs = await _practiceService.GetFAQsAsync(oduId);
        return Ok(faqs);
    }
}
