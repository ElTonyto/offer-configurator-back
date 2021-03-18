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
    public class OffersController : ControllerBase
    {
        private readonly OfferService _offerService;

        public OffersController(OfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public ActionResult<List<Offer>> Get() =>
             StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get all offers", Data = _offerService.Get() });

        [HttpGet("{id:length(24)}", Name = "GetOffer")]
        public ActionResult<Offer> Get(string id)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Offer not found", Data = new List<object>() });
            }

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Get an offer", Data = offer });
        }

        [HttpPost]
        public ActionResult<Offer> Create(OfferBody offerBody)
        {
            Product product = _offerService.productService.Get(offerBody.ProductId);

            if (product == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            Offer offer = _offerService.Create(offerBody, product);

            object result = CreatedAtRoute("GetOffer", new { id = offer.Id.ToString() }, offer).Value;
            return StatusCode(201, new HttpResponse { Status = 201, Type = "CREATED", Message = "Offer created", Data = result });
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Offer offerIn)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Offer not found", Data = new List<object>() });
            }

            _offerService.Update(id, offerIn);

            object result = CreatedAtRoute("GetOffer", new { id = offer.Id.ToString() }, offer).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Offer changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Offer not found", Data = new List<object>() });
            }

            _offerService.Remove(offer.Id);

            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Offer deleted", Data = new List<object>() });
        }
    }
}
