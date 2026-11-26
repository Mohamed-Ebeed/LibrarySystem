using LibrarySystem.Domain;
using LibrarySystem.Dtos.Account;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService) : ControllerBase
    {
        /// <summary>
        /// Create a new account.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(CreateResponse(user));
        }

        /// <summary>
        /// Login and get the JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Unauthorized("Wrong email or password.");

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Wrong email or password.");

            return Ok(CreateResponse(user));
        }

        private AuthResponse CreateResponse(ApplicationUser user)
        {
            var (token, expiresAt) = jwtService.GenerateToken(user);
            return new AuthResponse
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}