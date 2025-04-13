using Newtonsoft.Json;

namespace Lapka.SharedModels.DTO.Filters.Base
{
    public enum Order { ASC, DESC }

    public class BaseFilterModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public Order OrderBy { get; set; } = Order.DESC;
        public string OrderByField { get; set; }
        public string Except { get; set; }

        public int? Page { get; set; } = 1;
        public int? AmountOnPage { get; set; } = 15;

        [JsonIgnore]
        public bool IsPaginationEnabled => (Page != null && AmountOnPage != null);
        [JsonIgnore]
        public int Take => (AmountOnPage <= 0 ? 1 : AmountOnPage) ?? int.MaxValue;
        [JsonIgnore]
        public int Skip =>
            IsPaginationEnabled ?
                (
                    Page == 1 ? 0 : ((Page - 1) * AmountOnPage ?? 0)
                ) : 0;
    }
}

