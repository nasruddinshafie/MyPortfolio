using Application.Common.Interfaces;
using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUserByEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        var existingUserByUsername = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (existingUserByUsername != null)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // Create new user
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Generate token
        var token = _tokenGenerator.GenerateToken(createdUser, out var expiresAt);

        return new AuthResponseDto
        {
            Token = token,
            Username = createdUser.Username,
            Email = createdUser.Email,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Generate token
        var token = _tokenGenerator.GenerateToken(user, out var expiresAt);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = expiresAt
        };
    }
}
