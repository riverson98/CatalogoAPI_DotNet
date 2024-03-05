using AutoMapper;
using CatalogoAPI.DTOs;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using CatalogoAPI.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CatalogoAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> BuscaTodosOsProdutos()
    {
        var produtos = _unitOfWork.ProdutoRepository.BuscaTodos();

        if (produtos is null)
            return NotFound("Produtos não encontrados");

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [HttpGet("categoria/{id:int}")]
    public ActionResult<IEnumerable<ProdutoDTO>> BuscaTodosOsProdutosPorCategoria(int id)
    {
        var produtos = _unitOfWork.ProdutoRepository.BuscaProdutosPorCategoria(id);

        if (produtos is null)
            return NotFound("Nenhum produto encontrado nesta categoria");

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [HttpGet("paginacao")]
    public ActionResult<IEnumerable<ProdutoDTO>> BuscaTodosOsProdutosComPaginacao([FromQuery] ParametrosDePaginacaoDosProdutos paginacao)
    {
        var produtos = _unitOfWork.ProdutoRepository.BuscaTodosOsProdutosComPaginacao(paginacao);
        return ObtemProdutos(produtos);
    }

    [HttpGet("filtro/preco/paginacao")]
    public ActionResult<IEnumerable<ProdutoDTO>> BuscaProdutosComFiltroBaseadoNoPreco([FromQuery] ProdutosFiltroPreco filtro)
    {
        var produtos = _unitOfWork.ProdutoRepository.FiltraProdutosPorPreco(filtro);
        return ObtemProdutos(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<ProdutoDTO> BuscaProdutosPorId(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Busca(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Produto não encontrado...");

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return Ok(produtoDto);
    }

    [HttpPost]
    public ActionResult<ProdutoDTO> CriaProduto(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoCriado = _unitOfWork.ProdutoRepository.Adiciona(produto);
        _unitOfWork.Commit();

        var produtoCriadoDto = _mapper.Map<ProdutoDTO>(produtoCriado);

        return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriadoDto.ProdutoId }, produtoCriadoDto);
    }

    [HttpPatch("{id:int}/AtualizaParcialmente")]
    public ActionResult<ProdutoDTOUpdateResponse> AtualizaParcialmente(int id, 
        JsonPatchDocument<ProdutoDTOUpdateRequest> produtoRequestDto)
    {
        if(produtoRequestDto is null || id <= 0)
            return BadRequest();

        var produto = _unitOfWork.ProdutoRepository.Busca(produto => produto.ProdutoId.Equals(id));

        if(produto is null)
            return NotFound();

        var produtoAtualizadoRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        produtoRequestDto.ApplyTo(produtoAtualizadoRequest, ModelState);

        if(!ModelState.IsValid || TryValidateModel(produtoAtualizadoRequest))
            return BadRequest(ModelState);
        
        _mapper.Map(produtoAtualizadoRequest, produto);
        _unitOfWork.ProdutoRepository.Atualiza(produto);
        _unitOfWork.Commit();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int}")]
    public ActionResult<ProdutoDTO> AtualizaProduto(int id, ProdutoDTO produtoDto)
    {
        if(!id.Equals(produtoDto.ProdutoId))
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Atualiza(produto);
        _unitOfWork.Commit();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProdutoDTO> DeletaProdutoPorId(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Busca(produto => produto.ProdutoId.Equals(id));

        if (produto is null)
            return NotFound("Nenhum produto encontrado ...");

        var produtoExcluido = _unitOfWork.ProdutoRepository.Deleta(produto);
        _unitOfWork.Commit();

        var produtoExcluidoDto = _mapper.Map<ProdutoDTO>(produtoExcluido);

        return Ok(produtoExcluidoDto);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObtemProdutos(ListaPaginada<Produto> produtos)
    {
        var metadata = new
        {
            produtos.TotalDeElementos,
            produtos.ItensPorPagina,
            produtos.PaginaAtual,
            produtos.TotalDePagina,
            produtos.PossuiPaginaPosterior,
            produtos.PossuiPaginaAnterior
        };
        Response.Headers.Append("X-Paginacao", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }
    
}
