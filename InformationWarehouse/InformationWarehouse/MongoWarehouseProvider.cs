using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Warehouse;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InformationWarehouse
{
    public class MongoWarehouseProvider : DataWarehouseProvider
    {
        private const string Hashcode = "Hashcode";
        private const string Id = "Id";
        private const string IsCurrent = "IsCurrent";

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

        private BsonDocument FindOne(FilterDefinition<BsonDocument> filter)
        {
            var documents = Find(filter);
            return documents?.Result?.FirstOrDefault();
        }
        async private Task<IEnumerable<BsonDocument>> Find(FilterDefinition<BsonDocument> filter)
        {
            var documents = default(IEnumerable<BsonDocument>);

            if(filter != null)
            { 
                documents = await CollectionInformation?.Find(filter).ToListAsync();
            }

            return documents;
        }

        public IEnumerable<Dictionary<string, IEnumerable<object>>> DigInformation(string question)
        {
            var response = Enumerable.Empty<Dictionary<string, IEnumerable<object>>>();

            var queryresult = Find(
                Builders<BsonDocument>.Filter.Gt("Information.price", 0.5) &
                (Builders<BsonDocument>.Filter.Regex("Information.title", ".*STAR.*"))
            ).Result;
            foreach(var queryresultitem in queryresult)
            {
                var responsedictionary = queryresultitem.ToInformationDictionary();
                response = response.Concat(new[] { responsedictionary });
            }

            return response;
        }

        public void StoreInformation(Dictionary<string, IEnumerable<object>> information)
        {
            var hashcode = information.GetInformationHashcode();
            var id = information.GetInformationId();

            var currentInformationFilter = 
                Builders<BsonDocument>.Filter.Eq(Id, id) &
                Builders<BsonDocument>.Filter.Eq(IsCurrent, true)
            ;
            var currentInformation = FindOne(currentInformationFilter);
            var currentInformationHashcode = 
                currentInformation == null ? 
                null : 
                currentInformation.Elements.FirstOrDefault(element => element.Name == Hashcode).Value
            ;

            var isCurrentInformation = currentInformation != null && hashcode == currentInformationHashcode;
            //  Zu übertragende Information hat einen anderen Stand als der aktuelle Stand >> Übertragung in das Data-Warehouse
            if(!isCurrentInformation)
            {
                information = information.Where(entry => !entry.Key.Contains(".")).ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value?.Select(value => value)
                );

                //  Aktuelle Information mit "nicht aktuell" markieren
                if (currentInformation != null)
                {
                    currentInformation.Set(IsCurrent, false);
                    CollectionInformation.UpdateOneAsync(currentInformationFilter, currentInformation);
                }
                
                CollectionInformation.InsertOneAsync(new BsonDocument(new Dictionary<string, object>{
                    //  Information ist letzte aktuelle Information
                    { "IsCurrent", true },
                    //  ID des Informationssatz auf der Website
                    { Id, id },
                    //  Hashcode des abzulegenden Informationssatz
                    { Hashcode, hashcode },
                    { "Information", information }
                }));
            }
        }
    }
}