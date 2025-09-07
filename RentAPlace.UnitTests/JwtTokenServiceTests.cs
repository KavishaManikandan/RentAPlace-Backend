using Xunit;
using FluentAssertions;
using RentAPlace.Api.Security;
using RentAPlace.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace RentAPlace.UnitTests
{
    public class JwtTokenServiceTests
    {
        private JwtTokenService CreateService()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "ThisIsASecretKeyForTests1234567890" },
                    { "Jwt:Issuer", "rentaplace.test" },
                    { "Jwt:Audience", "rentaplace.clients" },
                    { "Jwt:ExpiryMinutes", "30" }
                }!)
                .Build();

            return new JwtTokenService(config);
        }

        [Fact]
        public void CreateToken_ShouldReturn_ValidJwt()
        {
            // Arrange
            var service = CreateService();
            var user = new User { UserId = 1, Email = "test@example.com", FullName = "Test User" };

            // Act
            var token = service.CreateToken(user);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            handler.CanReadToken(token).Should().BeTrue();
        }
    }
}
