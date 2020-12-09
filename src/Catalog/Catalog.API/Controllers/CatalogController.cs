using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route(template: "api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {

        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// گرفتن لیست کل محصولات
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            IEnumerable<Product> products = await _repository.GetProducts();
            return Ok(products);
        }

        /// <summary>
        /// گرفتن لیست محصول براساس آیدی آن محصول
        /// </summary>
        /// <param name="id">آیدی محصول درخواستی</param>
        /// <returns></returns>
        [HttpGet(template: "{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);

            if (product == null)
            {
                _logger.LogError(message: $"Product with id: {id}, not found.");
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// گرفتن محصول براساس دسته بندی مورد نظر
        /// </summary>
        /// <param name="category">دسته بندی مورد نظر</param>
        /// <returns></returns>
        [Route(template: "[action]/{category}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {

            IEnumerable<Product> product = await _repository.GetProductByCategory(category);
            return Ok(product);

        }

        /// <summary>
        /// ساختن یک محصول جدید
        /// </summary>
        /// <param name="product">مشخصات محصول جدید را بفرستید</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.Create(product);

            return CreatedAtRoute(routeName: "GetProduct", routeValues: new { id = product.Id }, value: product);
        }

        /// <summary>
        /// بروزرسانی محصول مورد نظر
        /// </summary>
        /// <param name="value">مشخصات محصول مورد نظر برای بروزرسانی</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product value)
        {
            return Ok(await _repository.Update(value));
        }

        /// <summary>
        /// حذف کردم محصول مورد نظر براساس آیدی آن محصول
        /// </summary>
        /// <param name="id">آیدی محصول برای حذف آن</param>
        /// <returns></returns>
        [HttpDelete(template: "{id:length(24)}")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.Delete(id));
        }

    }
}
