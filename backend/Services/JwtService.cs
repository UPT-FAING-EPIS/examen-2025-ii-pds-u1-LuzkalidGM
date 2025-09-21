using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Services
{
    /// <summary>
    /// Servicio para manejo de tokens JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Genera un token JWT para el usuario
        /// </summary>
        /// <param name="user">Usuario para el cual generar el token</param>
        /// <returns>Token JWT</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token a validar</param>
        /// <returns>Claims principal si el token es válido</returns>
        ClaimsPrincipal? ValidateToken(string token);
    }

    /// <summary>
    /// Implementación del servicio JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del servicio JWT
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un token JWT para el usuario
        /// </summary>
        /// <param name="user">Usuario para el cual generar el token</param>
        /// <returns>Token JWT</returns>
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "ProjectManagement.Api";
            var audience = jwtSettings["Audience"] ?? "ProjectManagement.Api";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token a validar</param>
        /// <returns>Claims principal si el token es válido</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
                var issuer = jwtSettings["Issuer"] ?? "ProjectManagement.Api";
                var audience = jwtSettings["Audience"] ?? "ProjectManagement.Api";

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}