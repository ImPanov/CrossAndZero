using BasicAuthorization.Services.Interfaces;
using CrossZero.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Packet.Shared;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CrossAndZero.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IGrainFactory _grainFactory;

        // GET: api/<AccountController>
        public GameController(IAccountService accountService, IGrainFactory grainFactory)
        {
            _accountService = accountService;
            _grainFactory = grainFactory;
        }

        // POST api/<AccountController>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] User authUser)
        {
            var player = _grainFactory.GetGrain<IGameEngine>(Guid.NewGuid());
            var user = await _accountService.AuthenticateAsync(authUser.userName, authUser.Password);
            if (user is null)
            {
                return BadRequest(new { message = "invalid data" });
            }
            this.Response.Cookies.Append("playerId", user.UserId.ToString());
            return new JsonResult(new
            {
                token = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(authUser.userName + ":" + authUser.Password))
            });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] User authUser)
        {
            var user = await _accountService.RegisterAsync(authUser.userName, authUser.Password);
            if (user is null)
            {
                return BadRequest(new { message = "invalid data" });
            }
            this.Response.Cookies.Append("playerId", user.UserId.ToString());
            return new JsonResult(new
            {
                token = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(authUser.userName + ":" + authUser.Password))
            });
        }

        
    }
}
