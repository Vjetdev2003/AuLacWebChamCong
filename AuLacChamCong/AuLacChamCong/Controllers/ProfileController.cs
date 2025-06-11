using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.Modules.Profile.Model;
using AuLacChamCong.Services.Modules.Profile.Service;
using AuLacChamCong.Services.Modules.User.Model;
using AuLacChamCong.Services.Modules.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly Login_Services _loginService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(IUserProfileService userProfileService, Login_Services loginServices, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userProfileService = userProfileService;
            _loginService = loginServices;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet("{psnPrkID}")]
        public async Task<ActionResult<Personnels>> GetUserProfile(string psnPrkID)
        {
            if (string.IsNullOrEmpty(psnPrkID))
            {
                return BadRequest(new { message = "PsnPrkID is required" });
            }

            // Call the service method to get user profile
            var user = await _userProfileService.GetUserProfileAsync(psnPrkID);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Return the user profile data
            return Ok(user);
        }
    }
}
