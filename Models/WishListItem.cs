using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class WishListItem
{
        [Key]
        public Guid Id {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime AddedDate {get; set;} = DateTime.UtcNow;
}