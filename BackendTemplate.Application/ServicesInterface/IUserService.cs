using BackendTemplate.Application.DTO;
using Microsoft.AspNetCore.Identity;

namespace BackendTemplate.Application.ServicesInterface
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto);

        Task<string?> LoginUserAsync(LoginUserDto loginUserDto);
    }
}
