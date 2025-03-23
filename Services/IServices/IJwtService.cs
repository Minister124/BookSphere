using System;
using BookSphere.Models;

namespace BookSphere.Services.IServices;

public interface IJwtService
{
        string GenerateToken(User user);
}
