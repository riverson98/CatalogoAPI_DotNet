namespace CatalogoAPI.Pagination;

public abstract class QueryParamsPaginacao
{
    const int maximoDeItensPorPagina = 50;
    private int _quantidadeDeItensPorPagina = maximoDeItensPorPagina;
    public int NumeroDaPagina { get; set; } = 1;

    public int QuantidadeDeItensPorPagina
    {
        get
        {
            return _quantidadeDeItensPorPagina;
        }
        set
        {
            _quantidadeDeItensPorPagina = (value > maximoDeItensPorPagina) ? maximoDeItensPorPagina : value;
        }
    }
}
