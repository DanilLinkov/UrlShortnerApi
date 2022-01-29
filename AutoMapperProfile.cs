using System.Security.Policy;
using AutoMapper;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ShortUrl, GetShortUrlDto>()
                .ForMember(dto => dto.ShortenedUrlId, opt => opt.MapFrom(o => o.ShortenedUrlId));
            CreateMap<CreateShortUrl, ShortUrl>();
            CreateMap<DeleteShortUrlDto, DeleteShortUrlDto>()
                .ForMember(dto => dto.ShortenedUrlId, opt => opt.MapFrom(o => o.ShortenedUrlId));
        }
    }
}