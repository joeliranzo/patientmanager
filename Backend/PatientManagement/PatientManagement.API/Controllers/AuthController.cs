using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.DTOs.User;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService
    ) : ControllerBase
{
    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await authService.LoginAsync(dto);
        return result is null ? Unauthorized("Invalid credentials") : Ok(result);
    }

    // POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await authService.RegisterAsync(dto);
        return success ? Ok("User registered.") : Conflict("Email already exists.");
    }
}
