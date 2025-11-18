namespace Application.DTOs.Product
{
    public class ProductReadDto
    {
        public Guid Id { get; set; }
        public int TitleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TitleName { get; set; }   // optional, if we want to include the title name
    }
}