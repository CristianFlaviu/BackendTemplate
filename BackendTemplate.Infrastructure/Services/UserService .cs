using BackendTemplate.Application.DTO;
using BackendTemplate.Application.ServicesInterface;
using BackendTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendTemplate.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IKeyVaultService _keyVaultService;

        public UserService(UserManager<UserEntity> userManager, IKeyVaultService keyVaultService)
        {
            _userManager = userManager;
            _keyVaultService = keyVaultService;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var user = new UserEntity
            {
                UserName = registerUserDto.UserName,
                Email = registerUserDto.Email,
                EmailConfirmed = true
            };

            return await _userManager.CreateAsync(user, registerUserDto.Password);
        }
        public async Task<string?> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginUserDto.Password))
                return null;

            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(UserEntity user)
        {
            var jwtSecret = await _keyVaultService.GetSecretAsync("JwtSecret");
            var jwtIssuer = await _keyVaultService.GetSecretAsync("JwtIssuer");
            var jwtAudience = await _keyVaultService.GetSecretAsync("JwtAudience");


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
