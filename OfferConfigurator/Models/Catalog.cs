using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfferConfigurator.Models
{
    public class Catalog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CatalogBody
    {
        public string Name { get; set; }
    }
}
