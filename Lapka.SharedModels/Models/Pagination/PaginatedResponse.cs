﻿using System.Collections.Generic;

namespace Lapka.SharedModels.DTO.Pagination
{
    public class PaginatedResponse<T>
        where T : class
    {
        public List<T> Items { get; set; }
        public int PagesCount { get; set; }
        public int ItemsCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
