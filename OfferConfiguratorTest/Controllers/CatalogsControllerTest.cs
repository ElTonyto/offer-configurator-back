
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfferConfigurator.Controllers;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using OfferConfigurator.Services;

namespace OfferConfiguratorTest.Controllers
{
    [TestClass]
    public class CatalogsControllerTest
    {
        // GET ALL
        [TestMethod]
        public void GetAllReturnCatalog()
        {
            List<Catalog> expectedData = new List<Catalog>();
            expectedData.Add(new Catalog { Id = "test", CreatedAt = DateTime.Now, Name = "Test" });

            var mockRepository = new Mock<ICatalogService>();
            mockRepository.Setup(x => x.Get())
                .Returns(expectedData);

            var controller = new CatalogsController(mockRepository.Object);

            // Act
            ObjectResult objectResult = controller.Get();
            HttpResponse result = objectResult.Value as HttpResponse;
            List<Catalog> resultData = result.Data as List<Catalog>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual("OK", result.Type);
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(1, resultData.Count);
        }

        [TestMethod]
        public void GetAllReturnCatalog_NotFound()
        {
            List<Catalog> expectedData = new List<Catalog>();
            //expectedData.Add(new Catalog { Id = "test", CreatedAt = DateTime.Now, Name = "Test" });

            var mockRepository = new Mock<ICatalogService>();
            mockRepository.Setup(x => x.Get())
                .Returns(expectedData);

            var controller = new CatalogsController(mockRepository.Object);

            // Act
            ObjectResult objectResult = controller.Get();
            HttpResponse result = objectResult.Value as HttpResponse;
            List<Catalog> resultData = result.Data as List<Catalog>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual("OK", result.Type);
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(0, resultData.Count);
        }

        // GET BY ID
        [TestMethod]
        public void GetReturnCatalogById()
        {
            Catalog expectedData = new Catalog { Id = "test", CreatedAt = DateTime.Now, Name = "Test" };
            var mockRepository = new Mock<ICatalogService>();
            mockRepository.Setup(x => x.Get("test"))
                .Returns(expectedData);

            var controller = new CatalogsController(mockRepository.Object);

            // Act
            ObjectResult objectResult = controller.Get("test");
            HttpResponse result = objectResult.Value as HttpResponse;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual("OK", result.Type);
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual(expectedData, result.Data);
        }

        [TestMethod]
        public void GetReturnCatalogById_NotFound()
        {
            Catalog expectedData = new Catalog { Id = "test", CreatedAt = DateTime.Now, Name = "Test" };
            var mockRepository = new Mock<ICatalogService>();
            mockRepository.Setup(x => x.Get("test"))
                .Returns(expectedData);

            var controller = new CatalogsController(mockRepository.Object);

            // Act
            ObjectResult objectResult = controller.Get("test2");
            HttpResponse result = objectResult.Value as HttpResponse;
            List<object> resultData = result.Data as List<object>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, objectResult.StatusCode);
            Assert.AreEqual("NOT_FOUND", result.Type);
            Assert.AreEqual("Error", result.Status);
            Assert.AreEqual(0, resultData.Count);
        }
    }
}
