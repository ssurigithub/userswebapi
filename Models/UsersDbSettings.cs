namespace webapi_mongo_auth_tuts.Models
{
    public class UsersDbSettings : IUsersDbSettings
    {
        public string ConnectionString {get;set;} 
        public string DatabaseName {get;set;} 
        public string CollectionName {get;set;} 
        
    }

    public interface IUsersDbSettings
    {
        string ConnectionString {get;set;}
        string DatabaseName {get;set;}
        string CollectionName {get;set;}

    }

    public interface IJwtSettings
    {
        string Secret {get;set;}
    }

    public class JwtSettings : IJwtSettings
    {
        public string Secret {get;set;}
    }

    
}