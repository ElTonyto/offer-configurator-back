using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class OfferService
    {
        private readonly IMongoCollection<Offer> _offer;
        public ProductService productService;

        public OfferService(IOfferConfiguratorDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _offer = database.GetCollection<Offer>(settings.OffersCollectionName);
            productService = new ProductService(settings);
        }

        public List<Offer> Get() =>
            _offer.Find(offer => true).ToList();

        public Offer Get(string id) =>
            _offer.Find<Offer>(offer => offer.Id == id).FirstOrDefault();

        public Offer Create(OfferBody offerBody, Product product)
        {
            Offer offer = new Offer
            {
                CreatedAt = DateTime.Now,
                IsActive = offerBody.IsActive,
                StartAt = offerBody.StartAt,
                EndAt = offerBody.EndAt,
                SubmittedBy = offerBody.SubmittedBy,
                Product = product,
                Price = offerBody.Price
            };

            _offer.InsertOne(offer);

            return offer;
        }

        public void Update(string id, Offer offerIn) =>
            _offer.ReplaceOne(offer => offer.Id == id, offerIn);

        public void Remove(Offer offerIn) =>
            _offer.DeleteOne(offer => offer.Id == offerIn.Id);

        public void Remove(string id) =>
            _offer.DeleteOne(offer => offer.Id == id);
    }
}