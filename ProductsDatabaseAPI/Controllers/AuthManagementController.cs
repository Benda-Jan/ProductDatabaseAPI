using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductDatabaseAPI.Configurations;
using ProductDatabaseAPI.Services;
using ProductDatabaseAPI.Dtos;

namespace ProductDatabaseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthManagementController : Controller
{
    private readonly AuthenticationService _service;

    public AuthManagementController(AuthenticationService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register (UserRegistrationRequestDto requestDto)
    {
        if (ModelState.IsValid)
        {
            var token = String.Empty;
            try
            {
                token = await _service.Register(requestDto);
            }
            catch(BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new RegisterRequestResponse()
            {
                Result = true,
                Token = token
            });
        }

        return BadRequest("Invalid request payload");
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(UserLoginRequestDto requestDto)
    {
        if (ModelState.IsValid)
        {
            var token = string.Empty;
            try
            {
                token = await _service.Login(requestDto);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new LoginRequestResponse()
                {
                    Result = true,
                    Token = token
                });
        }
        return BadRequest("Invalid request payload");
    }

}

