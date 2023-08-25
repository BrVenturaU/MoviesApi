using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class ReviewsControllerTests: BasePruebas
    {
        private static readonly string _url = "/api/peliculas/1/reviews";
        [TestMethod]
        public async Task ObtenerReviewsPeliculaNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            var appFactory = BuildWebApplicationFactory(dbName);
            var httpClient = appFactory.CreateClient();
            var response = await httpClient.GetAsync(_url);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        }

        [TestMethod]
        public async Task ObtenerReviewsListadoVacio()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildDbContext(dbName);
            var appFactory = BuildWebApplicationFactory(dbName);
            var httpClient = appFactory.CreateClient();
            context.Add(new Pelicula { Titulo = "Pelicula 1" });
            await context.SaveChangesAsync();
            var response = await httpClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();

            var reviews = JsonConvert
                .DeserializeObject<List<ReviewDTO>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0, reviews.Count);
        }
    }
}
