using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class WhiteList
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        public Guid UserId {get; set;}
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate {get; set;} = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User {get; set;}

        public virtual ICollection<WhiteListItem> WhiteListItems {get; set;}

}