using System;

namespace BookSphere.DTOs;

public class UpdateBookDto : CreateBookDto
{
        public Guid Id {get; set;}
}
