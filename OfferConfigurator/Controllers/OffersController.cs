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
    public class OffersController : ControllerBase
    {
        private readonly OfferService _offerService;
        private readonly VerifyHeaderId _verifyHeader;

        public OffersController(OfferService offerService, VerifyHeaderId verifyHeader)
        {
            _offerService = offerService;
            _verifyHeader = verifyHeader;
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
        public ActionResult<Offer> Create([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, OfferBody offerBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

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
        public ActionResult<Offer> Update([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id, OfferBody offerBody)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

            Product checkProduct = _offerService.productService.Get(offerBody.ProductId);

            if (checkProduct == null && offerBody.ProductId != null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Product not found", Data = new List<object>() });
            }

            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return StatusCode(404, new HttpResponse { Status = 404, Type = "NOT_FOUND", Message = "Offer not found", Data = new List<object>() });
            }

            Product product;

            if (offerBody.ProductId != null)
            {
                product = _offerService.productService.Get(offerBody.ProductId);
            } else
            {
                product = _offerService.productService.Get(offer.Product.Id);
            }

            offer.Product = product;
            offer.IsActive = (offerBody.IsActive == null) ? offer.IsActive : offerBody.IsActive;
            offer.StartAt = (offerBody.StartAt == null) ? offer.StartAt : offerBody.StartAt;
            offer.EndAt = (offerBody.EndAt == null) ? offer.EndAt : offerBody.EndAt;
            offer.SubmittedBy = (offerBody.SubmittedBy == null) ? offer.SubmittedBy : offerBody.SubmittedBy;
            offer.Price = (offerBody.Price == null) ? offer.Price : offerBody.Price;

            _offerService.Update(id, offer);

            object result = CreatedAtRoute("GetOffer", new { id = offer.Id.ToString() }, offer).Value;
            return StatusCode(200, new HttpResponse { Status = 200, Type = "SUCCESS", Message = "Offer changed", Data = result });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete([FromHeader(Name = "X-HEADER-ID")][Required] string headerId, string id)
        {
            if (headerId == null) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id not found", Data = new List<object>() });
            if (!_verifyHeader.checkUser(headerId)) return StatusCode(400, new HttpResponse { Status = 400, Type = "BAD_REQUEST", Message = "Header id incorrect", Data = new List<object>() });

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
