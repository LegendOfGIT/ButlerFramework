using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Warehouse;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InformationWarehouse
{
    public static class MongoWarehouseExtensions
    {
        public static Dictionary<string, IEnumerable<object>> ToInformationDictionary(this BsonDocument item)
        {
            var dictionary = default(Dictionary<string, IEnumerable<object>>);

            var document = item?["Information"]?.AsBsonDocument;
            var elements = document?.Elements;
            if(elements.Any())
            {
                elements.ToList().ForEach(element =>
                {
                    dictionary = dictionary ?? new Dictionary<string, IEnumerable<object>>();
                    dictionary[element.Name] = element.Value?.AsBsonArray?.Select(
                        arrayitem =>
                            arrayitem.IsBoolean ? (object)arrayitem.AsBoolean :
                            arrayitem.IsBsonDateTime ? (object)arrayitem.ToUniversalTime() :
                            arrayitem.IsDouble ? (object)arrayitem.AsDouble :
                            arrayitem.IsInt32 ? (object)arrayitem.AsInt32 :
                            arrayitem.IsInt64 ? (object)arrayitem.AsInt64 :
                            (object)arrayitem.AsString
                    );
                });
            }

            return dictionary;
        }
    }
}