using System;

namespace BookSphere.DTOs;

public class UserDto
{
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string Role { get; set; }
        public string MembershipId { get; set; }
        public int SuccessfulOrders { get; set; }
        public bool HasStackableDiscount { get; set; }
}
