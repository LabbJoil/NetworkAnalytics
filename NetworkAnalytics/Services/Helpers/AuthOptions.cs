using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetworkAnalytics.Services.Helper;
public class AuthOptions
{
    private const string ISSUER = "NetworkAnalyticsServer";
    private const string AUDIENCE = "ManagersNetworkAnalytics";
    private const string KEY_BASE64 = "bXlzdXBlcnNlY3JldF9zZWNyZXtgfbirebgFYRTgfy80eXBlb2YgfDAxfQ==";
    private static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Convert.FromBase64String(KEY_BASE64));

    public static JwtSecurityToken NewToken(IEnumerable<Claim> claims)
        => new(
                    issuer: ISSUER,
                    audience: AUDIENCE,
                    claims: claims ?? throw new Exception("Claim has not been transferred"),
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(2)),
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

    public static TokenValidationParameters NewOptionsAuth()
        => new()
        {
            ValidateIssuer = true,
            ValidIssuer = ISSUER,
            ValidateAudience = true,
            ValidAudience = AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };

    public static int GetUserIdFromToken(ClaimsPrincipal claimsPrincipal)
    {
        var sidClaim = claimsPrincipal.FindFirst(ClaimTypes.Sid);
        int idUser = sidClaim == null ? throw new Exception("Session ID not transferred") : int.Parse(sidClaim.Value);
        return idUser;
    }
}
