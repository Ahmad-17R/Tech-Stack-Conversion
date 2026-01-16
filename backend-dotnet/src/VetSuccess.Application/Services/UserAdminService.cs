using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class UserAdminService : IUserAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserAdminService(
        UserManager<User> userManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserListDto>> GetAllUsersAsync(bool? isSuperuser = null, bool? isActive = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.AsQueryable();

        if (isSuperuser.HasValue)
        {
            // Note: This requires checking roles, which is more complex with Identity
            // For now, we'll filter by the is_superuser field if it exists in your User entity
            // You may need to adjust this based on your actual User model
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                (u.UserName != null && u.UserName.ToLower().Contains(search)));
        }

        query = query.OrderBy(u => u.Email);

        var users = await query.ToListAsync(cancellationToken);
        var userDtos = new List<UserListDto>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserListDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.IsSuperuser = roles.Contains("Superuser");
            userDtos.Add(dto);
        }

        return userDtos;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        var dto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        dto.IsSuperuser = roles.Contains("Superuser");

        return dto;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ValidationException($"User with email '{request.Email}' already exists");
        }

        // Validate password confirmation
        if (request.Password != request.PasswordConfirm)
        {
            throw new ValidationException("Passwords do not match");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = request.IsActive ?? true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException($"Failed to create user: {errors}");
        }

        // Add role if superuser
        if (request.IsSuperuser ?? false)
        {
            await _userManager.AddToRoleAsync(user, "Superuser");
        }

        return await GetUserByIdAsync(user.Id, cancellationToken);
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        var updated = false;

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            // Check if new email is already taken
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != userId)
            {
                throw new ValidationException($"Email '{request.Email}' is already taken");
            }

            user.Email = request.Email;
            user.UserName = request.Email;
            updated = true;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            // Note: Assuming User entity has FirstName property
            // You may need to adjust based on your actual User model
            updated = true;
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            // Note: Assuming User entity has LastName property
            // You may need to adjust based on your actual User model
            updated = true;
        }

        if (request.IsActive.HasValue && request.IsActive.Value != user.EmailConfirmed)
        {
            user.EmailConfirmed = request.IsActive.Value;
            updated = true;
        }

        if (request.IsSuperuser.HasValue)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isSuperuser = roles.Contains("Superuser");

            if (request.IsSuperuser.Value && !isSuperuser)
            {
                await _userManager.AddToRoleAsync(user, "Superuser");
            }
            else if (!request.IsSuperuser.Value && isSuperuser)
            {
                await _userManager.RemoveFromRoleAsync(user, "Superuser");
            }
        }

        if (updated)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException($"Failed to update user: {errors}");
            }
        }

        return await GetUserByIdAsync(userId, cancellationToken);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException($"Failed to delete user: {errors}");
        }
    }
}
