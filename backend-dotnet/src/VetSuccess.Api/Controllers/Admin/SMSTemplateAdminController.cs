using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/sms-templates")]
[Authorize(Roles = "Superuser")]
public class SMSTemplateAdminController : ControllerBase
{
    private readonly ISMSTemplateAdminService _smsTemplateAdminService;

    public SMSTemplateAdminController(ISMSTemplateAdminService smsTemplateAdminService)
    {
        _smsTemplateAdminService = smsTemplateAdminService;
    }

    /// <summary>
    /// Get all SMS templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SMSTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SMSTemplateDto>>> GetAllSMSTemplates()
    {
        var templates = await _smsTemplateAdminService.GetAllSMSTemplatesAsync();
        return Ok(templates);
    }

    /// <summary>
    /// Get SMS template by ID
    /// </summary>
    [HttpGet("{templateId:guid}")]
    [ProducesResponseType(typeof(SMSTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SMSTemplateDto>> GetSMSTemplateById(Guid templateId)
    {
        var template = await _smsTemplateAdminService.GetSMSTemplateByIdAsync(templateId);
        return Ok(template);
    }

    /// <summary>
    /// Create a new SMS template
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SMSTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SMSTemplateDto>> CreateSMSTemplate([FromBody] CreateSMSTemplateRequest request)
    {
        var template = await _smsTemplateAdminService.CreateSMSTemplateAsync(request);
        return CreatedAtAction(nameof(GetSMSTemplateById), new { templateId = template.Id }, template);
    }

    /// <summary>
    /// Update SMS template
    /// </summary>
    [HttpPut("{templateId:guid}")]
    [ProducesResponseType(typeof(SMSTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SMSTemplateDto>> UpdateSMSTemplate(
        Guid templateId, 
        [FromBody] UpdateSMSTemplateRequest request)
    {
        var template = await _smsTemplateAdminService.UpdateSMSTemplateAsync(templateId, request);
        return Ok(template);
    }

    /// <summary>
    /// Partially update SMS template
    /// </summary>
    [HttpPatch("{templateId:guid}")]
    [ProducesResponseType(typeof(SMSTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SMSTemplateDto>> PatchSMSTemplate(
        Guid templateId, 
        [FromBody] UpdateSMSTemplateRequest request)
    {
        var template = await _smsTemplateAdminService.UpdateSMSTemplateAsync(templateId, request);
        return Ok(template);
    }

    /// <summary>
    /// Delete SMS template
    /// </summary>
    [HttpDelete("{templateId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSMSTemplate(Guid templateId)
    {
        await _smsTemplateAdminService.DeleteSMSTemplateAsync(templateId);
        return NoContent();
    }
}
