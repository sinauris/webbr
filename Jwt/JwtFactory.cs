using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Webbr.Jwt.Helpers;

namespace Webbr.Jwt
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string displayName, string userName, string role, string place, string id, string theme);
    }

    public class JwtFactory : IJwtFactory
    {
        #region Fields
        private readonly JwtIssuerOptions _jwtOptions;
        #endregion

        #region Constructor
        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }
        #endregion


        #region GenerateEncodedToken
        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst(Constants.JwtClaimIdentifiers.Rol),
                identity.FindFirst(Constants.JwtClaimIdentifiers.Id),
                identity.FindFirst(Constants.JwtClaimIdentifiers.Plc),
                identity.FindFirst(Constants.JwtClaimIdentifiers.Din),
                identity.FindFirst(Constants.JwtClaimIdentifiers.Thm)
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: userName.Contains("tux") ? DateTime.Now.AddDays(365) : DateTime.Now.AddDays(30),
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        #endregion

        #region GenerateClaimsIdentity
        public ClaimsIdentity GenerateClaimsIdentity(string displayName, string userName, string role, string place, string id, string theme)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Constants.JwtClaimIdentifiers.Id, id),
                new Claim(Constants.JwtClaimIdentifiers.Rol, role),
                new Claim(Constants.JwtClaimIdentifiers.Plc, place),
                new Claim(Constants.JwtClaimIdentifiers.Din, displayName),
                new Claim(Constants.JwtClaimIdentifiers.Thm, theme)
            });
        }
        #endregion

        #region ToUnixEpochDate
        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        #endregion

        #region ThrowIfInvalidOptions
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero) throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null) throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null) throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
        #endregion
    }
}