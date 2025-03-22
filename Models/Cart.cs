using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class Cart
{
        [Key]
        public Guid Id {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime LastUpdated {get; set;} = DateTime.UtcNow;
}