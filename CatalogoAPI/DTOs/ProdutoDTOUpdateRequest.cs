using System.ComponentModel.DataAnnotations;

namespace CatalogoAPI.DTOs;

public class ProdutoDTOUpdateRequest : IValidatableObject
{
    [Required]
    [Range(1, 9999, ErrorMessage = "O estoque deve estar entre 1 e 9999")]
    public float Estoque { get; set; }
    public DateTime DataDeCadastro { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataDeCadastro <= DateTime.Now.Date)
            yield return new ValidationResult("A data deve ser maior ou igual a data atual",
                new[] {nameof(this.DataDeCadastro)});
    }
}
