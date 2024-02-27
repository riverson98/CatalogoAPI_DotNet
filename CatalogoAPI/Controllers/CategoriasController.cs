using AutoMapper;
using CatalogoAPI.DTOs;
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

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> BuscaTodasAsCategorias()
        {
            var categorias = _unitOfWork.CategoriaRepository.BuscaTodos();

            if (categorias is null)
                return NotFound("Categorias não encontradas");

            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return Ok(categoriasDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> BuscaCategoriasPorId(int id)
        {
            _logger.LogInformation($"########################### GET  api/categorias/id = {id}  #######################################");
            var categoria = _unitOfWork.CategoriaRepository.Busca(categoria => categoria.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Categoria não encontradada...");

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDto);
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> CriaCategoria(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
                return BadRequest();


            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaCriada = _unitOfWork.CategoriaRepository.Adiciona(categoria);
            _unitOfWork.Commit();

            var novaCategoriaDto = _mapper.Map<CategoriaDTO>(categoriaCriada);

            return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> AtualizaCategoria(int id, CategoriaDTO categoriaDto)
        {
            if (!id.Equals(categoriaDto.CategoriaId))
                return BadRequest();

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaAtualizada = _unitOfWork.CategoriaRepository.Atualiza(categoria);
            _unitOfWork.Commit();

            var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);

            return Ok(categoriaAtualizadaDto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> DeletaCategoriaPorId(int id)
        {
            var categoria = _unitOfWork.CategoriaRepository.Busca(categoria => categoria.CategoriaId.Equals(id));

            if (categoria is null)
                return NotFound("Nenhuma categoria encontrada...");

            var categoriaExcluida = _unitOfWork.CategoriaRepository.Deleta(categoria);
            _unitOfWork.Commit();

            var categoriaExcluidaDto = _mapper.Map<CategoriaDTO>(categoriaExcluida);

            return Ok(categoriaExcluidaDto);
        }
    }
}
