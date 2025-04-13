using Lapka.DAL.DbContext;
using Lapka.DAL.Models;
using Lapka.DAL.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lapka.DAL.Repository
{
    public class UsersRepository : IRepository<ApplicationUser>
    {
        private LapkaDbContext _db = null;
        public UsersRepository(
            IServiceScopeFactory factory)
        {
            _db = factory.CreateScope().ServiceProvider.GetRequiredService<LapkaDbContext>();
        }

        public void Create(ApplicationUser item)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _db.Users;
        }

        public IEnumerable<ApplicationUser> GetPaginated(int page, int amountOnPage)
        {
            return GetAll().Skip(page).Take(amountOnPage);
        }

        public void Update(ApplicationUser item)
        {

        }
    }
}
