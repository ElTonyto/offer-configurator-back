using System;
using Microsoft.AspNetCore.Mvc;
using OfferConfigurator.Services;
using OfferConfigurator.Models;
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
            _offerService.Get();

        [HttpGet("{id:length(24)}", Name = "GetOffer")]
        public ActionResult<Offer> Get(string id)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return NotFound();
            }

            return offer;
        }

        [HttpPost]
        public ActionResult<Offer> Create(OfferBody offerBody)
        {
            Offer offer = _offerService.Create(offerBody);

            if (offer == null)
            {
                return NotFound();
            }

            return CreatedAtRoute("GetOffer", new { id = offer.Id.ToString() }, offer);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Offer offerIn)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return NotFound();
            }

            _offerService.Update(id, offerIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            Offer offer = _offerService.Get(id);

            if (offer == null)
            {
                return NotFound();
            }

            _offerService.Remove(offer.Id);

            return NoContent();
        }
    }
}
