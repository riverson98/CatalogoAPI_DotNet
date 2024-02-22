using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> BuscaTodosOsProdutos()
    {
        var produtos = _context.Produtos.AsNoTracking()
            .Take(10)
            .ToList();
        
        if (produtos is null)
            return NotFound("Produtos não encontrados");
        
        return produtos;
    }

    [HttpGet("{id:int}", Name = "ObterProduto")]
    public ActionResult<Produto> BuscaProdutosPorId(int id)
    {
        var produto = _context.Produtos
                .AsNoTracking()
                .FirstOrDefault(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Produto não encontrado...");

        return produto;
    }

    [HttpPost]
    public ActionResult CriaProduto(Produto produto)
    {
        if (produto is null)
            return BadRequest();

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
    }

    [HttpPut("{id:int}")]
    public ActionResult AtualizaProduto(int id, Produto produto)
    {
        if(!id.Equals(produto.ProdutoId))
            return BadRequest();

        _context.Entry(produto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult DeletaProdutoPorId(int id)
    {
        var produto = _context.Produtos
                .FirstOrDefault(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Nenhum produto encontrado ...");

        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto);
    }
    
}
