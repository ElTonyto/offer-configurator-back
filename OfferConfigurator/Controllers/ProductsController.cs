﻿using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
using OfferConfigurator.Response;
using System.Collections.Generic;

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
        public ActionResult<List<Product>> Get() =>
           StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get all offers", Data = _productService.Get() });

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
        public ActionResult<Product> Create(ProductBody productBody)
        {
            Catalog catalog = _productService.catalogService.Get(productBody.CatalogId);

            if (catalog == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Catalog not found", Data = new List<object>() });
            }

            Product product =_productService.Create(productBody);

            object result = CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product).Value;
            return StatusCode(201, new HttpResponse { Status = 201, Type = "CREATED", Message = "Product created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Product productIn)
        {
            Product product = _productService.Get(id);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            _productService.Update(id, productIn);

            object result = CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Product changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
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
