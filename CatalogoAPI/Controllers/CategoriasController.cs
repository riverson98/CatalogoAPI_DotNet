using CatalogoAPI.Context;
using CatalogoAPI.Filters;
using CatalogoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(AppDbContext context, IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("lerArquivoConfiguracao")]
        public string buscaValorDeConfiguracao()
        {
            var valor1 = _configuration["chave1"];
            var valor2 = _configuration["chave2"];
            var secao1 = _configuration["secao1:chave2"];

            return $"Chave 1 = {valor1} \nChave 2 = {valor2} \nSeção 1 = {secao1}";
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> BuscaTodasAsCategoriasEProdutos() 
        {
            _logger.LogInformation("########################### GET  api/categorias BuscaTodasAsCategoriasEProdutos  #######################################");
            return await _context.Categorias
                    .Include(categoria => categoria.Produtos)
                    .Where(produto => produto.CategoriaId <= 5)
                    .ToListAsync();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Categoria>>> BuscaTodasAsCategorias()
        {
            var categorias = await _context.Categorias.AsNoTracking().ToListAsync();

            if (categorias is null)
                return NotFound("Categorias não encontradas");

            return categorias;
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public async Task<ActionResult<Categoria>> BuscaCategoriasPorId(int id)
        {
            _logger.LogInformation($"########################### GET  api/categorias/id = {id}  #######################################");
            var categoria = await _context.Categorias.AsNoTracking()
                    .FirstOrDefaultAsync(categoriaSelecionada => categoriaSelecionada.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Categoria não encontradada...");

            return categoria;
        }

        [HttpPost]
        public async Task<ActionResult> CriaCategoria(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> AtualizaCategoria(int id, Categoria categoria)
        {
            if (!id.Equals(categoria.CategoriaId))
                return BadRequest();

            _context.Entry(categoria).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletaCategoriaPorId(int id)
        {
            var categoria = await _context.Produtos
                    .FirstOrDefaultAsync(categoriaSelecionada => categoriaSelecionada.ProdutoId.Equals(id));

            if (categoria is null)
                return NotFound("Nenhuma categoria encontrada...");

            _context.Produtos.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
    }
}
