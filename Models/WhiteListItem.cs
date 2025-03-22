using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class WhiteListItem
{
        [Key]
        public Guid Id {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime AddedDate {get; set;} = DateTime.UtcNow;
}