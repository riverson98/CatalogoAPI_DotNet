using CatalogoAPI.Context;
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

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> BuscaTodasAsCategoriasEProdutos() 
        {
            return _context.Categorias
                    .Include(categoria => categoria.Produtos)
                    .Where(produto => produto.CategoriaId <= 5)
                    .ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> BuscaTodasAsCategorias()
        {
            var categorias = _context.Categorias.AsNoTracking().ToList();

            if (categorias is null)
                return NotFound("Categorias não encontradas");

            return categorias;
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public ActionResult<Categoria> BuscaCategoriasPorId(int id)
        {
            var categoria = _context.Categorias.AsNoTracking()
                    .FirstOrDefault(categoriaSelecionada => categoriaSelecionada.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Categoria não encontradada...");

            return categoria;
        }

        [HttpPost]
        public ActionResult CriaCategoria(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult AtualizaCategoria(int id, Categoria categoria)
        {
            if (!id.Equals(categoria.CategoriaId))
                return BadRequest();

            _context.Entry(categoria).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return Ok(categoria);
        }
        [HttpDelete("{id:int}")]
        public ActionResult DeletaCategoriaPorId(int id)
        {
            var categoria = _context.Produtos
                    .FirstOrDefault(categoriaSelecionada => categoriaSelecionada.ProdutoId.Equals(id));

            if (categoria is null)
                return NotFound("Nenhuma categoria encontrada...");

            _context.Produtos.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }
    }
}
