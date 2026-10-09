


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigitalMenu.Services;

public class AuthService : IAuthService
{
    
    
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        
        
        
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email);


        if (user == null)
        {
            
            return null;
        }
        
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        
        
        if (!isPasswordValid) return null;

        var token=GenerateJwtToken(user);
        
        return new LoginResponseDto(
            token,
            user.Email,
            user.Role ?? "RestoranAdmin",
            user.RestaurantId ?? null
                );




    }

    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Jwt:Secret"]!;
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "RestoranAdmin"),
            
        };
        
        if (user.RestaurantId.HasValue)
        {
            claims.Add(new Claim("restaurant_id", user.RestaurantId.Value.ToString()));
        }

        

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



