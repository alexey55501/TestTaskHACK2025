using Lapka.BLL.Infrastructure.Helpers.Auth;
using Lapka.BLL.Services;
using Lapka.DAL.Models;
using Lapka.DAL.Repository;
using Lapka.SharedModels.Constants;
using Lapka.SharedModels.DTO.Auth;
using Lapka.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lapka.API.Controllers
{
    /// <summary>
    /// Managing users account
    /// Editing basic user stuff
    /// Adding, updating user's data etc
    /// </summary>

    [Authorize(Roles =
            GlobalConstants.Roles.USER + "," +
            GlobalConstants.Roles.ADMIN + "," +
            GlobalConstants.Roles.SHELTER,
        AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route(APIRoutes.V1.User.Base)]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UsersRepository _usersRepo;
        private readonly UserService _userService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            UserService userService,
            UsersRepository usersRepository,
            IServiceScopeFactory factory,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userService = userService;
            _usersRepo = usersRepository;
            _configuration = configuration;
        }

        #region User Management

        [HttpGet]
        [Route(APIRoutes.V1.User.GetInfo)]
        [SwaggerOperation("Returns info about current user")]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return Ok(
                    new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        Roles = await _userManager.GetRolesAsync(user),
                        ShelterId = user?.Shelter?.Id ?? 0,
                    }
                );
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        [HttpGet]
        [Route(APIRoutes.V1.User.GetId)]
        [SwaggerOperation("Returns current user Id")]
        public async Task<IActionResult> GetId()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return Ok(user.Id);
            }
            else
            {
                return BadRequest();
            }
        }


        //[HttpPut]
        //[Route(APIRoutes.V1.User.Update)]
        //[SwaggerOperation("Update current user")]
        //public async Task<IActionResult> Update(
        //    [FromBody] UserDTO payload)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    if (user != null)
        //    {
        //        Mapper.Map(payload, user);
        //        user.Email = user.UserName = payload.Email;

        //        var result = await _userManager.UpdateAsync(user);

        //        if (result.Succeeded)
        //        {
        //            var responseDTO = await GenerateToken(user);
        //            return Ok(responseDTO);
        //        }
        //        else return BadRequest(result);
        //    }
        //    else
        //    {
        //        return BadRequest(new { Message = "User not found." });
        //    }
        //}

        //[HttpPut]
        //[Route(APIRoutes.V1.User.UpdatePassword)]
        //[SwaggerOperation("Update user's password")]
        //public async Task<IActionResult> UpdatePassword(
        //    [FromBody] UserPasswordUpdateDTO payload)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    try
        //    {
        //        await _userService.ChangePassword(user, payload);
        //        var responseDTO = await GenerateToken(user);
        //        return Ok(responseDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}


        private async Task<AuthenticatedUserResponseDTO> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.Join(", ", roles)),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            JwtSecurityToken token = null;

            token = JWT.GenerateToken(
                _configuration, authClaims);

            return new AuthenticatedUserResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Roles = roles.ToList(),
            };
        }

        #endregion
    }
}

