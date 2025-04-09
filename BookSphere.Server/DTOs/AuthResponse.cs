using System;

namespace BookSphere.DTOs;

public class AuthResponse
{
        public string Token { get; set; }
        public UserDto User { get; set; }
}
