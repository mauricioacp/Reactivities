using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class RefreshToken
    {
        public class Command : IRequest<User>
        {

            public string RefreshToken { get; set; }
        }
        
        public class Handler:IRequestHandler<Command,User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IUserAccessor _userAccessor;

            public Handler(UserManager<AppUser>userManager,IJwtGenerator jwtGenerator,
                IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _jwtGenerator = jwtGenerator;
                _userManager = userManager;
            }

            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request
                    .RefreshToken);

                if (oldToken != null && !oldToken.IsActive)throw new RestException(HttpStatusCode.Unauthorized);

                if (oldToken != null)
                {
                    oldToken.Revoked=DateTime.UtcNow;
                }
                
                var refreshToken=_jwtGenerator.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);

                await _userManager.UpdateAsync(user);

                return new User(user, _jwtGenerator, refreshToken.Token);
            }
        }
    }
}