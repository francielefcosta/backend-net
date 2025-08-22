using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyProject.Services;
using MyProject.Models;
using System.Globalization;
using MongoDB.Bson;


namespace MyProject.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public AuthController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        //[HttpPost("Login")]
        // public async Task<IActionResult> Login([FromBody] LoginDto dto)
        // {
        //     if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
        //     {
        //         return BadRequest("Usuário ou senha inválidos.");
        //     }

        //     var user = await _mongoDbService.GetUserByUsernameAsync(dto.Username);
        //     if (user == null || user.Password != dto.Password)
        //     {
        //         return Unauthorized("Usuário ou senha incorretos.");
        //     }

        //     // Aqui você pode gerar um token JWT ou realizar outras ações de autenticação

        //     return Ok(new { Message = "Login bem-sucedido!" });
        // }
    }
}