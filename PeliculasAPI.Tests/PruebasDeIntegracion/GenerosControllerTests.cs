using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class GenerosControllerTests: BasePruebas
    {
        public static readonly string _url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var dbName = Guid.NewGuid().ToString();
            var appFactory = BuildWebApplicationFactory(dbName);
            var httpClient = appFactory.CreateClient();
            var response = await httpClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();

            var generos = JsonConvert
                .DeserializeObject<List<GeneroDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(0, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildDbContext(dbName);
            var appFactory = BuildWebApplicationFactory(dbName);
            var httpClient = appFactory.CreateClient();
            context.Add(new Genero() { Nombre = "Genero 1" });
            context.Add(new Genero() { Nombre = "Genero 2" });
            await context.SaveChangesAsync();

            var response = await httpClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();

            var generos = JsonConvert
                .DeserializeObject<List<GeneroDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildDbContext(dbName);
            var context2 = BuildDbContext(dbName);
            var appFactory = BuildWebApplicationFactory(dbName);
            var httpClient = appFactory.CreateClient();
            context.Add(new Genero() { Nombre = "Genero 1" });
            await context.SaveChangesAsync();
            var response = await httpClient.DeleteAsync($"{_url}/1");
            response.EnsureSuccessStatusCode();

            var exists = await context2.Generos.AnyAsync();

            Assert.IsFalse(exists);

        }

        [TestMethod]
        public async Task BorrarGeneroRetornaUnauthorized()
        {
            var dbName = Guid.NewGuid().ToString();
            var appFactory = BuildWebApplicationFactory(dbName, false);
            var httpClient = appFactory.CreateClient();
            
            var response = await httpClient.DeleteAsync($"{_url}/1");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
