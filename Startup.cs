using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using UrlShortner.AuthUserAccessors;
using UrlShortner.CookieReaders;
using UrlShortner.CookieWriters;
using UrlShortner.Data;
using UrlShortner.Decryptors;
using UrlShortner.Encryptors;
using UrlShortner.KeyGenerators;
using UrlShortner.MiddleWares;
using UrlShortner.Models.Auth;
using UrlShortner.RedisCacheTicketStores;
using UrlShortner.Services.AuthService;
using UrlShortner.Services.CacheService;
using UrlShortner.Services.ShortUrlService;
using UrlShortner.Util;

namespace UrlShortner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "UrlShortner", Version = "v1"});
            });
            
            services.AddAutoMapper(typeof(Startup));

            services.AddHttpContextAccessor();

            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 0;
                })
                .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = CookieNames.ShortUrlSession;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = new TimeSpan(0, 30, 0);
                options.SessionStore = new RedisCacheTicketStore(new RedisCacheOptions()
                {
                    Configuration = Configuration.GetConnectionString("RedisConnection"),
                });
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Path = "/";
            });

            services.AddScoped<IKeyGenerator, KeyGenerator>();

            services.AddScoped<IEncryptor, Encryptor>(o => new Encryptor(Configuration["CookieSecret"]));
            services.AddScoped<IDecryptor, Decryptor>(o => new Decryptor(Configuration["CookieSecret"]));
            services.AddScoped<IShortUrlService, ShortUrlService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>(o =>
            {
                var redisCacheOptions = new RedisCacheOptions()
                {
                    Configuration = Configuration.GetConnectionString("RedisConnection"),
                };
                
                return new CacheService(new RedisCache(redisCacheOptions));
            });
            services.AddScoped<IAuthUserAccessor, AuthUserAccessor>();
            services.AddScoped<ICookieWriter, CookieWriterToResponse>(o =>
            {
                var httpContextAccessor = o.GetRequiredService<IHttpContextAccessor>();
                var encryptor = o.GetRequiredService<IEncryptor>();
                
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(30),
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                };
                
                return new CookieWriterToResponse(httpContextAccessor,cookieOptions,encryptor);
            });
            services.AddScoped<ICookieReader, CookieReaderToResponse>();
            
            services.AddCors(options =>
            {
                options.AddPolicy(name: "FrontEnd",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var applicationServices = app.ApplicationServices;
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortner v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors("FrontEnd");

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseAddAnonymousCookies();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseAutoWrapper(new AutoWrapperOptions
            {
                ShowApiVersion = true,
                ShowStatusCode = true,
                // SwaggerPath = "/yourswaggerpath"
            });
        }
    }
}