using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SmartRecruiter.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        
    }
    
}