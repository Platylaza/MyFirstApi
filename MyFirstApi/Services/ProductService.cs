using MyFirstApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MyFirstApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IMemoryCache _cache;
        private readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
            new Product { Id = 2, Name = "Smartphone", Price = 699.99m, Category = "Electronics" },
            new Product { Id = 3, Name = "Headphones", Price = 199.99m, Category = "Accessories" }
        };

        private const string ProductsCacheKey = "products_list";

        public ProductService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Product> GetAll() {
            if (!_cache.TryGetValue(ProductsCacheKey, out IEnumerable<Product> cachedProducts))
        {
            cachedProducts = _products.ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _cache.Set(ProductsCacheKey, cachedProducts, cacheOptions);
        }

            return cachedProducts;
        }

        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public IEnumerable<Product> Search(string query) => _products.Where(p => p.Name.Contains(query));

        public Product Add(Product newProduct)
        {
            newProduct.Id = _products.Max(p => p.Id) + 1;
            _products.Add(newProduct);

            _cache.Remove(ProductsCacheKey);

            return newProduct;
        }

        public bool Update(int id, Product updateProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return false;
            }
            product.Name = updateProduct.Name;
            product.Category = updateProduct.Category;
            product.Price = updateProduct.Price;
            return true;
        }

        public bool Delete(int id)
        {
            var product = GetById(id);
            if (product == null) return false;

            _cache.Remove(ProductsCacheKey);
            return _products.Remove(product);
        }
    }
}