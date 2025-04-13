using AutoMapper;
using Lapka.BLL.Services;
using Lapka.DAL.Models;
using Lapka.SharedModels.Constants;
using Lapka.SharedModels.DTO.Filters.Users;
using Lapka.SharedModels.DTO.User;
using Lapka.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using static Lapka.SharedModels.Constants.GlobalConstants;

namespace Lapka.API.Controllers
{
    /// <summary>
    /// Admin controller for managing application
    /// </summary>

    [ApiController]
    [Route(APIRoutes.V1.Admin.Base)]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            UserService userService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        [Route(APIRoutes.V1.Admin.GetUsers)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Get paginated users")]
        public async Task<IActionResult> GetUsers(
            [FromBody] UsersFilterModel filter)
        {
            var users = _userService.GetFilteredUsers(filter, this.User.IsInRole(GlobalConstants.Roles.ADMIN));

            if (users == null)
                return BadRequest();
            else
                return Ok(users);
        }

        [HttpGet]
        [Route(APIRoutes.V1.Admin.GetUserInfo)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Get user's info")]
        public async Task<IActionResult> GetUserInfo(string id)
        {
            var userInfo = _userService.GetUserInfo(id);

            if (userInfo == null) return BadRequest();


            return Ok(userInfo);
        }

        [HttpPut]
        [Route(APIRoutes.V1.Admin.UpdateUser)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Update user's info")]
        public async Task<IActionResult> UpdateUser(
            string id,
            [FromBody] UserDTO dto)
        {
            var requester = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user = Mapper.Map(dto, user);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        [HttpDelete]
        [Route(APIRoutes.V1.Admin.DeleteUser)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Delete user")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var requester = await _userManager.GetUserAsync(User);
            var isModerator = await _userManager.IsInRoleAsync(requester, Roles.SHELTER);

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userService.DeleteUser(user);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        [HttpPatch]
        [Route(APIRoutes.V1.Admin.BanUser)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Ban user")]
        public async Task<IActionResult> BanUser(string id, [FromBody] string reason)
        {
            var requester = await _userManager.GetUserAsync(User);
            var isModerator = await _userManager.IsInRoleAsync(requester, Roles.SHELTER);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest(new { Message = "User not found." });

            var roles = await _userManager.GetRolesAsync(user);

            user.BanDate = System.DateTime.Now;
            user.BanReason = reason;
            user.RoleBeforeBan = roles?[0];

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result);

            await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
            await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.BANNED_USER);

            return Ok(new { BanDate = user.BanDate });
        }

        [HttpPatch]
        [Route(APIRoutes.V1.Admin.UnbanUser)]
        [Authorize(Roles = GlobalConstants.Roles.ADMIN, AuthenticationSchemes = "Bearer")]
        [SwaggerOperation("Unban user")]
        public async Task<IActionResult> UnbanUser(string id)
        {
            var requester = await _userManager.GetUserAsync(User);
            var isModerator = await _userManager.IsInRoleAsync(requester, Roles.SHELTER);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest(new { Message = "User not found." });

            await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
            await _userManager.AddToRoleAsync(user, user.RoleBeforeBan);

            user.BanDate = null;
            user.BanReason = null;
            user.RoleBeforeBan = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result);


            return Ok(result);
        }
    }
}

