namespace Application.DTOs.Product
{
    public class ProductUpdateDto
    {
        public Guid Id { get; set; }
        public int? TitleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? Stock { get; set; }
    }
}