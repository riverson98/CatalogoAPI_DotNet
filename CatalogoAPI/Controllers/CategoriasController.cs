using Asp.Versioning;
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
using Microsoft.AspNetCore.Http;

namespace CatalogoAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[EnableCors("DominiosPermitidos")]
//[EnableRateLimiting("fixedwindow")]
[ApiVersion("1.0")]
[Produces("application/json")]
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

    /// <summary>
    /// Obtem uma lista de objetos Categoria
    /// </summary>
    /// <returns>Lista de objetos categorias</returns>
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaTodasAsCategorias()
    {
        var categorias = await _unitOfWork.CategoriaRepository.BuscaTodosAsync();

        if (categorias is null)
            return NotFound("Categorias não encontradas");

        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDto);
    }

    /// <summary>
    /// Obtem uma categoria pelo seu id
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     GET api/v1/categorias/paginacao?NumeroDaPagina=1&amp;QuantidadeDeItensPorPagina=1
    /// </remarks>
    /// <param name="paginacao">Numero da pagina e quantidades de itens a ser buscado (max:50)</param>
    /// <returns>Objeto categoria</returns>
    [HttpGet("paginacao")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaTodosAsCategoriasComPaginacao([FromQuery] 
                                                                                   ParametrosDePaginacaoDasCategorias paginacao)
    { 
        var categorias = await _unitOfWork.CategoriaRepository.BuscaTodasAsCategoriasComPaginacaoAsync(paginacao);
        return ObtemCategorias(categorias);
    }

    /// <summary>
    /// Obtem uma lista categoria pelo nome
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     GET api/v1/categorias/filtro/nome/paginacao?Nome=exemplo&amp;NumeroDaPagina=1&amp;QuantidadeDeItensPorPagina=1
    /// </remarks>
    /// <param name="filtro">Nome do objeto numero da pagina e quantidade de itens por pagina(max:50)</param>
    /// <returns>Objeto categoria filtrado e paginado</returns>
    [HttpGet("filtro/nome/paginacao")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> BuscaCategoriasFiltradas([FromQuery] CategoriasFiltroNome filtro)
    {
        var categoriasFiltradas = await _unitOfWork.CategoriaRepository.FiltraCategoriaPorNomeAsync(filtro);
        return ObtemCategorias(categoriasFiltradas);
    }

    /// <summary>
    /// Obtem uma categoria pelo seu id
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     GET api/v1/categorias?id=1
    /// </remarks>
    /// <param name="id">Codigo do objeto categoria buscado</param>
    /// <returns>Objeto categoria</returns>
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDTO>> BuscaCategoriasPorId(int id)
    {
        _logger.LogInformation($"########################### GET  api/categorias/id = {id}  #######################################");
        var categoria = await _unitOfWork.CategoriaRepository.BuscaAsync(categoria => categoria.CategoriaId.Equals(id));

        if (categoria is null)
            return NotFound("Categoria não encontradada...");

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDto);
    }

    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    ///     
    ///     POST api/v1/categorias
    ///     {
    ///         "nome" : "categoria exemplo",
    ///         "imagemUrl":"http://categoriaExemplo.jpg"
    ///     }
    /// </remarks>
    /// <param name="categoriaDto">Objeto categoria</param>
    /// <returns>Objeto categoria criada</returns>
    /// <remarks>Retorna um objeto categoria incluido</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Atualiza informações do objeto categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     PUT api/v1/categorias?id=1 
    ///     {
    ///         "categoriaId":"1"
    ///         "nome" : "categoria exemplo",
    ///         "imagemUrl":"http://categoriaExemplo.jpg"
    ///     }
    /// </remarks>
    /// <param name="id">Codigo do objeto categoria e ser editado</param>
    /// <param name="categoriaDto">Informacões do objeto a ser incluido</param>
    /// <returns>Objeto categoria atualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
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

    /// <summary>
    /// Deleta objeto categoria pelo o id
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     DELETE api/v1/categorias?id=1
    /// </remarks>
    /// <param name="id">Codigo do objeto categoria</param>
    /// <returns>Categoria excluida</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
