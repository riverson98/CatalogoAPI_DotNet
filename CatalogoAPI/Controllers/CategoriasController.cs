using AutoMapper;
using CatalogoAPI.DTOs;
using CatalogoAPI.Filters;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using CatalogoAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using X.PagedList;

namespace CatalogoAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("DominiosPermitidos")]
//[EnableRateLimiting("fixedwindow")]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<CategoriasController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("lerArquivoConfiguracao")]
    public string buscaValorDeConfiguracao()
    {
        var valor1 = _configuration["chave1"];
        var valor2 = _configuration["chave2"];
        var secao1 = _configuration["secao1:chave2"];

        return $"Chave 1 = {valor1} \nChave 2 = {valor2} \nSeção 1 = {secao1}";
    }

    //[Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaTodasAsCategorias()
    {
        var categorias = await _unitOfWork.CategoriaRepository.BuscaTodosAsync();

        if (categorias is null)
            return NotFound("Categorias não encontradas");

        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDto);
    }

    [HttpGet("paginacao")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaTodosAsCategoriasComPaginacao([FromQuery] 
                                                                                   ParametrosDePaginacaoDasCategorias paginacao)
    { 
        var categorias = await _unitOfWork.CategoriaRepository.BuscaTodasAsCategoriasComPaginacaoAsync(paginacao);
        return ObtemCategorias(categorias);
    }

    [HttpGet("filtro/nome/paginacao")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaCategoriasFiltradas([FromQuery] CategoriasFiltroNome filtro)
    {
        var categoriasFiltradas = await _unitOfWork.CategoriaRepository.FiltraCategoriaPorNomeAsync(filtro);
        return ObtemCategorias(categoriasFiltradas);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> BuscaCategoriasPorId(int id)
    {
        _logger.LogInformation($"########################### GET  api/categorias/id = {id}  #######################################");
        var categoria = await _unitOfWork.CategoriaRepository.BuscaAsync(categoria => categoria.CategoriaId.Equals(id));

        if (categoria is null)
            return NotFound("Categoria não encontradada...");

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> CriaCategoria(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
            return BadRequest();


        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaCriada = _unitOfWork.CategoriaRepository.Adiciona(categoria);
        await _unitOfWork.CommitAsync();

        var novaCategoriaDto = _mapper.Map<CategoriaDTO>(categoriaCriada);

        return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
    }

    [HttpPut("{id:int}")]
    [DisableCors]
    public async Task<ActionResult<CategoriaDTO>> AtualizaCategoria(int id, CategoriaDTO categoriaDto)
    {
        if (!id.Equals(categoriaDto.CategoriaId))
            return BadRequest();

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var categoriaAtualizada = _unitOfWork.CategoriaRepository.Atualiza(categoria);
        await _unitOfWork.CommitAsync();

        var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoriaDTO>> DeletaCategoriaPorId(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.BuscaAsync(categoria => categoria.CategoriaId.Equals(id));

        if (categoria is null)
            return NotFound("Nenhuma categoria encontrada...");

        var categoriaExcluida = _unitOfWork.CategoriaRepository.Deleta(categoria);
        await _unitOfWork.CommitAsync();

        var categoriaExcluidaDto = _mapper.Map<CategoriaDTO>(categoriaExcluida);

        return Ok(categoriaExcluidaDto);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObtemCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("X-Paginacao", JsonConvert.SerializeObject(metadata));

        var CategoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(CategoriasDto);
    }
}
