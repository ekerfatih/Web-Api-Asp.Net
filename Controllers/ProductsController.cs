using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers {


    //localhost:5000/api/products
    
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase {
        private readonly ProductsContext _context;

        public ProductsController(ProductsContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts() {
            var products = await _context.Products.Where(i => i.IsActive).Select(p => ProductToDTO(p)).ToListAsync();
            if (products == null) return NotFound();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var product = await _context.Products.Where(p=> p.ProductId == id).Select(p => ProductToDTO(p)).FirstOrDefaultAsync();
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product) {
            try {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Created Product ID: {product.ProductId}");

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving product: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity) {
            if (id != entity.ProductId) return BadRequest();
            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            if (product == null) return BadRequest();
            product.ProductName = entity.ProductName;
            product.Price = entity.Price;
            product.IsActive = entity.IsActive;
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int? id) {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                return NotFound();
            }
            return NoContent();

        }



        private static ProductDTO ProductToDTO(Product p) {
            var entity = new ProductDTO();
            if (p != null) {
                entity.ProductId = p.ProductId;
                entity.ProductName = p.ProductName;
                entity.Price = p.Price;
            }
            return entity;
        }

    }
}