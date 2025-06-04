using AuthenticationService.Context;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Services;

public class AuthService
{
	private readonly IDistributedCache _cache;
	private readonly ApplicationDbContext _context;
	private readonly string _jwtKey = "your-256-bit-secret-key-must-be-long-enough";

	public AuthService(IDistributedCache cache, ApplicationDbContext context)
	{
		_cache = cache;
		_context = context;
	}

	public async Task<string> AuthenticateAsync(string username, string password)
	{
		var cacheKey = $"auth_token_{username}";

		// Спробуємо знайти токен у кеші
		var cachedToken = await _cache.GetStringAsync(cacheKey);
		if (!string.IsNullOrEmpty(cachedToken))
		{
			Console.WriteLine($"Token loaded from cache for user: {username}");
			return cachedToken;
		}

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
		var tokenString = tokenHandler.WriteToken(token);

		// Зберігаємо токен у кеш на 2 години
		await _cache.SetStringAsync(cacheKey, tokenString, new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
		});

		Console.WriteLine($"Token cached for user: {username}");

		return tokenString;
	}

	public async Task<bool> RegisterUserAsync(string username, string password, string role)
	{
		if (await _context.Users.AnyAsync(u => u.Username == username)) return false;

		_context.Users.Add(new User { Username = username, PasswordHash = password, Role = role });
		await _context.SaveChangesAsync();
		return true;
	}
}
