using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.DTOs;
using AutoMapper;
using MyFirstApi.Services;
using MyFirstApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using MyFirstApi.Auth;

namespace MyFirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly StoreSettings _storeSettings;

        public ProductsController(
            IProductService productService,
            IMapper mapper,
            IOptions<StoreSettings> options)
        {
            _productService = productService;
            _mapper = mapper;
            _storeSettings = options.Value;
        }

        /// <summary>
        /// Returns store-level configuration information.
        /// </summary>
        [HttpGet("store-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetStoreInfo()
        {
            return Ok(new
            {
                name = _storeSettings.StoreName,
                currency = _storeSettings.DefaultCurrency,
                discountsEnabled = _storeSettings.EnableDiscounts
            });
        }

        /// <summary>
        /// Returns all products.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadDto>))]
        public ActionResult<IEnumerable<ProductReadDto>> GetAll() =>
            Ok(_mapper.Map<IEnumerable<ProductReadDto>>(_productService.GetAll()));

        /// <summary>
        /// Searches products by name.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadDto>))]
        public ActionResult<IEnumerable<ProductReadDto>> Search([FromQuery] string query) =>
            Ok(_mapper.Map<IEnumerable<ProductReadDto>>(_productService.Search(query)));

        /// <summary>
        /// Returns a product by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductReadDto> GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return _mapper.Map<ProductReadDto>(product);
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ProductReadDto> Add([FromBody] ProductCreateDto newProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(newProductDto);
            var created = _productService.Add(product);

            ProductReadDto readDto = _mapper.Map<ProductReadDto>(product);

            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] ProductCreateDto updatedProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_productService.Update(id, _mapper.Map<Product>(updatedProductDto)))
            {
                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete(int id)
        {
            if (!_productService.Delete(id))
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
        [HttpGet("secure")]
        public IActionResult GetSecureInfo()
        {
            return Ok(new { message = "You reached a protected endpoint." });
        }
    }
}