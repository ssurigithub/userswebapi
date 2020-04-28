using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace webapi_mongo_auth_tuts.Models
{
    public class Address
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}
        public string Street {get;set;}
        public string Suite {get;set;}
        public string City {get;set;} 
        public string State {get;set;} 
        public string Zip {get;set;}
        
        [BsonElement("Date")]
        public DateTime CreatedDate {get;set;} = DateTime.Now;
    }

}