using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class Review
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        public Guid BookId {get; set;}

        [Required]
        public Guid UserId {get; set;}

        [Required]
        [Range(1, 5)]
        public int Rating {get; set;}

        [StringLength(1000)]
        public string Comment {get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate {get; set;} = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User {get; set;}

        [ForeignKey("BookId")]
        public virtual Book Book {get; set;}
}