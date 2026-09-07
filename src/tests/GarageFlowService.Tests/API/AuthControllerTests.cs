using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using GarageFlowService.API.Controllers;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GarageFlowService.Tests.API;

public class AuthControllerTests
{
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "GarageFlowServiceSecretKey2024!SuperSecure",
                ["Jwt:Issuer"] = "GarageFlowService",
                ["Jwt:Audience"] = "GarageFlowServiceClients",
                ["Auth:AdminUser"] = "admin",
                ["Auth:AdminPassword"] = "Admin@123"
            })
            .Build();

        var customerRepository = new Mock<ICustomerRepository>();
        customerRepository
            .Setup(repository => repository.GetByDocumentAsync("12345678909", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer("Cliente", "cliente@example.com", "11999999999", "12345678909"));

        _controller = new AuthController(configuration, customerRepository.Object);
    }

    [Fact]
    public async Task Login_WithCpfAndValidCustomer_ReturnsJwtToken()
    {
        var request = new LoginRequest("12345678909");

        var result = await _controller.Login(request);

        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().NotBeNull();
        var token = ok.Value!.GetType().GetProperty("token")?.GetValue(ok.Value)?.ToString();
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().StartWith("Bearer ");

        var jwt = token!.Replace("Bearer ", string.Empty);
        var handler = new JwtSecurityTokenHandler();
        handler.ReadJwtToken(jwt);
    }

    [Fact]
    public async Task Login_WithInvalidCpf_ReturnsUnauthorized()
    {
        var result = await _controller.Login(new LoginRequest("123"));

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
