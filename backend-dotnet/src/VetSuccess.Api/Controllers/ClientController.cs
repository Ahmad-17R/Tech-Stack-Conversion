using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Client;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers;

[ApiController]
[Route("api/v1/call-center/clients")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Search clients by name, email, or phone
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClientListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClientListDto>>> SearchClients(
        [FromQuery] string? search,
        [FromQuery(Name = "practice_odu_id")] string? practiceOduId)
    {
        var clients = await _clientService.SearchClientsAsync(search, practiceOduId);
        return Ok(clients);
    }

    /// <summary>
    /// Get client details by ODU ID
    /// </summary>
    [HttpGet("{oduId}")]
    [ProducesResponseType(typeof(ClientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDetailDto>> GetClientDetails(string oduId)
    {
        var client = await _clientService.GetClientDetailsAsync(oduId);
        return Ok(client);
    }

    /// <summary>
    /// Update client information
    /// </summary>
    [HttpPut("{oduId}")]
    [ProducesResponseType(typeof(ClientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDetailDto>> UpdateClient(string oduId, [FromBody] ClientUpdateDto updateDto)
    {
        var client = await _clientService.UpdateClientAsync(oduId, updateDto);
        return Ok(client);
    }

    /// <summary>
    /// Partially update client information
    /// </summary>
    [HttpPatch("{oduId}")]
    [ProducesResponseType(typeof(ClientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDetailDto>> PatchClient(string oduId, [FromBody] ClientUpdateDto updateDto)
    {
        var client = await _clientService.UpdateClientAsync(oduId, updateDto);
        return Ok(client);
    }
}
