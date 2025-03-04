using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BonusSystem.Infrastructure.Auth;

/// <summary>
/// JWT-based authentication service
/// </summary>
public class JwtAuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbOptions _options;
    private readonly ILogger<JwtAuthenticationService> _logger;

    public JwtAuthenticationService(
        IUserRepository userRepository,
        IOptions<AppDbOptions> options,
        ILogger<JwtAuthenticationService> logger)
    {
        _userRepository = userRepository;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AuthResult> SignInAsync(UserLoginDto loginDto)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            // For prototype, we're not checking passwords
            // In a real app, we would verify hashed password here

            // Generate token
            var token = await GenerateTokenAsync(user.Id, user.Role);

            return new AuthResult
            {
                Success = true,
                UserId = user.Id,
                Token = token,
                Role = user.Role
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign in for {Email}", loginDto.Email);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "An error occurred during sign in"
            };
        }
    }

    public async Task<AuthResult> SignUpAsync(UserRegistrationDto registrationDto)
    {
        try
        {
            // Check if user already exists
            var exists = await _userRepository.IsUserExistsByEmailAsync(registrationDto.Email);
            if (exists)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "User with this email already exists"
                };
            }

            // Create user
            var userId = Guid.NewGuid();
            var user = new UserDto
            {
                Id = userId,
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                Role = registrationDto.Role,
                BonusBalance = 0
            };

            await _userRepository.CreateAsync(user);

            // Generate token
            var token = await GenerateTokenAsync(userId, registrationDto.Role);

            return new AuthResult
            {
                Success = true,
                UserId = userId,
                Token = token,
                Role = registrationDto.Role
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign up for {Email}", registrationDto.Email);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "An error occurred during sign up"
            };
        }
    }

    public Task<bool> SignOutAsync(string token)
    {
        // For JWT, client-side is responsible for token disposal
        // Server-side could implement token blacklisting if needed
        return Task.FromResult(true);
    }

    public async Task<UserRole> GetUserRoleAsync(Guid userId)
    {
        try
        {
            return await _userRepository.GetUserRoleAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role for user {UserId}", userId);
            return UserRole.Buyer; // Default role
        }
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.JwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<string> GenerateTokenAsync(Guid userId, UserRole role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.JwtSecret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.TokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public Task<Guid?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.JwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Task.FromResult<Guid?>(Guid.Parse(userId));
        }
        catch
        {
            return Task.FromResult<Guid?>(null);
        }
    }
}