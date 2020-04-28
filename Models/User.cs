using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace webapi_mongo_auth_tuts.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id {get;set;}

        [Required]
        [MinLength(5)]
        [StringLength(255)]
        public string FirstName {get;set;}


        [Required]
        [MinLength(5)]
        [StringLength(255)]
        public string LastName {get;set;}


        [Required]
        [MinLength(4)]
        [EnumDataType(typeof(Status))]
        public string Status {get;set;} = "Active";

        [Required]
        [MinLength(4)]
        
        [EnumDataType(typeof(Roles))]
        public string Role {get;set;} = "Developer";


        [Required]
        [MinLength(8)]
        [StringLength(255)]
        
        public string Email {get;set;}


        [Required]
        [MinLength(5)]
        [StringLength(1024)]
        public string Password {get;set;}

        [BsonElement("Date")]
        public DateTime CreatedDate {get;set;} = DateTime.Now;
        public string Salt { get; internal set; }
    }

    public enum Roles
    {
        Developer, Architect, Manager, Director
    }

    public enum Status
    {
        Active, Inactive
    }

}