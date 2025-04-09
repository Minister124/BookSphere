using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class AddToWhiteListDto
{
        [Required]
        public Guid BookId { get; set; }
}
