using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GarageFlowService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GarageFlowService.API.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICustomerRepository _customerRepository;

    public AuthController(IConfiguration configuration, ICustomerRepository customerRepository)
    {
        _configuration = configuration;
        _customerRepository = customerRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var cpf = NormalizeCpf(request.Cpf);

        if (!IsValidCpf(cpf))
            return Unauthorized(new { error = "CPF inválido." });

        var customer = await _customerRepository.GetByDocumentAsync(cpf, cancellationToken);

        if (customer is null)
            return Unauthorized(new { error = "Cliente não encontrado." });

        if (!customer.IsActive)
            return Unauthorized(new { error = "Cliente inativo." });

        var token = GenerateJwtToken(customer.Name, customer.Document, customer.Id.ToString(), "Customer");
        return Ok(new
        {
            token = $"Bearer {token}",
            expiresAt = DateTime.UtcNow.AddHours(8),
            customer = new { customer.Id, customer.Name, customer.Document }
        });
    }

    private static string NormalizeCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return string.Empty;

        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    private static bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        var digits = cpf.Select(c => c - '0').ToArray();

        var sum1 = 0;
        for (var i = 0; i < 9; i++)
            sum1 += digits[i] * (10 - i);

        var mod1 = sum1 % 11;
        var digit1 = mod1 < 2 ? 0 : 11 - mod1;

        if (digits[9] != digit1)
            return false;

        var sum2 = 0;
        for (var i = 0; i < 10; i++)
            sum2 += digits[i] * (11 - i);

        var mod2 = sum2 % 11;
        var digit2 = mod2 < 2 ? 0 : 11 - mod2;

        return digits[10] == digit2;
    }

    private string GenerateJwtToken(string name, string document, string subject, string role)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, name),
            new Claim("document", document),
            new Claim(ClaimTypes.NameIdentifier, subject),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Sub, subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Cpf);

