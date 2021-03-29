using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfferConfigurator.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public ObjectResult Get() =>
            StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get all catalogs", Data = _catalogService.Get() });

        [HttpGet("{id:length(24)}", Name = "GetCatalog")]
        public ObjectResult Get(string id)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get a catalog", Data = catalog });
        }

        [HttpPost]
        public ObjectResult Create([FromHeader(Name = "X-ROLE")][Required] string role, CatalogBody catalogBody)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Catalog alreadyExist = _catalogService.GetByName(catalogBody.Name);

            if (alreadyExist != null)
            {
                return StatusCode(409, new HttpResponse { Status = "Error", Type = "CONFLICT", Message = "Catalog already exists", Data = new List<object>() });
            }

            Catalog catalog = _catalogService.Create(catalogBody);

            object result = CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog).Value;
            return StatusCode(201, new HttpResponse { Status = "Success", Type = "CREATED", Message = "Catalog created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public ObjectResult Update([FromHeader(Name = "X-ROLE")][Required] string role, string id, CatalogBody catalogBody)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            catalog.Name = catalogBody.Name;

            _catalogService.Update(id, catalog);

            object result = CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog).Value;
            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Catalog changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public ObjectResult Delete([FromHeader(Name = "X-ROLE")][Required] string role, string id)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            _catalogService.Remove(catalog.Id);

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Catalog deleted", Data = new List<object>() });
        }
    }
}
