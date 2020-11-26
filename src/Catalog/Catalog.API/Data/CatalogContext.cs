using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Settings;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {

        public CatalogContext(ICatalogDatabaseSettings settings)
        {
            // اتصال برقرار کردن به دیتابیس مونگو
            var client = new MongoClient("mongodb://username:password@ds011111.mongolab.com:11111/db-name");

            // انتخاب دیتابیس مونگو برای کاتالوگ
            var database = client.GetDatabase(settings.DatabaseName);

            // ست کردن کالکشن پروداکت از داخل دیتابیس مونگو
            Products = database.GetCollection<Product>(settings.CollectionName);

            // فرستادن اطلاعات کالکشن برای بررسی داده های آن
            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}
