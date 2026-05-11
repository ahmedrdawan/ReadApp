using System.Collections.Generic;

namespace MyReadsApp.Core.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }
}
