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
        private readonly CatalogService _catalogService;
        private readonly VerifyHeaderId _verifyHeader;

        public CatalogsController(CatalogService catalogService, VerifyHeaderId verifyHeader)
        {
            _catalogService = catalogService;
            _verifyHeader = verifyHeader;
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
        public ActionResult<Catalog> Create([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, CatalogBody catalogBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

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
        public ActionResult<Catalog> Update([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id, CatalogBody catalogBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            Catalog catalog = _catalogService.Get(id);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            catalog.Name = catalogBody.Name;

            _catalogService.Update(id, catalog);

            object result = CreatedAtRoute("GetCatalog", new { id = catalog.Id.ToString() }, catalog).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Catalog changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

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
