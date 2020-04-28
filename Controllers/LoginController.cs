
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi_mongo_auth_tuts.Models;
using webapi_mongo_auth_tuts.Services;

namespace webapi_mongo_auth_tuts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly UserService _userService;
        public LoginController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<AuthenticationModel> Authenticate([FromBody] AuthenticationModel model)
        {
            if(model == null) return BadRequest();
            _userService.Authenticate(model);
            return model;
        }
    }
}