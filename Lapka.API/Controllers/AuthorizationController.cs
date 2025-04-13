using AutoMapper;
using Lapka.BLL.Infrastructure.Helpers.Auth;
using Lapka.DAL.DbContext;
using Lapka.DAL.Models;
using Lapka.SharedModels.Constants;
using Lapka.SharedModels.DTO.Auth;
using Lapka.SharedModels.Routes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SunApp.API.Controllers
{
    [ApiController]
    [Route(APIRoutes.V1.Auth.Base)]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LapkaDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthorizationController(
            UserManager<ApplicationUser> userManager,
            LapkaDbContext db,
            IConfiguration configuration)
        {
            _userManager = userManager;
            this._db = db;
            _configuration = configuration;
        }

        [HttpPost]
        [Route(APIRoutes.V1.Auth.Login)]
        [SwaggerOperation("General login endpoint. Returns roles user assigned to",
                Tags = new[] { "Authorization" })]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                if (user.BanDate != null)
                {
                    return BadRequest(new { Message = user.BanReason });
                }

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

                return Ok(new AuthenticatedUserResponseDTO()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    Roles = roles.ToList(),
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route(APIRoutes.V1.Auth.Register)]
        [SwaggerOperation("User registration",
            Tags = new[] { "Authorization" })]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO payload)
        {
            var userExists = await _userManager.FindByEmailAsync(payload.Email);

            if (userExists != null)
                return BadRequest("Користувач з такою поштою вже існує.");

            ApplicationUser user = new();

            user = Mapper.Map(payload, user);

            //{
            //    Birthday = payload.Birthday,
            //    Email = payload.Email,
            //    UserName = payload.Email,
            //    Name = payload.Name.Trim(),
            //    RegisterDate = DateTime.Now,
            //    LastActivityDate = DateTime.Now,
            //};

            user.Email = user.UserName = payload.Email;
            user.Name = payload.Name;
            user.RegisterDate = DateTime.Now;
            user.LastActivityDate = DateTime.Now;

            // Creating a user
            user.Id = Guid.NewGuid().ToString(); // Ensure the primary key is set
            var result = await _userManager.CreateAsync(user, payload.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            if (payload.Role.ToLower() == "shelter")
            {
                // Assigning to a role
                result = await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.SHELTER);

                // Creating a Shelter
                _db.Shelters.Add(new Lapka.API.DAL.Models.Shelter
                {
                    Phone = payload.Phone, 
                    Address = payload.Address,
                    AdditionDate = DateTime.Now,
                    Type = payload.ShelterType ?? Lapka.SharedModels.Base.ShelterType.Other,
                    User = user,
                });
                _db.SaveChanges();
            }
            else
                result = await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.USER);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            return Ok("Користувач створений успішно.");
        }

        //[HttpPost]
        //[Route(APIRoutes.V1.Auth.RestorePassword)]
        //[SwaggerOperation("Restore Password to account",
        //    Tags = new[] { "Authorization" })]
        //public async Task<IActionResult> RestorePassword(RestorePasswordDTO payload)
        //{
        //    var user = await _userManager.FindByEmailAsync(payload.Email);

        //    if (user != null)
        //    {
        //        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        token = HttpUtility.UrlEncode(token);

        //        var actionLink = $"{_configuration["FE:BasePath"]}/auth/reset?userId={user.Id}&token={token}";
        //        var emailData = EmailDataDTO.CreateForForgotPassword(payload.Email, actionLink);

        //        var result = await _emailSenderService.SendEmailAsync(emailData);
        //    }
        //    return Ok();
        //}

        //[HttpPost]
        //[Route(APIRoutes.V1.Auth.ResetPassword)]
        //[SwaggerOperation("Reset Password to account",
        //    Tags = new[] { "Authorization" })]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDTO payload)
        //{
        //    var user = await _userManager.FindByIdAsync(payload.UserId);
        //    if (user == null)
        //        return BadRequest("");

        //    var result = await _userManager.ResetPasswordAsync(user, payload.Token, payload.Password);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors.FirstOrDefault().Description);
        //    }

        //    EmailDataDTO emailData = EmailDataDTO.CreateForResetPassword(user.Email);
        //    await _emailSenderService.SendEmailAsync(emailData);

        //    return Ok("Password changed");
        //}
    }

}

