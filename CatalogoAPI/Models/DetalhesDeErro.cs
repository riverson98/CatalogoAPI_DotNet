using System.Text.Json;

namespace CatalogoAPI.Models
{
    public class DetalhesDeErro
    {
        public int StatusCode { get; set; }
        public string? Mensagem { get; set; }
        public string? Rastro { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
