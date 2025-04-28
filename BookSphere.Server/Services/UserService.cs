using System;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSphere.Services;

public class UserService : IUserService
{
          private readonly BookSphereDbContext _context;

          private readonly IMapper _mapper;

          private readonly IJwtService _jwtService;

          public UserService(BookSphereDbContext context, IMapper mapper, IJwtService jwtService)
          {
                    _context = context;
                    _mapper = mapper;
                    _jwtService = jwtService;
          }

          public async Task<AuthResponse> RegisterAsync(RegisterDto register)
          {
                    // check if email exist or not
                    if (await _context.Users.AnyAsync(u => u.EmailAddress == register.Email)) throw new ArgumentException("Emal already Registered");


                    // Map the register data from client with User model via automapper in MappingProfile.cs
                    var user = _mapper.Map<User>(register);


                    // Hash the password sent from client
                    user.PasswordHash = HashPassword(register.Password);

                    // Generate Membership ID
                    user.MembershipId = GenerateMembershipId();

                    //Create whiteList and cart for the user
                    user.WhiteList = new WhiteList
                    {
                              CreatedDate = DateTime.UtcNow
                    };

                    user.Cart = new Cart
                    {
                              LastUpdated = DateTime.UtcNow
                    };

                    //save user to DB
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    //Generate JWT token
                    var token = _jwtService.GenerateToken(user);

                    return new AuthResponse
                    {
                              Token = token,
                              User = _mapper.Map<UserDto>(user)
                    };
          }

          public async Task<AuthResponse> LoginAsync(LoginDto login)
          {
                    //Find user by email
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.EmailAddress == login.Email);

                    if (user == null ||  !VerifyPassword(login.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid email or password");

                    var token = _jwtService.GenerateToken(user);

                    return new AuthResponse
                    {
                              Token = token,
                              User = _mapper.Map<UserDto>(user)
                    };
          }

          public async Task<UserDto> GetUserProfileAsync(Guid userId)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null) throw new KeyNotFoundException("User not found");

                    return _mapper.Map<UserDto>(user);
          }

          public async Task<UserDto> UpdateUserProfileAsync(Guid userId, UserDto userDto)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if(user == null) throw new KeyNotFoundException("User not found");

                    //update user properties
                    user.FullName = userDto.FullName;
                    user.Phone = userDto.PhoneNumber;
                    user.Address = userDto.Address;

                    //save changes
                    await _context.SaveChangesAsync();

                    return _mapper.Map<UserDto>(user);
          }

          public async Task<bool> AssignRoleAsync(Guid userId, string role)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null) throw new KeyNotFoundException("User not found");

                    if(role != "Admin" && role != "Staff" && role != "Member") throw new ArgumentException("Invalid role");

                    user.Role = role;
                    await _context.SaveChangesAsync();

                    return true;
          }

          public async Task<bool> IfUserExist(Guid userId)
          {
                    return await _context.Users.AnyAsync(u => u.Id == userId);
          }

          public async Task<int> GetSuccessfulOrderCount(Guid userId)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null) throw new KeyNotFoundException("User not found");

                    return user.SuccessfulOrder;
          }

          public async Task<bool> HasStackableDiscount(Guid userId)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null) throw new KeyNotFoundException("User not found");

                    return user.HasStackableDiscount;
          }

          public async Task<bool> UseStackableDiscount(Guid userId)
          {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null) throw new KeyNotFoundException("User not found");

                    if (!user.HasStackableDiscount) return false;

                    await _context.SaveChangesAsync();

                    return true;
          }

          private string HashPassword(string password)
          {
                    using var sha256 = SHA256.Create();
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
          }

          private bool VerifyPassword(string password, string hash)
          {
                    return HashPassword(password) == hash;
          }

          private string GenerateMembershipId()
          {
                    var random = new Random();
                    const string chars =  "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                    var id = new StringBuilder("BS-");

                    for (int i = 0; i < 6; i++)
                    {
                              id.Append(chars[random.Next(chars.Length)]);
                    }

                    return id.ToString();
          }
}
