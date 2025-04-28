using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class WhiteListItem
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        public Guid WhiteListId {get; set;}

        [Required]
        public Guid BookId {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime AddedDate {get; set;} = DateTime.UtcNow;

        [ForeignKey("WhiteListId")]
        public virtual WhiteList WhiteList {get; set;}

        [ForeignKey("BookId")]
        public virtual Book Book {get; set;}

}