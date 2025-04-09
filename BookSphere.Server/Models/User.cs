using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class User
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [StringLength(100)]
        public string FullName {get; set;}

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress {get; set;}

        [Required]
        [StringLength(100)]
        public string PasswordHash {get; set;}

        [Required]
        [Phone]
        [StringLength(10)]
        public string Phone {get; set;}

        [StringLength(100)]
        public string Address {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime RegisteredDate {get; set;} = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Role {get; set;}

        [StringLength(50)]
        public string MembershipId {get; set;}

        public int SuccessfulOrder {get; set;}
        public bool HasStackableDiscount {get; set;}

        public virtual ICollection<Order> Orders {get; set;} // is collection navigation
        public virtual ICollection<Review> Reviews {get; set;} // is collection navigation
        public virtual WhiteList WhiteList {get; set;} // is reference navigation
        public virtual Cart Cart {get; set;} // is reference navigation
}
