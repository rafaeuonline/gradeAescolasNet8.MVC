using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;

namespace gradeAescolas.MVC.Utils;

public static class TokenHelper
{
    public static JwtSecurityToken ReadToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
    }

    public static string? GetClaim(JwtSecurityToken token, string claimType)
    {
        var fallbackClaims = new Dictionary<string, string[]>
    {
        { ClaimTypes.Name, new[] { "name", "unique_name", "preferred_username", "sub" } },
        { ClaimTypes.NameIdentifier, new[] { "nameid", "sub" } },
        { ClaimTypes.Email, new[] { "email" } },
        { ClaimTypes.Role, new[] { "role" } }
    };

        var claim = token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

        if (claim == null && fallbackClaims.TryGetValue(claimType, out var aliases))
        {
            foreach (var alt in aliases)
            {
                claim = token.Claims.FirstOrDefault(c => c.Type == alt)?.Value;
                if (claim != null) break;
            }
        }

        return claim;
    }

    public static List<string> GetClaims(JwtSecurityToken token, string claimType)
    {
        var fallbackClaims = new Dictionary<string, string[]>
    {
        { ClaimTypes.Name, new[] { "name", "unique_name", "preferred_username", "sub" } },
        { ClaimTypes.NameIdentifier, new[] { "nameid", "sub" } },
        { ClaimTypes.Email, new[] { "email" } },
        { ClaimTypes.Role, new[] { "role" } }
    };

        var claims = token.Claims
                          .Where(c => c.Type == claimType)
                          .Select(c => c.Value)
                          .ToList();

        if (!claims.Any() && fallbackClaims.TryGetValue(claimType, out var aliases))
        {
            foreach (var alt in aliases)
            {
                claims = token.Claims
                              .Where(c => c.Type == alt)
                              .Select(c => c.Value)
                              .ToList();

                if (claims.Any()) break;
            }
        }

        return claims;
    }
}