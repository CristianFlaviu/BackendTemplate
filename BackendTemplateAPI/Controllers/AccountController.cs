using BackendTemplate.Application.DTO;
using BackendTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendTemplate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public AccountController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new UserEntity
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                // Create the user
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    // Optionally, sign the user in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Ok(new { Message = "User registered successfully" });
                }

                // Return the errors if creation failed
                return BadRequest(result.Errors);
            }

            // Return validation errors if model is invalid
            return BadRequest(ModelState);
        }
    }
}
