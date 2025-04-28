using System;

namespace BookSphere.DTOs;

public class AnnouncementDto
{
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
}
