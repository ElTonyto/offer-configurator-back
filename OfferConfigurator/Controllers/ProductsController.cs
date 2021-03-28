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

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<List<Product>> Get([FromQuery] string isParent)
        {
            if (isParent == "true")
            {
                return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get all parent products", Data = _productService.GetAllParent() });
            } else if (isParent == "false")
            {
                return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get all child products", Data = _productService.GetAllChild() });
            } else
            {
                return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get all products", Data = _productService.Get() });
            }
            
        }           

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public ActionResult<Product> GetById(string id)
        {
            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get a product", Data = product });
        }

        [HttpPost]
        public ActionResult<Product> Create([FromHeader(Name = "X-ROLE")][Required] string role, ProductBody productBody)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Catalog catalog = _productService.catalogService.Get(productBody.CatalogId);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            if (productBody.ParentId == null && productBody.Options != null) {
                return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Options should be null as your product is a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null && productBody.AllOptions != null) {
                return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "AllOptions should be null as your product is not a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null) {
                Product parent = _productService.Get(productBody.ParentId);
                if (parent == null){
                    return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Parent product not found", Data = new List<object>() });
                }
            }

            Product product =_productService.Create(productBody);

            object result = CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product).Value;
            return StatusCode(201, new HttpResponse { Status = "Success", Type = "CREATED", Message = "Product created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult<Product> Update([FromHeader(Name = "X-ROLE")][Required] string role, string id, ProductBody productBody)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Catalog checkCatalog = _productService.catalogService.Get(productBody.CatalogId);

            if (checkCatalog == null && productBody.CatalogId != null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            if (productBody.ParentId == null && productBody.Options != null) {
                return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Options should be null as your product is a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null && productBody.AllOptions != null) {
                return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "AllOptions should be null as your product is not a parent", Data = new List<object>() });
            }
            if (productBody.ParentId != null) {
                Product parent = _productService.Get(productBody.ParentId);
                if (parent == null){
                    return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Parent product not found", Data = new List<object>() });
                }
            }

            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
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
            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Product changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete([FromHeader(Name = "X-ROLE")][Required] string role, string id)
        {
            if (role == null) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is not found", Data = new List<object>() });
            if (!HeaderRole.Verify(role)) return StatusCode(400, new HttpResponse { Status = "Error", Type = "BAD_REQUEST", Message = "Role is incorrect", Data = new List<object>() });

            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            _productService.Remove(product.Id);

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Product deleted", Data = new List<object>() });
        }
    }
}
