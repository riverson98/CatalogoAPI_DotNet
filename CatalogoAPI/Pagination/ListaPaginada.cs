namespace CatalogoAPI.Pagination;

public class ListaPaginada<T> : List<T> where T : class
{
    public int PaginaAtual { get; private set; }
    public int TotalDePagina { get; private set; }
    public int ItensPorPagina { get; private set; }
    public int TotalDeElementos { get; private set; }

    public bool PossuiPaginaAnterior => PaginaAtual > 1;
    public bool PossuiPaginaPosterior => PaginaAtual < TotalDePagina;

    public ListaPaginada(List<T> itens, int totalDeElementos, int numeroDaPagina, int itensPorPagina)
    {
        TotalDeElementos = totalDeElementos;
        ItensPorPagina = itensPorPagina;
        PaginaAtual = numeroDaPagina;
        TotalDePagina = (int)Math.Ceiling(totalDeElementos / (double)itensPorPagina);
        AddRange(itens);
    }

    public static ListaPaginada<T> ParaListaPaginada(IQueryable<T> fonteDeDados, int numeroDaPagina, int itensPorPagina)
    {
        var totalDeElementos = fonteDeDados.Count();
        var itens = fonteDeDados.Skip((numeroDaPagina - 1 ) * itensPorPagina)
            .Take(itensPorPagina)
            .ToList();

        return new ListaPaginada<T>(itens, totalDeElementos, numeroDaPagina, itensPorPagina);
    }
}
