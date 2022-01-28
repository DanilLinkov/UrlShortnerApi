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
                .ForMember(dto => dto.ShortenedUrl, opt => opt.MapFrom(o => "http://localhost:3000/"+o.ShortenedUrlId));
            CreateMap<CreateShortUrl, ShortUrl>();
            CreateMap<DeleteShortUrlDto, DeleteShortUrlDto>()
                .ForMember(dto => dto.ShortenedUrl, opt => opt.MapFrom(o => o.ShortenedUrl.Replace("http://localhost:3000/", "")));
        }
    }
}