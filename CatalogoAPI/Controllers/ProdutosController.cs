using CatalogoAPI.Models;
using CatalogoAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProdutosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> BuscaTodosOsProdutos()
    {
        var produtos = _unitOfWork.ProdutoRepository.BuscaTodos();
        
        if (produtos is null)
            return NotFound("Produtos não encontrados");
        
        return Ok(produtos);
    }

    [HttpGet("categoria/{id:int}")]
    public ActionResult<IEnumerable<Categoria>> BuscaTodosOsProdutosPorCategoria(int id)
    {
        var produtos = _unitOfWork.ProdutoRepository.BuscaProdutosPorCategoria(id);
        
        if (produtos is null)
            return NotFound("Nenhum produto encontrado nesta categoria");
        
        return Ok(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult BuscaProdutosPorId(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Busca(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Produto não encontrado...");

        return Ok(produto);
    }

    [HttpPost]
    public ActionResult CriaProduto(Produto produto)
    {
        if (produto is null)
            return BadRequest();

        var produtoCriado = _unitOfWork.ProdutoRepository.Adiciona(produto);
        _unitOfWork.Commit();

        return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriado.ProdutoId }, produtoCriado);
    }

    [HttpPut("{id:int}")]
    public ActionResult AtualizaProduto(int id, Produto produto)
    {
        if(!id.Equals(produto.ProdutoId))
            return BadRequest();

        _unitOfWork.ProdutoRepository.Atualiza(produto);
        _unitOfWork.Commit();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult DeletaProdutoPorId(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Busca(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Nenhum produto encontrado ...");

        _unitOfWork.ProdutoRepository.Deleta(produto);
        _unitOfWork.Commit();

        return Ok(produto);
    }
    
}
