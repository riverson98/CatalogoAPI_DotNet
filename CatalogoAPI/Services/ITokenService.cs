using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatalogoAPI.Services;

public interface ITokenService
{
    JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims, IConfiguration _config);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipal(string token, IConfiguration _config);
}
