
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyProject.Services;
using MyProject.Models;
using System.Globalization;
using MongoDB.Bson;

namespace MyProject.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProdutoController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly CloudinaryService _cloudinary;

        public ProdutoController(MongoDbService mongoDbService, CloudinaryService cloudinary)
        {
            _mongoDbService = mongoDbService;
            _cloudinary = cloudinary;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto dto)
        {
            var senhaCerta = Environment.GetEnvironmentVariable("AdminSettings__Password");

            if (!decimal.TryParse(dto.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out var precoDecimal) ||
                dto.Quantity < 0)
            {
                return BadRequest("Preço ou quantidade inválidos.");
            }

            if (dto.AdminPassword != senhaCerta)
            {
                var productFake = new Product
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = dto.Name,
                    Price = precoDecimal,
                    Description = dto.Description,
                    Quantity = dto.Quantity,
                    Img = "https://res.cloudinary.com/dzjynsyhg/image/upload/v1747883911/samples/cloudinary-logo-vector.svg"
                };

                return Ok(productFake);
            }

            if (dto.Img == null || dto.Img.Length == 0)
            {
                return BadRequest("Imagem não encontrada.");
            }

            string imageUrl;
            try
            {
                imageUrl = await _cloudinary.UploadImageAsync(dto.Img);
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao enviar imagem: {ex.Message}");
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = precoDecimal,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Img = imageUrl
            };

            await _mongoDbService.CreateProductAsync(product);

            return CreatedAtAction(nameof(CreateProduct), new { id = product.Id }, product);
        }


        [HttpPut("UpdateProductId/{id}")]
        public async Task<IActionResult> Put(string id, [FromForm] ProductDto dto)
        {
            var senhaCerta = Environment.GetEnvironmentVariable("AdminSettings__Password");

            if (!decimal.TryParse(dto.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out var precoDecimal) ||
                dto.Quantity < 0)
            {
                return BadRequest("Preço ou quantidade inválidos.");
            }

            if (dto.AdminPassword != senhaCerta)
            {
                var productFake = new Product
                {
                    Id = id,
                    Name = dto.Name,
                    Price = precoDecimal,
                    Description = dto.Description,
                    Quantity = dto.Quantity,
                    Img = "https://res.cloudinary.com/dzjynsyhg/image/upload/v1747883911/samples/cloudinary-logo-vector.svg"
                };

                return Ok(productFake);
            }

            var colecao = _mongoDbService.GetCollection<Product>("products");
            var produtoExistente = await colecao.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (produtoExistente == null)
                return NotFound();

            string imgUrl = produtoExistente.Img;

            if (dto.Img != null && dto.Img.Length > 0)
            {
                try
                {
                    await _cloudinary.DeleteImageAsync(produtoExistente.Img);
                    imgUrl = await _cloudinary.UploadImageAsync(dto.Img);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao atualizar imagem: {ex.Message}");
                }
            }

            var product = new Product
            {
                Id = produtoExistente.Id,
                Name = dto.Name,
                Price = precoDecimal,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Img = imgUrl
            };

            var resultado = await colecao.ReplaceOneAsync(p => p.Id == id, product);
            if (resultado.MatchedCount == 0)
                return NotFound();

            return Ok(product);
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

        // DELETE api/produto/{id}
        [HttpDelete("DeleteProductId/{id}")]
        public async Task<IActionResult> Delete(string id, [FromBody] ProductDelete adm)
        {
            Console.WriteLine(adm.AdminPassword);

            var senhaCerta = Environment.GetEnvironmentVariable("AdminSettings__Password");

            // Se a senha for diferente, não faz nada e responde OK (fake)
            if (adm.AdminPassword != senhaCerta)
            {
                return Ok(new
                {
                    id,
                    message = "Produto excluído com sucesso!"
                });
            }

            var colecao = _mongoDbService.GetCollection<Product>("products");

            //Pegar valores do Produto
            var produto = await colecao.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (produto == null)
            {
                return NotFound();
            }

            //Apagar o produto do banco
            var resultado = await colecao.DeleteOneAsync(p => p.Id == id);
            if (resultado.DeletedCount == 0)
            {
                return NotFound();
            }


            // Verificar se tem imagem e deletar do Cloudinary
            if (!string.IsNullOrEmpty(produto.Img))
            {
                try
                {
                    await _cloudinary.DeleteImageAsync(produto.Img);
                }
                catch (Exception ex)
                {
                    // Loga o erro ou trata conforme seu fluxo
                    Console.WriteLine($"Erro ao deletar imagem: {ex.Message}");
                }
            }


            return Ok(new
            {
                id = produto.Id,
                message = "Produto excluído com sucesso!"
            });
        }
    }
}
