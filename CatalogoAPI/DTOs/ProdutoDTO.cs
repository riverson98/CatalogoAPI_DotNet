using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CatalogoAPI.DTOs;

public class ProdutoDTO
{
    [JsonIgnore]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatorio")]
    [StringLength(80)]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "A Descrição é obrigatoria")]
    [StringLength(300)]
    public string? Descricao { get; set; }

    [Required]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    public int CategoriaId { get; set; }
}
