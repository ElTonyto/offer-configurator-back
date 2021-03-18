using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _product;
        private CatalogService _catalogService;

        public ProductService(IOfferConfiguratorDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _product = database.GetCollection<Product>(settings.ProductsCollectionName);
            _catalogService = new CatalogService(settings);
        }

        public List<Product> Get() =>
            _product.Find(product => true).ToList();

        public Product Get(string id) =>
            _product.Find<Product>(product => product.Id == id).FirstOrDefault();

        public Product Create(ProductBody productBody)
        {
            Catalog catalog = _catalogService.Get(productBody.CatalogId);

            if (catalog == null)
            {
                return null;
            }

            Product product = new Product
            {
                Name = productBody.Name,
                CreatedAt = DateTime.Now,
                Price = productBody.Price,
                Brand = productBody.Brand,
                CatalogId = productBody.CatalogId,
                RemainingStock = productBody.RemainingStock,
                Description = productBody.Description,
                Options = productBody.Options
            };

            _product.InsertOne(product);

            return product;
        }

        public void Update(string id, Product productIn) =>
            _product.ReplaceOne(product => product.Id == id, productIn);

        public void Remove(Product productIn) =>
            _product.DeleteOne(product => product.Id == productIn.Id);

        public void Remove(string id) =>
            _product.DeleteOne(product => product.Id == id);
    }
}