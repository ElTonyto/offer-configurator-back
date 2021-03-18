using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
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
            _catalogService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCatalog")]
        public ActionResult<Catalog> Get(string id)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return NotFound();
            }

            return catalog;
        }

        [HttpPost]
        public ActionResult<Catalog> Create(CatalogBody catalogBody)
        {
            Catalog catalog = _catalogService.Create(catalogBody);

            if (catalog == null)
            {
                return NotFound();
            }

            return CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Catalog catalogIn)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return NotFound();
            }

            _catalogService.Update(id, catalogIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return NotFound();
            }

            _catalogService.Remove(catalog.Id);

            return NoContent();
        }
    }
}
