using AutoMapper;
using Lapka.BLL.Helpers;
using Lapka.DAL.DbContext;
using Lapka.DAL.Models;
using Lapka.SharedModels.Constants;
using Lapka.SharedModels.DTO.Filters.Users;
using Lapka.SharedModels.DTO.Pagination;
using Lapka.SharedModels.DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lapka.BLL.Services
{
    public class UserService : BaseService
    {
        private readonly IConfiguration _configuration;
        private readonly LapkaDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            IConfiguration configuration,
            IServiceScopeFactory factory,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _db = factory.CreateScope().ServiceProvider
                .GetRequiredService<LapkaDbContext>();
            _userManager = userManager;
        }

        #region Users

        public ApplicationUser GetUser(string id) => _db.Users.Where(t => t.Id == id).FirstOrDefault();
        public ApplicationUser GetUserByEmail(string email) => _db.Users.Where(t => t.Email == email).FirstOrDefault();

        public PaginatedResponse<UserShortInfoDTO> GetFilteredUsers(
            UsersFilterModel filter,
            bool isAdmin)
        {
            var query = _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role).AsQueryable();

            int elementsCount = query.Count();

            if (filter.IsPaginationEnabled)
                query = query.Skip(filter.Skip).Take(filter.Take);

            var result = Mapper.Map<List<UserShortInfoDTO>>(query.ToList());

            return
                new PaginatedResponse<UserShortInfoDTO>()
                {
                    Items = result,
                    CurrentPage = filter.Page ?? 0,
                    PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, elementsCount),
                };
        }

        public UserShortInfoDTO GetUserInfo(string id)
        {
            var user = _db.Users.Find(id);

            _db.Entry(user)
                .Collection(u => u.UserRoles)
                .Query()
                .Include(ur => ur.Role)
                .Load();


            return Mapper.Map<UserShortInfoDTO>(user);
        }

        public async Task UpdateLastActivity(string userId, string ipAddress)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastActivityDate = DateTime.Now;

                await _db.SaveChangesAsync();
            }
        }

        public async Task<IdentityResult> DeleteUser(ApplicationUser user)
        {
            if (user != null)
            {
                user.IsDeleted = true;

                var result = await _userManager.UpdateAsync(user);

                return result;
            }

            return new IdentityResult();
        }

        #endregion

        #region Ban user

        public async Task BanUser(string id, string reason)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            await BanUser(user, reason);
        }

        public async Task BanUser(ApplicationUser user, string reason)
        {
            var roles = await _userManager.GetRolesAsync(user);

            user.BanDate = System.DateTime.UtcNow;
            user.BanReason = reason;
            user.RoleBeforeBan = roles.FirstOrDefault();

            using (var transaction = _db.Database.BeginTransaction())
            {
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await _userManager.RemoveFromRolesAsync(user, roles);
                //await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
                await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.BANNED_USER);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task UnbanUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            await UnbanUser(user);
        }

        public async Task UnbanUser(ApplicationUser user)
        {
            var roleBeforeBan = user.RoleBeforeBan;

            user.BanDate = null;
            user.BanReason = null;
            user.RoleBeforeBan = null;

            using (var transaction = _db.Database.BeginTransaction())
            {
                await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
                await _userManager.AddToRoleAsync(user, roleBeforeBan);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Settings

        //public async Task ChangePassword(ApplicationUser user, UserPasswordUpdateDTO payload)
        //{
        //    if (user == null)
        //    {
        //        throw new Exception("User not found.");
        //    }

        //    var result = await _userManager.ChangePasswordAsync(
        //        user,
        //        payload.OldPassword,
        //        payload.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        throw new Exception(result.Errors.First().Description);
        //    }

        //    EmailDataDTO emailData = EmailDataDTO.CreateForResetPassword(user.Email);
        //    await _emailSender.SendEmailAsync(emailData);
        //}

        #endregion
    }
}
