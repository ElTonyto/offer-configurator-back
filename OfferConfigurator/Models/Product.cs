using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfferConfigurator.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CatalogId { get; set; }
        public Nullable<int> RemainingStock { get; set; }
        public string Price { get; set; }
        public string Brand { get; set; }
        public string Options { get; set; } // TODO: Enum je pense
    }

    public class ProductBody
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CatalogId { get; set; }
        public string Price { get; set; }
        public string Brand { get; set; }
        public Nullable<int> RemainingStock { get; set; }
        public string Options { get; set; }
    }
}
