using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MinimalApi.Models;

namespace MinimalApi.Endpoints
{
    public static class AuthenticationEndpoints
    {

       public static void AddAuthenticationEndpoints(this WebApplication app)
        {
            app.MapPost("/api/token", (IConfiguration configuration, [FromBody] AuthenticationData data) =>
            {
                var user = ValidateCredentials(data);

                if (user == null)
                {
                    return Results.Unauthorized();
                }
                string token = GenerateToken(user, configuration);
                return Results.Ok(token);
            });
        }



          

        private static string GenerateToken(UserData user, IConfiguration configuration)
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
            claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

            var token = new JwtSecurityToken(
                configuration.GetValue<string>("Authentication:Issuer"),
                configuration.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(1),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);




        }

        private static UserData? ValidateCredentials(AuthenticationData data)
        {
            // This is not production code, replace this with a call to your auth system
            if (CompareValues(data.UserName, "esan") &&
               CompareValues(data.Password, "Test123"))
            {
                return new UserData(1, "Esan", "Orizi", data.UserName!);
            }
            if (CompareValues(data.UserName, "massa") &&
               CompareValues(data.Password, "Test123"))
            {
                return new UserData(2, "Massa", "Orizi", data.UserName!);
            }

            return null;
        }

        private static bool CompareValues(string? actual, string expected)
        {
            if (actual is not null)
            {
                if (actual.Equals(expected))
                {
                    return true;
                }
            }
            return false;
        }

    }


}

