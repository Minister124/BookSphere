using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class Order
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [StringLength(50)]
        public string ClaimCode {get; set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate {get; set;} = DateTime.UtcNow;

        [Required]
        [StringLength(10)]
        public string Status {get; set;}  //pending, accepted, Cancelled

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount {get; set;}  

        [Column(TypeName = "decimal(18,2)")]
        public decimal DecimalAmount {get; set;}  

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount {get; set;}  

        public bool AppliedBulkDiscount {get; set;} // 5% for 5+ Books
        
        public bool AppliedLoyaltyDiscount {get; set;} // 10% after 10 successful orders

        [StringLength(50)]
        public string ProcessedBy {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime? ProcessedDate {get; set;}

        [StringLength(500)]
        public string Notes {get; set;}
}
