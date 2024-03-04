using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PrjNullCore.AppDbContext;
using PrjNullCore.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace PrjNullCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _ApplicationDbContext;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, ApplicationDbContext aPplicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _ApplicationDbContext = aPplicationDbContext;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            // Implement your login logic here


            await _signInManager.SignOutAsync();
            await _signInManager.ForgetTwoFactorClientAsync();
            await HttpContext.SignOutAsync();

            HttpContext.User =
                new GenericPrincipal(new GenericIdentity(string.Empty), null);


            var isAuth1 = User.Identity.IsAuthenticated;
            
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            var isAuth = User.Identity.IsAuthenticated;
            if (result != null && result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                // Create claims for the authenticated user
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            // Add more claims as needed, such as roles or permissions
        };

                var claimsIdentity = new ClaimsIdentity(claims, "Token");

                // Generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
                    Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Return token and user data
                return Ok(new
                {
                    Token = tokenHandler.WriteToken(token),
                    UserId = user.Id,
                    Username = user.Email
                    // Include any other user data you want to return
                });
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            // Implement your registration logic here
            // Example:
            var usr = new ApplicationUser() { Email = model.Email, FirstName = model.FirstName, UserName = model.Email };
            //var result = await _userService.RegisterAsync(model);
            var result = await _userManager.CreateAsync(usr, model.Password);
            if (result.Succeeded)
            {
                return Ok(result.Succeeded.ToString());
            }
            return BadRequest(result.Errors.ToString());
        }
    }
}