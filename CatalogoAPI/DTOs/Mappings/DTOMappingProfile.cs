using AutoMapper;
using CatalogoAPI.Models;

namespace CatalogoAPI.DTOs.Mappings;

public class DTOMappingProfile : Profile
{
    public DTOMappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpdateRequest>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpdateResponse>().ReverseMap();
    }
}
