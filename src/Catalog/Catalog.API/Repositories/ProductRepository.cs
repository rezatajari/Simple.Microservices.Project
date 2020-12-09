using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// لیست کل محصولات
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context
                         .Products
                         .Find(filter:p => true)
                         .ToListAsync();
        }

        /// <summary>
        /// لیست محصول مورد نظر براساس آیدی دریافتی
        /// </summary>
        /// <param name="id">آیدی محصول خواسته شده</param>
        /// <returns></returns>
        public async Task<Product> GetProduct(string id)
        {
            return await _context
                         .Products
                         .Find(filter: p => p.Id == id)
                         .FirstOrDefaultAsync();

        }

        /// <summary>
        /// لیست محصول/محصولات براساس اسم
        /// </summary>
        /// <param name="name">اسم محصول درخواستی</param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {

            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(field: p => p.Name, name);

            return await _context
                         .Products
                         .Find(filter)
                         .ToListAsync();
        }

        /// <summary>
        /// لیست محصول/محصولات براساس دسته بندی
        /// </summary>
        /// <param name="categoryName">اسم دسته بندی درخواستی</param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {

            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(field: p => p.Category, category);

            return await _context
                         .Products
                         .Find(filter)
                         .ToListAsync();

        }

        /// <summary>
        /// ایجاد کردن محصول جدید
        /// </summary>
        /// <param name="product">مشخصات محصول جدید</param>
        /// <returns></returns>
        public async Task Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        /// <summary>
        /// بروزرسانی محصول مورد نظر
        /// </summary>
        /// <param name="product">محصول نیاز به بروزرسانی</param>
        /// <returns></returns>
        public async Task<bool> Update(Product product)
        {
            var updateResult = await _context
                                     .Products
                                     .ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);

            return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
        }

        /// <summary>
        /// حذف کردن محصول مورد نظر
        /// </summary>
        /// <param name="product">محصول نیاز به حذف</param>
        /// <returns></returns>
        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(field: p => p.Id, id);

            DeleteResult deleteResult = await _context
                                            .Products
                                            .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;

        }
    }
}
