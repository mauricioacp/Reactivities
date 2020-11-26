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
            var user = await Mediator.Send(query);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(Register.Command command)
        {
            command.Origin = Request.Headers["origin"];
            await Mediator.Send(command);
            return Ok("Registration successful - please check your email");
        }

        [HttpGet]
        public async Task<ActionResult<User>> CurrentUser()
        {
            var user = await Mediator.Send(new CurrentUser.Query());
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<User>> FacebookLogin(ExternalLogin.Query query)
        {
            var user = await Mediator.Send(query);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [HttpPost("refreshtoken")]
        public async Task<ActionResult<User>> RefreshToken(RefreshToken.Command command)
        {
            command.RefreshToken = Request.Cookies["refreshtoken"];
            var user = await Mediator.Send(command);
            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<ActionResult> VerifyEmail(ConfirmEmail.Command command)
        {
            var result = await Mediator.Send(command);
            if (!result.Succeeded) return BadRequest("Problem verifying email address");
            return Ok("Email confirmed - you can now login");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] ResendEmailVerification.Query query)
        {
            query.Origin = Request.Headers["origin"];
            await Mediator.Send(query);
            return Ok("Email verification link sent - please check email");
        }

        private void SetTokenCookie(string refreshtoken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshtoken", refreshtoken, cookieOptions);
        }
    }
}