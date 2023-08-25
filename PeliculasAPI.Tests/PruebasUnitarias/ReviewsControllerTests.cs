using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class ReviewsControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task UsuarioYaHaHechoReviewPelicula()
        {
            var dbName = Guid.NewGuid().ToString();
            var context1 = BuildDbContext(dbName);
            var context2 = BuildDbContext(dbName);
            var mapper = ConfigureAutoMapper();
            var controllerContext = BuildControllerContext();

            var movieId = CreateMovie(dbName);
            var review1 = new Review
            {
                PeliculaId = movieId,
                UsuarioId = DefaultUserId,
                Puntuacion = 5,
                Comentario = "Test"
            };
            context1.Add(review1);
            await context1.SaveChangesAsync();
            var creationReview = new ReviewCreacionDTO { 
                Comentario = "Test",
                Puntuacion = 5
            };

            var controller = new ReviewController(context2, mapper);
            controller.ControllerContext = controllerContext;

            var response = await controller.Post(movieId, creationReview);
            var result = response as BadRequestObjectResult;

            Assert.AreEqual((int) HttpStatusCode.BadRequest, result.StatusCode);

        }

        [TestMethod]
        public async Task UsuarioHaceReviewPelicula()
        {
            var dbName = Guid.NewGuid().ToString();
            var context1 = BuildDbContext(dbName);
            var context2 = BuildDbContext(dbName);
            var mapper = ConfigureAutoMapper();
            var controllerContext = BuildControllerContext();

            var movieId = CreateMovie(dbName);
            var creationReview = new ReviewCreacionDTO
            {
                Comentario = "Test",
                Puntuacion = 5
            };

            var controller = new ReviewController(context1, mapper);
            controller.ControllerContext = controllerContext;

            var response = await controller.Post(movieId, creationReview);
            var result = response as NoContentResult;

            var review = await context2.Reviews
                .FirstOrDefaultAsync(r => r.UsuarioId == DefaultUserId && r.PeliculaId == movieId);


            Assert.AreEqual((int)HttpStatusCode.NoContent, result.StatusCode);
            Assert.AreEqual(movieId, review.PeliculaId);
            Assert.AreEqual(DefaultUserId, review.UsuarioId);

        }

        private int CreateMovie(string dbName)
        {
            var context = BuildDbContext(dbName);
            var movie = new Entidades.Pelicula { Titulo = "Pelicula pruebas" };
            context.Peliculas.Add(movie);
            context.SaveChanges();

            return movie.Id;
        }
    }
}
