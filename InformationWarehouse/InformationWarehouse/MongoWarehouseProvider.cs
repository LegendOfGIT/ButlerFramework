using System;
using System.Collections.Generic;

using Data.Warehouse;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InformationWarehouse
{
    public class MongoWarehouseProvider : DataWarehouseProvider
    {
        private const string DatabaseId = "test";
        private const string CollectionInformationId = "information";

        private MongoClient DatabaseClient = default(MongoClient);
        private IMongoDatabase Database = default(IMongoDatabase);
        private IMongoCollection<BsonDocument> CollectionInformation = default(IMongoCollection<BsonDocument>);

        public MongoWarehouseProvider()
        {
            this.DatabaseClient = new MongoClient();
            this.Database = this.DatabaseClient.GetDatabase(DatabaseId);
            this.CollectionInformation = this.Database.GetCollection<BsonDocument>(CollectionInformationId);
        }

        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }

        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            information = information.PrepareInformation();

            CollectionInformation.InsertOneAsync(new BsonDocument(new Dictionary<string, object>{
                { "Id", information.GetInformationId() },
                { "Information", information }
            }));
        }
    }
}