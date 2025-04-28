using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class RegisterDto
{
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        [StringLength(200)]
        public string Address { get; set; }
}
