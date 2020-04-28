using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi_mongo_auth_tuts.Models;
using webapi_mongo_auth_tuts.Services;

namespace webapi_mongo_auth_tuts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly UserService _userService;

        // Inject UsersService  to the controller constructor through DI
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // lets try 3 different return types: specific type:

        // if you have HttpGet attribute, that takes precedence over everything.
        // if you dont have HttpGet and use actioname, then that name will have to be used in the route.
        // if you dont have actionname, then the method name will have to be used. in both cases, the route should include action.
        
        [ActionName("GetUsersReturnsSpecificType")]
        //[HttpGet] //commenting this out
        [NonAction]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IList<User> GetUsers()
        {
            var list =  _userService.Get();
            if(list.Count > 0 ){
                return list;
            }
            return list; // we can't return 2 different types of response as our return type is a specific type. for those situations use IActionResult or ActionResult<T>
        }

        //[HttpGet]
        [NonAction]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsersWithIActionResult()
        {
            var list = _userService.Get();
            if(list.Count > 0) return Ok(list);
            return NotFound();
        }

        [HttpGet(Name="Get")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public ActionResult<IList<User>> GetUsersWithActionResult()
        {
            var x = HttpContext.User.Identity.IsAuthenticated;
            var list = _userService.Get();
            if(list.Count > 0) return list;
            return NotFound();
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]   
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]   
        public async Task<ActionResult<User>> GetUser(string id)
        {
            try{
            var user = await _userService.GetUser(id);
            if(user != null) return user;
            }
            catch(Exception err){
                // log error
                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]   
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] User user)
        {
            // check if its a valid user object
            if(user == null) return BadRequest();

            // check validations // this might not be required as the Model state is already checked.
            if(!ModelState.IsValid){
                var messages = (from x in ModelState.Values select x.Errors.SelectMany(e => e.ErrorMessage));
                var errors = string.Join(";", messages);
                return StatusCode(StatusCodes.Status404NotFound, $"{{msg: Invalid object passed with errors: {errors} }}");
            }
            // check if the user exists already before saving.

            var foundUser = await _userService.FindUser(user.Email);
            if(foundUser != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"{{msg: User with the same email ${user.Email} already exists.");
            }


            await Task.Run(() =>  _userService.Create(user));
            return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] User user)
        {
            var foundUser = await _userService.GetUser(id);
            if(foundUser == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"{{msg: No user found with id: {id} }}");
            }
            try
            {
                _userService.Update(id, user);
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            
        }



    }
}