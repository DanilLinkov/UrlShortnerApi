using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UrlShortner.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        // public List<ShortUrl> ShortUrls { get; set; }
    }
}