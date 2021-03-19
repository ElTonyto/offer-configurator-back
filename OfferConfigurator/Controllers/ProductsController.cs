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
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly VerifyHeaderId _verifyHeader;

        public ProductsController(ProductService productService, VerifyHeaderId verifyHeader)
        {
            _productService = productService;
            _verifyHeader = verifyHeader;
        }

        [HttpGet]
        public ActionResult<List<Product>> Get() =>
            StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get all products", Data = _productService.Get() });

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public ActionResult<Product> Get(string id)
        {
            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get a product", Data = product });
        }

        [HttpPost]
        public ActionResult<Product> Create([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, ProductBody productBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            Catalog catalog = _productService.catalogService.Get(productBody.CatalogId);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            if (productBody.ParentId == null && productBody.Options != null) {
                return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Options should be null as your product is a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null && productBody.AllOptions != null) {
                return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "AllOptions should be null as your product is not a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null) {
                Product parent = _productService.Get(productBody.ParentId);
                if (parent == null){
                    return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Parent product not found", Data = new List<object>() });
                }
            }

            Product product =_productService.Create(productBody);

            object result = CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product).Value;
            return StatusCode(201, new HttpResponse { Status = 201, Type = "CREATED", Message = "Product created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult<Product> Update([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id, ProductBody productBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            Catalog checkCatalog = _productService.catalogService.Get(productBody.CatalogId);

            if (checkCatalog == null && productBody.CatalogId != null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            if (productBody.ParentId == null && productBody.Options != null) {
                return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Options should be null as your product is a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null && productBody.AllOptions != null) {
                return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "AllOptions should be null as your product is not a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null) {
                Product parent = _productService.Get(productBody.ParentId);
                if (parent == null){
                    return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Parent product not found", Data = new List<object>() });
                }
            }

            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            product.ParentId = (productBody.ParentId == null) ? product.ParentId : productBody.ParentId;
            product.CatalogId = (productBody.CatalogId == null) ? product.CatalogId : productBody.CatalogId;
            product.Brand = (productBody.Brand == null) ? product.Brand : productBody.Brand;
            product.Description = (productBody.Description == null) ? product.Description : productBody.Description;
            product.Name = (productBody.Name == null) ? product.Name : productBody.Name;
            product.Price = (productBody.Price == null) ? product.Price : productBody.Price;
            product.RemainingStock = (productBody.RemainingStock == null) ? product.RemainingStock : productBody.RemainingStock;
            product.Options = (productBody.Options == null) ? product.Options : productBody.Options;
            product.AllOptions = (productBody.AllOptions == null) ? product.AllOptions : productBody.AllOptions;


            _productService.Update(id, product);

            object result = CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Product changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            _productService.Remove(product.Id);

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Product deleted", Data = new List<object>() });
        }
    }
}
