using System;

namespace UrlShortner.Models.Auth
{
    public class AnonymousUserAuthTicket
    {
        public Guid AnonymousUserId { get; set; }
        public Guid AnonymousUserSessionId { get; set; }
    }
}