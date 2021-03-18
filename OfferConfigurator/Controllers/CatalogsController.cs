using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using System.Collections.Generic;

namespace OfferConfigurator.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogsController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public ActionResult<List<Catalog>> Get() =>
            StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get all catalogs", Data = _catalogService.Get() });

        [HttpGet("{id:length(24)}", Name = "GetCatalog")]
        public ActionResult<Catalog> Get(string id)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get a catalog", Data = catalog });
        }

        [HttpPost]
        public ActionResult<Catalog> Create(CatalogBody catalogBody)
        {
            Catalog alreadyExist = _catalogService.GetByName(catalogBody.Name);

            if (alreadyExist != null)
            {
                return StatusCode(409, new HttpResponse { Status = 409, Type = "CONFLICT", Message = "Catalog already exists", Data = new List<object>() });
            }

            Catalog catalog = _catalogService.Create(catalogBody);

            object result = CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog).Value;
            return StatusCode(201, new HttpResponse { Status = 201, Type = "CREATED", Message = "Catalog created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Catalog catalogIn)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            _catalogService.Update(id, catalogIn);

            object result = CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Catalog changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            _catalogService.Remove(catalog.Id);

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Catalog deleted", Data = new List<object>() });
        }
    }
}
