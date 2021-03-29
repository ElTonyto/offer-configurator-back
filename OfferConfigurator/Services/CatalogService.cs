using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IMongoCollection<Catalog> _catalog;

        public CatalogService(IOfferConfiguratorDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _catalog = database.GetCollection<Catalog>(settings.CatalogsCollectionName);
        }

        public List<Catalog> Get() =>
            _catalog.Find(catalog => true).ToList();

        public Catalog Get(string id) =>
            _catalog.Find<Catalog>(catalog => catalog.Id == id).FirstOrDefault();

        public Catalog GetByName(string name) =>
            _catalog.Find<Catalog>(catalog => catalog.Name == name).FirstOrDefault();

        public Catalog Create(CatalogBody catalogBody)
        {
            Catalog catalog = new Catalog
            {
                CreatedAt = DateTime.Now,
                Name = catalogBody.Name
            };

            _catalog.InsertOne(catalog);

            return catalog;
        }

        public void Update(string id, Catalog catalogIn) =>
            _catalog.ReplaceOne(catalog => catalog.Id == id, catalogIn);

        public void Remove(Catalog catalogIn) =>
            _catalog.DeleteOne(catalog => catalog.Id == catalogIn.Id);

        public void Remove(string id) =>
            _catalog.DeleteOne(catalog => catalog.Id == id);
    }

    public interface ICatalogService
    {
        public List<Catalog> Get();
        public Catalog Get(string id);
        public Catalog GetByName(string name);
        public Catalog Create(CatalogBody catalogBody);
        public void Update(string id, Catalog catalogIn);
        public void Remove(Catalog catalogIn);
        public void Remove(string id);
    }
}