using CatalogoAPI.Filters;
using CatalogoAPI.Models;
using CatalogoAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IRepository<Categoria> _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(IRepository<Categoria> repository, IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _repository = repository;
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

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Categoria>> BuscaTodasAsCategorias()
        {
            var categorias = _repository.BuscaTodos();

            if (categorias is null)
                return NotFound("Categorias não encontradas");

            return Ok(categorias);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public ActionResult<Categoria> BuscaCategoriasPorId(int id)
        {
            _logger.LogInformation($"########################### GET  api/categorias/id = {id}  #######################################");
            var categoria = _repository.Busca(categoria => categoria.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Categoria não encontradada...");

            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult CriaCategoria(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            var categoriaCriada = _repository.Adiciona(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult AtualizaCategoria(int id, Categoria categoria)
        {
            if (!id.Equals(categoria.CategoriaId))
                return BadRequest();

            _repository.Atualiza(categoria);

            return Ok(categoria);
        }
        [HttpDelete("{id:int}")]
        public ActionResult DeletaCategoriaPorId(int id)
        {
            var categoria = _repository.Busca(categoria => categoria.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Nenhuma categoria encontrada...");

            _repository.Deleta(categoria);

            return Ok(categoria);
        }
    }
}
