using Application.Security;
using Domain;

namespace Infrastucture.Security
{
    public class JwtGenerator :IJwtGenerator
    {
        public string CreateToken(AppUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}