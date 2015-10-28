using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InformationWarehouse
{
    public class MongoEntityInformation : BsonDocument
    {
        [BsonElement("Id")]
        public string Id { get; set; }
    }
}