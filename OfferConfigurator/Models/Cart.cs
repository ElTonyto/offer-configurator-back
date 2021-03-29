using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfferConfigurator.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }
        public string TypeId { get; set; }
        public string Quantity { get; set; }
    }

    public class CartBody
    {
        public string Type { get; set; }
        public string TypeId { get; set; }
        public string Quantity { get; set; }
    }
}
