using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;

namespace VetSuccess.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/users")]
[Authorize(Roles = "Superuser")]
public class UserAdminController : ControllerBase
{
    private readonly IUserAdminService _userAdminService;

    public UserAdminController(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    /// <summary>
    /// Get all users with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserListDto>>> GetAllUsers(
        [FromQuery] bool? isSuperuser = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null)
    {
        var users = await _userAdminService.GetAllUsersAsync(isSuperuser, isActive, search);
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var user = await _userAdminService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userAdminService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUserById), new { userId = user.Uuid }, user);
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var user = await _userAdminService.UpdateUserAsync(userId, request);
        return Ok(user);
    }

    /// <summary>
    /// Partially update user
    /// </summary>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> PatchUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var user = await _userAdminService.UpdateUserAsync(userId, request);
        return Ok(user);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _userAdminService.DeleteUserAsync(userId);
        return NoContent();
    }
}
