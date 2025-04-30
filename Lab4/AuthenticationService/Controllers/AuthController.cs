using AuthenticationService.Services;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = AuthenticationService.Requests.RegisterRequest;
using LoginRequest = AuthenticationService.Requests.LoginRequest;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthenticationService.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly AuthService _authService;

	public AuthController(AuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request)
	{
		var success = await _authService.RegisterUserAsync(request.Username, request.Password, request.Role);
		if (!success) return BadRequest("User already exists.");
		return Ok("User registered successfully.");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		var token = await _authService.AuthenticateAsync(request.Username, request.Password);
		if (token == null) return Unauthorized("Invalid credentials");
		return Ok(new { Token = token });
	}

	[HttpGet("validate")]
	[Authorize]
	public IActionResult ValidateToken()
	{
		var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
		return Ok(new { role });
	}
}
