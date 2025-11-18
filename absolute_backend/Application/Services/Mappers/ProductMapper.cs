using Application.DTOs.Product;
using Core.Entities;

namespace Application.Mappers
{
    public static class ProductMapper
    {
        // entity to read dto
        public static ProductReadDto ToReadDto(Product p)
        {
            return new ProductReadDto
            {
                Id = p.Id,
                TitleId = p.TitleId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                TitleName = p.Title?.Name
            };
        }

        // create dto to entity
        public static Product ToEntity(ProductCreateDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                Stock = dto.Stock,
                CreatedAt = DateTime.UtcNow
            };
        }

        // update dto to entity partial?
        public static void UpdateEntity(Product entity, ProductUpdateDto dto)
        {
            if (dto.TitleId.HasValue)
                entity.TitleId = dto.TitleId.Value;

            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                entity.Description = dto.Description;

            if (dto.Price.HasValue)
                entity.Price = dto.Price.Value;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                entity.ImageUrl = dto.ImageUrl;

            if (dto.Stock.HasValue)
                entity.Stock = dto.Stock.Value;
        }
    }
}