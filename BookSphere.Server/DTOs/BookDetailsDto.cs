namespace BookSphere.DTOs;

public class BookDetailsDto : BookDto
{
        public List<ReviewDto> Reviews { get; set; }
}