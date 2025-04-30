namespace AuthenticationService.Requests;

public record RegisterRequest(string Username, string Password, string Role);
