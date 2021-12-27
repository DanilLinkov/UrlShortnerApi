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
            CreateMap<ShortUrl, GetShortUrlDto>();
            CreateMap<CreateShortUrl, ShortUrl>();
        }
    }
}