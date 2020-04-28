using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Driver;
using webapi_mongo_auth_tuts.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace webapi_mongo_auth_tuts.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly string _secret = "This key has to be long enough to produce a 256 bit key size";

        // Inject IUserDbSettings configuration service to the service constructor through DI
        public UserService(IUsersDbSettings settings)
        {
            // get the connection string and connect.
            var client = new MongoClient(settings.ConnectionString);
            var dbName = client.GetDatabase(settings.DatabaseName);
            _users = dbName.GetCollection<User>(settings.CollectionName);
            
        }

        internal string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var genToken = GenerateTokenWithJwt(user);
            return tokenHandler.WriteToken(token);
        }

        internal string GenerateTokenWithJwt(User user)
        {
            var key = Encoding.ASCII.GetBytes(_secret);
            var header = new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            var claims = new List<Claim> 
            {
                //new Claim(ClaimTypes.Name, user.FirstName),
                new Claim("Id", user.Id)

            };
            var payload = new JwtPayload(null, null, claims, DateTime.Now, DateTime.Now.AddHours(1));
            //payload.Add("User", user);
            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }

        internal void Authenticate(AuthenticationModel model)
        {
            // find the user with email and return true or false.
            var user = FindUser(model.Email).Result;

            if(user == null) // find if user exists
            {
                model.AuthenticationResult = Result.Failed;
                model.Errors.Add($"The Email: ${model.Email} doesn't exist. Register to login.");
                return;
            }

            // find if passwords match
            var providedPassword = HashPassword(model.Password, Convert.FromBase64String(user.Salt));

            if(providedPassword != user.Password){
                model.AuthenticationResult = Result.Failed;
                model.Errors.Add($"Invalid Password. Please try again.");
                return;
            }
            
            model.Errors.Clear();
            model.AuthenticationResult = Result.Success;
            model.Token = GenerateTokenWithJwt(user);

        }

        internal List<User> Get()
        {
            return _users.Find( user => true).ToList();
        }

        internal async Task<User> GetUser(string id) => await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();

        internal async void Create(User user)
        {

            byte[] salt = new byte[128/8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var saltToUse = Convert.ToBase64String(salt);
            user.Salt = saltToUse;
            user.Password = HashPassword(user.Password, salt);
             await _users.InsertOneAsync(user);
        }

        private  string HashPassword(string password, byte[] salt)
        {
            
            // ideal way is to store the salt along with the password in the DB.

            
            // generate a salt
            

            var hashedPassword =  Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested : 256 /8
                
            ));
            return hashedPassword;
            
            
        }

        internal Task<User> FindUser(string email)
        {
            return _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        internal async void Update(string id, User user)
        {
            // create a updatedefinition:
            var updatedefinition = Builders<User>.Update
                .Set(u => u.Status, user.Status)
                .Set(u => u.FirstName, user.FirstName)
                .Set(u => u.LastName, user.LastName)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Password, user.Password)
                .Set(u => u.Role, user.Role);

            await _users.UpdateOneAsync(u => u.Id == id, updatedefinition);
                
        }
    }
}