namespace quangcao.Models
{
    public interface IProductService
    {
        SanPham GetProductById(Guid productId);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public SanPham GetProductById(Guid productId)
        {
            return _context.SanPhams.FirstOrDefault(p => p.IdSanPham == productId);
        }
    }

}
