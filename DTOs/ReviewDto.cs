using System;

namespace BookSphere.DTOs;

public class ReviewDto
{
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
}
