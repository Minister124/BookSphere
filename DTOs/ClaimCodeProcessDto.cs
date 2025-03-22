using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class ClaimCodeProcessDto
{
        [Required]
        [StringLength(50)]
        public string ClaimCode { get; set; }
        
        [Required]
        [StringLength(50)]
        public string MembershipId { get; set; }
}
