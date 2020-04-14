using System;
using MongoDB.Bson;

namespace MerchantIntegration.Core.Entity
{
    public class Customer
    {
        public ObjectId Id { get; set; }
        public string GatewayCustomerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public readonly DateTime Created = DateTime.UtcNow;
        public readonly DateTime Updated = DateTime.UtcNow;
    }
}