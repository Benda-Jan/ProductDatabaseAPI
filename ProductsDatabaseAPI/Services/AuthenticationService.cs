using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductDatabaseAPI.Configurations;
using ProductDatabaseAPI.Controllers;
using ProductDatabaseAPI.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductDatabaseAPI.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthManagementController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _jwtConfig;

    public AuthenticationService(
        ILogger<AuthManagementController> logger,
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> optionsMonitor)
    {
        _logger = logger;
        _userManager = userManager;
        _jwtConfig = optionsMonitor.CurrentValue;
    }

    public async Task<string> Register(UserRegistrationRequestDto requestDto)
    {
        var alreadyExists = await _userManager.FindByEmailAsync(requestDto.Email);
        if (alreadyExists != null)
            throw new BadHttpRequestException("Email already exists");

        var newUser = new IdentityUser()
        {
            Email = requestDto.Email,
            UserName = requestDto.Name
        };

        var result = await _userManager.CreateAsync(newUser, requestDto.Password);
        if (!result.Succeeded)
            throw new BadHttpRequestException("Error creating the user, please try again later");

        var token = GenerateJwtToken(newUser) ??
            throw new BadHttpRequestException("Error creating the user, please try again later");

        return token;
    }

    public async Task<string> Login(UserLoginRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email)
            ?? throw new BadHttpRequestException("Invalid authentication");

        var valid = await _userManager.CheckPasswordAsync(user, requestDto.Password);
        if (!valid)
            throw new BadHttpRequestException("IncorrectPassword");

        var token = GenerateJwtToken(user) ??
                throw new BadHttpRequestException("Error logging the user, please try again later");

        return token;
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        if (user is null || user.Email is null)
            return string.Empty;

        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.Now.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }
}

