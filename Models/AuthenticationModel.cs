using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webapi_mongo_auth_tuts.Models
{
    public class AuthenticationModel
    {
        

        
         [Required]
        [MinLength(8)]
        [StringLength(255)]
        public string Email {get;set;}

        [Required]
        [MinLength(5)]
        [StringLength(1024)]
        public string Password {get;set;}

        public List<string> Errors {get;set;} = new List<string>();

        public Result AuthenticationResult {get;internal set;} 

        public string Token {get;internal set;}
        
        
    }

    public enum Result {
        Success, Failed, RehashNeeded
    }
    
}