namespace Portfolio.UI.DTOs.Auth;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
}
