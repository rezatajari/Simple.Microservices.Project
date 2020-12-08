using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// گرفتن سبد براساس نام کاربر
        /// </summary>
        /// <param name="userName">نام کاربر</param>
        /// <returns></returns>
        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _context
                        .Redis
                        .StringGetAsync(userName);

            if (basket.IsNullOrEmpty)
                return null;

            // پاسخ درخواستی که از طرف دیتابیس ردیس به ما می آید به صورت جی سان است و از این طریق آن را به آبجکت مدل خودمان تبدیل می کنیم
            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        /// <summary>
        /// بروزرسانی سبد کاربر بوسیله مشخصات کارت سبد
        /// </summary>
        /// <param name="basket">مشخصات کارت سبد</param>
        /// <returns></returns>
        public async Task<BasketCart> UpdateBasket(BasketCart basket)
        {
            bool updated = await _context
                                .Redis
                                .StringSetAsync(basket.UserName,JsonConvert.SerializeObject(basket));

            if (!updated)
                return null;

            return await GetBasket(basket.UserName);
        }

        /// <summary>
        /// حذف کردن سبد بوسیله نام کاربر
        /// </summary>
        /// <param name="userName">نام کاربر</param>
        /// <returns></returns>
        public async Task<bool> DeleteBasket(string userName)
        {
            return await _context
                    .Redis
                    .KeyDeleteAsync(userName);
        }
    }
}
