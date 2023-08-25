using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class SalasDeCinesControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCineA5KmOMenos()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            using var context1 = LocalDbDatabaseInitializer.GetDbContextLocalDb(false);
            var salasDeCine = new List<SalaDeCine>()
            {
                new SalaDeCine{Nombre = "Agora", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))}
            };

            context1.AddRange(salasDeCine);
            await context1.SaveChangesAsync();

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 5,
                Latitud = 18.481139,
                Longitud = -69.938950
            };

            using var context2 = LocalDbDatabaseInitializer.GetDbContextLocalDb(false);
            var mapper = ConfigureAutoMapper();
            var controller = new SalasDeCineController(context2, mapper, geometryFactory);
            var response = await controller.Cercanos(filtro);
            var value = response.Value;

            Assert.IsNotNull(value);
            Assert.AreEqual(2, value.Count);
        }
    }
}
