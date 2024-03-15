using CatalogoAPI.Validations;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogoAPI.Models;

public class Produto : IValidatableObject
{
    [JsonIgnore]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatorio")]
    [StringLength(80)]
    //[PrimeiraLetraMaiuscula]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "A Descrição é obrigatoria")]
    [StringLength(300)]
    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName ="decimal(10,2)")]
    [Range(1, 50000)]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime DataDeCadastro { get; set; }
    public int CategoriaId { get; set; }
    
    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(this.Nome))
        {
            var primeiraLetra = this.Nome?.ToString()[0]
                .ToString();

            if (!primeiraLetra!.Equals(primeiraLetra.ToUpper()))
                yield return 
                    new ValidationResult("A primeira letra do nome do produto deve ser maiúscula",
                    new[] {nameof(this.Nome)});
        }

        if (this.Estoque <= 0)
            yield return new ValidationResult("O estoque dedve ser maior que zero", new[] {
                nameof(this.Estoque) 
            });
    }       
}
