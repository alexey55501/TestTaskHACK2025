using Lapka.SharedModels.Base;
using Lapka.SharedModels.DTO.Filters.Base;

namespace Lapka.SharedModels.DTO.Filters
{
    public class AnimalsFilterDTO : BaseFilterModel
    {
        public bool FavoritesOnly { get; set; }
        public AnimalType Type { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public string Location { get; set; }
        public bool? Sex { get; set; }
        public bool? IsSterilized { get; set; }
        public bool? HasPassport { get; set; }
    }
}
