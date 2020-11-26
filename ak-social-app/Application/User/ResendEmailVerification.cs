using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.User
{
    public class ResendEmailVerification
    {
        public class Query : IRequest
        {
            public string Email { get; set; }
            public string Origin { get; set; }
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly IEmailSender _emailSender;
            private readonly UserManager<AppUser> _userManager;

            public Handler(UserManager<AppUser> userManager, IEmailSender emailSender)
            {
                _userManager = userManager;
                _emailSender = emailSender;
            }

            public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //convert to pure string
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                //development origin  localhost... production origin?? 
                var verifyUrl = $"{request.Origin}/user/verifyEmail?token={token}&email={request.Email}";
                //pretty nasty to type this stuff out
                var message = $"<p>Please click the below link to verify your email address:</p>" +
                              $"<p><a href='{verifyUrl}'>{verifyUrl}</a></p>";

                await _emailSender.SendEmailAsync(request.Email, "Please verify email address", message);

                return Unit.Value;
            }
        }
    }
}