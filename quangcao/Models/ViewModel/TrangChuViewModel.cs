namespace quangcao.Models.ViewModel
{
    public class TrangChuViewModel
    {
        public AnhBiaTrang AnhBia { get; set; }
        public GioiThieu GioiThieu { get; set; }
        public List<ThanhVienDoiNgu> DanhSachNhanVien { get; set; }
        public List<TinTuc> LatestNews { get; set; } = new List<TinTuc>();
    }

}
