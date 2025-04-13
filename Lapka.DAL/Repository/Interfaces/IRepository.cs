using System.Collections.Generic;

namespace Lapka.DAL.Repository.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        void Create(T item);
        T Get(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetPaginated(int page, int amountOnPage);
        void Update(T item);
        void Delete(int id);
    }
}

