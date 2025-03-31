using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IUserService
{
            //Authentication
            Task<AuthResponse> RegisterAsync(RegisterDto register);
            Task<AuthResponse> LoginAsync(LoginDto login);

            //User Profile
            Task<UserDto> GetUserProfileAsync(Guid userId);
            Task<UserDto> UpdateUserProfileAsync(Guid userId, UserDto user);

            //Role Management for admin
            Task<bool> AssignRoleAsync(Guid userId, string Role);

            //Check if user exist
            Task<bool> IfUserExist(Guid userId);

            // User discount information
            Task<int> GetSuccessfulOrderCount(Guid userId);
            Task<bool> HasStackableDiscount(Guid userId);
            Task<bool> UseStackableDiscount(Guid userId);
}
