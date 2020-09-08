using Domain;

namespace Application.Security
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}