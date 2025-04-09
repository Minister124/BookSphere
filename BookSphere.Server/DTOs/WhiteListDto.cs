using System;

namespace BookSphere.DTOs;

public class WhiteListDto
{
        public Guid Id { get; set; }
        public List<WhiteListItemDto> Items { get; set; }
}
