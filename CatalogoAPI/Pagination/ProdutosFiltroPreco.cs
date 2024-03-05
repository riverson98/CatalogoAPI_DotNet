namespace CatalogoAPI.Pagination;

public class ProdutosFiltroPreco : QueryParamsPaginacao
{
    public decimal? Preco { get; set; }
    public string? PrecoCriterio { get; set; } //maior, menor ou igual
}
