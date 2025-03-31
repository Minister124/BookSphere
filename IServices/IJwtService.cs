using System;
using BookSphere.Models;

namespace BookSphere.IServices;

public interface IJwtService
{
        string GenerateToken(User user);
}
