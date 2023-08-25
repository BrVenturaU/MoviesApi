using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using PeliculasAPI;
using PeliculasAPI.Controllers;
using PeliculasAPI.Helpers;
using PeliculasAPI.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Tests
{
    public class BasePruebas
    {
        protected string DefaultUserId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string DefaultUserEmail = "ejemplo@hotmail.com";

        protected ApplicationDbContext BuildDbContext(string dbName)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName).Options;

            var dbContext = new ApplicationDbContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected ControllerContext BuildControllerContext()
        {
            var claims = new Claim[] { 
                new Claim(ClaimTypes.Name, DefaultUserEmail),
                new Claim(ClaimTypes.Email, DefaultUserEmail),
                new Claim(ClaimTypes.NameIdentifier, DefaultUserId),
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            };
        }

        protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string dbName, bool ignoreSecurity=true)
        {
            var factory = new WebApplicationFactory<Startup>();
            return factory.WithWebHostBuilder(options =>
            {
                options.ConfigureTestServices(services =>
                {
                    var dbContextDescriptor = services
                        .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (dbContextDescriptor != null)
                        services.Remove(dbContextDescriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    if (!ignoreSecurity)
                        return;

                    services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    services.AddControllers(options =>
                    {
                        options.Filters.Add(new FakeUserFilter());
                    });
                });
            });
        }
    }
}
