
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyProject.Services;
using MyProject.Models;

namespace MyProject.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProdutoController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public ProdutoController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var img = dto.Img;

            if (img == null || img.Length == 0)
            {
                return BadRequest("Imagem não encontrada.");
            }

            // Define o caminho da pasta onde a imagem será salva
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Verifica se a pasta existe, caso contrário, cria
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Gera nome único pra imagem
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Img = $"/uploads/{fileName}" // Caminho relativo para acessar a imagem
            };
            
            await _mongoDbService.CreateProductAsync(product);
            return CreatedAtAction(nameof(CreateProduct), new { id = product.Id }, product);
        }

        // GET api/produto
        [HttpGet("GetProducts")]
        public async Task<IActionResult> Get()
        {
            var colecao = _mongoDbService.GetCollection<Product>("products");
            var produtos = await colecao.Find(p => true).ToListAsync();
            return Ok(produtos);
        }

        // GET api/produto/{id}
        [HttpGet("GetProductId/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var colecao = _mongoDbService.GetCollection<Product>("products");
            var produto = await colecao.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        // PUT api/produto/{id}
        [HttpPut("UpdateProductId/{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Product produtoAtualizado)
        {
            var colecao = _mongoDbService.GetCollection<Product>("products");
            var resultado = await colecao.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);
            if (resultado.MatchedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE api/produto/{id}
        [HttpDelete("DeleteProductId/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var colecao = _mongoDbService.GetCollection<Product>("products");
            var resultado = await colecao.DeleteOneAsync(p => p.Id == id);
            if (resultado.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
