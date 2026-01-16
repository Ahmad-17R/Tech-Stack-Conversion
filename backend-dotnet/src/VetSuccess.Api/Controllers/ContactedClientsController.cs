using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.SMSHistory;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers;

[ApiController]
[Route("api/v1/call-center")]
[Authorize]
public class ContactedClientsController : ControllerBase
{
    private readonly ISMSHistoryService _smsHistoryService;

    public ContactedClientsController(ISMSHistoryService smsHistoryService)
    {
        _smsHistoryService = smsHistoryService;
    }

    /// <summary>
    /// Get contacted clients with pagination
    /// </summary>
    [HttpGet("contacted-clients")]
    [ProducesResponseType(typeof(ContactedClientsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContactedClientsResponse>> GetContactedClients(
        [FromQuery] int page = 1,
        [FromQuery(Name = "page_size")] int pageSize = 20)
    {
        var response = await _smsHistoryService.GetContactedClientsAsync(page, pageSize);
        return Ok(response);
    }

    /// <summary>
    /// Update SMS history record
    /// </summary>
    [HttpPatch("sms-history/{uuid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSMSHistory(Guid uuid, [FromBody] SMSHistoryUpdateDto updateDto)
    {
        await _smsHistoryService.UpdateSMSHistoryAsync(uuid, updateDto);
        return NoContent();
    }
}
