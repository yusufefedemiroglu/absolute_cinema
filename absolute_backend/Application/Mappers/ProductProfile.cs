using AutoMapper;
using Application.DTOs.Product;
using Core.Entities;

namespace Application.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Entity → ReadDto
            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.CreatedAt.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.TitleName,
                    opt => opt.MapFrom(src => src.Title != null ? src.Title.Name : null));

            // CreateDto → Entity
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // UpdateDto → Entity (partial update)
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, val) => val != null));
        }
    }
}