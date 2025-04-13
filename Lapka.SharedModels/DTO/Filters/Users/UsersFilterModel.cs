using Lapka.SharedModels.DTO.Filters.Base;

namespace Lapka.SharedModels.DTO.Filters.Users
{
    public class UsersFilterModel : BaseFilterModel
    {
        public string Role { get; set; }
    }
}

