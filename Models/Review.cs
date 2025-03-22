using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class Review
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [Range(1, 5)]
        public int Rating {get; set;}

        [StringLength(1000)]
        public string Comment {get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate {get; set;} = DateTime.UtcNow;
}