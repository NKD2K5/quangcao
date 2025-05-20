namespace quangcao.Models.ViewModel
{
    public class sanphamChiTietViewModel
    {
        public SanPham SanPham { get; set; }
        public List<DanhGia> TatCaDanhGia { get; set; }
        public List<DanhGia> DanhGiaHienTai { get; set; }
        public List<TinTuc> TinTucs { get; set; }
    }

}
