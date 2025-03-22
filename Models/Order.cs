using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class Order
{
        [Key]
        public Guid Id {get; set;}

        
}
