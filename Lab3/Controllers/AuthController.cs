using Lab3.Requests;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

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
}
