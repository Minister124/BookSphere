using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class Announcement
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [StringLength(100)]
        public string Title {get; set;}

        [Required]
        [StringLength(1000)]
        public string Content {get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate {get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate {get; set;}

        [Required]
        public bool IsActive {get; set;}

        [Required]
        public string Type {get; set;} //New Arrival, Info, Deal

        [StringLength(500)]
        public string ImageUrl {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate {get; set;} = DateTime.UtcNow;

        public Guid CreatedById {get; set;}

        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }
}