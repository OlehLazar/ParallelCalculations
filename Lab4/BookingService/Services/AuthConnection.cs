namespace BookingService.Services;
using System.Text.Json;

public class AuthConnection
{
	private readonly HttpClient _httpClient;

	public AuthConnection(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<string> ValidateTokenAsync(string token)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/validate");
		request.Headers.Add("Authorization", $"Bearer {token}");

		var response = await _httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var content = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<ValidateTokenResponse>(content);
		return result?.Role;
	}

	private class ValidateTokenResponse
	{
		public string Role { get; set; }
	}
}
