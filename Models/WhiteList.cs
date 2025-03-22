using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class WhiteList
{
        [Key]
        public Guid Id {get; set;}
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate {get; set;} = DateTime.UtcNow;

}