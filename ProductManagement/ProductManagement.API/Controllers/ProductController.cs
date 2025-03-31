using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs.Product.Requests;
using ProductManagement.Application.DTOs.Product.Responses;
using ProductManagement.Domain.Models;

namespace ProductManagement.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _mediator.Send(new DeleteProductRequest(id));
            return Ok(new { Message = "Product deleted successfully" });
        }

        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _mediator.Send(new GetProductByIdRequest(id));
            return Ok(response);
        }

        [Authorize]
        [HttpGet("user-products")]
        public async Task<IActionResult> GetProductsByUser()
        {
            var response = await _mediator.Send(new GetProductsByUserRequest());
            return Ok(response);
        }

        [HttpGet("search-products")]
        public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
        {
            var result = await _mediator.Send(new SearchProductsRequest(searchTerm));
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(new { Message = "Product updated successfully" });
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetAll()
        {
            var products = await _mediator.Send(new GetAllProductsRequest());
            return Ok(products);
        }

        [HttpPost("hide-by-user/{userId}")]
        public async Task<IActionResult> HideProductsByUser(Guid userId)
        {
            var success = await _mediator.Send(new HideProductsByUserRequest(userId));
            return Ok(new { Message = "Products hidden successfully" });
        }

        [HttpPost("show-by-user/{userId}")]
        public async Task<IActionResult> ShowProductsByUser(Guid userId)
        {
            var success = await _mediator.Send(new ShowProductsByUserRequest(userId));
            return Ok(new { Message = "Products showed successfully" });
        }

        [HttpGet("filter-products")]
        public async Task<IActionResult> FilterProducts([FromQuery] FilterProductsRequest request)
        {
            var products = await _mediator.Send(request);
            return Ok(products);
        }


        [HttpGet("page")]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetEventsByPage([FromQuery] GetProductsByPageAsyncRequest request)
        {
            var pagedProducts = await _mediator.Send(request);
            return Ok(pagedProducts);
        }

        [Authorize]
        [HttpDelete("delete-all-by-user/{userId}")]
        public async Task<IActionResult> DeleteAllProductsByUser(Guid userId)
        {
            var success = await _mediator.Send(new DeleteAllProductsByUserRequest(userId));
            return Ok(new { Message = "Products deleted successfully" });
        }

    }
}
