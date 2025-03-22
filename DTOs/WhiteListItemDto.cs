using System;

namespace BookSphere.DTOs;

public class WhiteListItemDto
{
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public BookDto Book { get; set; }
        public DateTime AddedDate { get; set; }
}
