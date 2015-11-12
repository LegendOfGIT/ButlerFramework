using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Driver;

namespace InformationWarehouse
{
    public static class MongoWarehouseExtensions
    {
        private static FilterDefinition<BsonDocument> ToMongoDatabaseFilter(this Filter filter)
        {
            var mongofilter = default(FilterDefinition<BsonDocument>);

            var target = filter?.Target;
            if(!string.IsNullOrEmpty(target))
            {
                target = $"Information.{target}";

                var minimum = filter?.Minimum;
                var maximum = filter?.Maximum;
                var value = filter?.Value;

                //  Größer als
                if(minimum != null && maximum == null)
                {
                    mongofilter = Builders<BsonDocument>.Filter.Gt(target, (double)minimum);
                }
                //  Prüfung auf bestimmten Wert
                else if (value != null)
                {
                    //  Stimmt mit genauem Wert überein
                    mongofilter = Builders<BsonDocument>.Filter.Eq(target, value);

                    //  Auswertung einer RegularExpression
                    if (value is string && ((string)value).Contains("*"))
                    {
                        mongofilter = Builders<BsonDocument>.Filter.Regex(target, (string)value);
                    }
                }
            }

            return mongofilter;
        }
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
        public static FilterDefinition<BsonDocument> ToMongoDatabaseFilter(this IEnumerable<Filter> filter)
        {
            var mongofilter = default(FilterDefinition<BsonDocument>);

            filter?.ToList().ForEach(f => {
                //  Generierung eines Filters, falls dieser festgelegt wurde
                if(!string.IsNullOrEmpty(f.Target))
                {
                    var targetfilter = f.ToMongoDatabaseFilter();
                    
                    //  Es gibt noch keine Filterbasis. Datenbankfilter neu erstellen
                    if(mongofilter == default(FilterDefinition<BsonDocument>) && targetfilter != null)
                    {
                        mongofilter = targetfilter;
                    }
                }

                var subfilter = default(FilterDefinition<BsonDocument>);
                //  Und-Verbindungen
                subfilter = f.And.ToMongoDatabaseFilter();
                if(subfilter != default(FilterDefinition<BsonDocument>))
                {
                    //  Es gibt noch keine Filterbasis. Datenbankfilter neu erstellen
                    if (mongofilter == default(FilterDefinition<BsonDocument>))
                    {
                        mongofilter = subfilter;
                    }
                    else
                    {
                        mongofilter = mongofilter & subfilter;
                    }
                }

                //  Oder-Verbindungen
                subfilter = f.Or.ToMongoDatabaseFilter();
                if (subfilter != default(FilterDefinition<BsonDocument>))
                {
                    //  Es gibt noch keine Filterbasis. Datenbankfilter neu erstellen
                    if (mongofilter == default(FilterDefinition<BsonDocument>))
                    {
                        mongofilter = subfilter;
                    }
                    else
                    {
                        mongofilter = mongofilter | subfilter;
                    }
                }
            });

            return mongofilter;
        }
    }
}