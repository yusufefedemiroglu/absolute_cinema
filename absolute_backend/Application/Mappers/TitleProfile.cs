using AutoMapper;
using Application.DTOs.Titles;
using Core;

namespace Application.Mappers
{
    public class TitleProfile : Profile
    {
        private const string ImageBase = "https://image.tmdb.org/t/p/w500";

        public TitleProfile()
        {
            CreateMap<Title, TitleLiteDto>()
                .ForMember(dest => dest.PosterPath,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.PosterPath)
                        ? ""
                        : $"{ImageBase}{src.PosterPath}"
                    ))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src =>
                        src.TitleGenres.Select(g => g.Genre.Name).ToList()
                    ));

            CreateMap<Title, TitleDetailDto>()
                .ForMember(dest => dest.PosterPath,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.PosterPath)
                        ? ""
                        : $"{ImageBase}{src.PosterPath}"
                    ))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src =>
                        src.TitleGenres.Select(g => g.Genre.Name).ToList()
                    ));
        }
    }
}