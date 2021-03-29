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
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult<List<Catalog>> Get() =>
            StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get all cart", Data = _cartService.Get() });

        [HttpGet("{id:length(24)}", Name = "GetCart")]
        public ActionResult<Catalog> Get(string id)
        {
            Cart cart = _cartService.Get(id);

            if (cart == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Cart not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Get a cart", Data = cart });
        }

        [HttpPost]
        public ActionResult<Cart> Create(CartBody cartBody)
        {
            Cart cart = _cartService.Create(cartBody);

            object result = CreatedAtRoute("GetCart", new { id = cart.Id.ToString() }, cart).Value;
            return StatusCode(201, new HttpResponse { Status = "Success", Type = "CREATED", Message = "Cart inserted", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult<Offer> Update(string id, CartBody cartBody)
        {
            Cart cart = _cartService.Get(id);

            if (cart == null)
            {
                return StatusCode(404, new HttpResponse { Status = "Error", Type = "NOT_FOUND", Message = "Cart not found", Data = new List<object>() });
            }

            cart.Quantity = (cartBody.Quantity == null) ? cart.Quantity: cartBody.Quantity;
            cart.Type = (cartBody.Type == null) ? cart.Type : cartBody.Type;
            cart.TypeId = (cartBody.TypeId == null) ? cart.TypeId : cartBody.TypeId;

            _cartService.Update(id, cart);

            object result = CreatedAtRoute("GetOffer", new { id = cart.Id.ToString() }, cart).Value;
            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Cart changed", Data = result });
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _cartService.Remove();

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Cart deleted", Data = new List<object>() });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            _cartService.Remove(id);

            return StatusCode(200, new HttpResponse { Status = "Success", Type = "OK", Message = "Cart deleted", Data = new List<object>() });
        }
    }
}
