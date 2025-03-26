using Lab3.Context;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab3.Services;

public class AuthService
{
	private readonly ApplicationDbContext _context;
	private readonly string _jwtKey = "your-256-bit-secret-key-must-be-long-enough";

	public AuthService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<string> AuthenticateAsync(string username, string password)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
		if (user == null || user.PasswordHash != password) return null;

		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_jwtKey);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[] {
					new Claim(ClaimTypes.Name, user.Username),
					new Claim(ClaimTypes.Role, user.Role)
				}),
			Expires = DateTime.UtcNow.AddHours(2),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public async Task<bool> RegisterUserAsync(string username, string password, string role)
	{
		if (await _context.Users.AnyAsync(u => u.Username == username)) return false;

		_context.Users.Add(new User { Username = username, PasswordHash = password, Role = role });
		await _context.SaveChangesAsync();
		return true;
	}
}
