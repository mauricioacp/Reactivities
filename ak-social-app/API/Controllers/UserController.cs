using System;
using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RefreshToken = Application.User.RefreshToken;

namespace API.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login.Query query)
        {
            var user= await Mediator.Send(query);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(Register.Command command)
        {
            var user= await Mediator.Send(command);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [HttpGet]
        public async Task<ActionResult<User>> CurrentUser()
        {
            var user= await Mediator.Send(new CurrentUser.Query());
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<User>> FacebookLogin(ExternalLogin.Query query)
        {
            var user= await Mediator.Send(query);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [HttpPost("refreshtoken")]
        public async Task<ActionResult<User>> RefreshToken(RefreshToken.Command command)
        {
            command.RefreshToken = Request.Cookies["refreshtoken"];
            var user=await Mediator.Send(command);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        private void SetTokenCookie(string refreshtoken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshtoken",refreshtoken,cookieOptions);
        }
    }
}