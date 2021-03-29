using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class CartService
    {
        private readonly IMongoCollection<Cart> _cart;
        IMongoDatabase database;

        public CartService(IOfferConfiguratorDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);

            _cart = database.GetCollection<Cart>(settings.CartCollectionName);
        }

        public List<Cart> Get() =>
            _cart.Find(cart => true).ToList();

        public Cart Get(string id) =>
            _cart.Find<Cart>(cart => cart.Id == id).FirstOrDefault();

        public Cart Create(CartBody cartBody)
        {
            Cart cart = new Cart
            {
                Type = cartBody.Type,
                TypeId = cartBody.TypeId,
                Quantity = cartBody.Quantity
            };

            _cart.InsertOne(cart);

            return cart;
        }

        public void Update(string id, Cart cartIn) =>
            _cart.ReplaceOne(cart => cart.Id == id, cartIn);

        public void Remove() =>
            database.DropCollection("Cart");

        public void Remove(string id) =>
            _cart.DeleteOne(cart => cart.Id == id);
    }
}