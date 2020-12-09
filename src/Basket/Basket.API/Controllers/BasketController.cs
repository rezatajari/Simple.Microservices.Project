using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route(template: "api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {

        private readonly IBasketRepository _repository;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _eventBus;

        public BasketController(IBasketRepository repository, IMapper mapper,EventBusRabbitMQProducer eventBus)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// گرفتن مشخصات سبد کاربر براساس نام کاربر
        /// </summary>
        /// <param name="userName">نام کاربر</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new BasketCart(userName));
        }

        /// <summary>
        /// بروزرسانی یا ساخت سبد جدید
        /// </summary>
        /// <param name="basket">مشخصات سبد برای بروزرسانی یا ایجاد شدن</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basket)
        {
            return Ok(await _repository.UpdateBasket(basket));
        }

        /// <summary>
        /// حذف کردن یک سبد بوسیله نام کاربر مورد نظر به آن سبد
        /// </summary>
        /// <param name="userName">نام کاربر</param>
        /// <returns></returns>
        [HttpDelete(template: "{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            return Ok(await _repository.DeleteBasket(userName));
        }

        /// <summary>
        /// عملیات ارسال رویداد حساب کردن سبد به ربیت ام کیو
        /// </summary>
        /// <param name="basketChekout">مدل سبد</param>
        /// <returns></returns>
        [Route(template: "[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketChekout basketChekout)
        {
            // گرفتن سبد
            BasketCart basket = await _repository.GetBasket(basketChekout.UserName);
            if (basket == null)
                return BadRequest();

            // حذف سبد 
            bool basketRemoved = await _repository.DeleteBasket(basket.UserName);
            if (!basketRemoved)
                return BadRequest();

            // ارسال سبد به ربیت ام کیو
             BasketCheckoutEvent eventMessage = _mapper.Map<BasketCheckoutEvent>(basketChekout);
             eventMessage.RequestId = Guid.NewGuid();
             eventMessage.TotalPrice = basket.TotalPrice;

             try
             {
                 _eventBus.PublishBasketCheckout(EvenBusConstants.BasketCheckoutQueue, eventMessage);
             }
             catch (Exception e)
             {
                 throw;
             }

             return Accepted();
        }
    }
}
