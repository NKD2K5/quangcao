using System.Collections.Generic;

namespace quangcao.Models
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public List<TinTuc> News { get; set; } = new List<TinTuc>();
        public List<SanPham> Products { get; set; } = new List<SanPham>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public int TotalNewsCount { get; set; }
        public int TotalProductsCount { get; set; }
    }
}
