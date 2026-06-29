using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaizesDoNordeste.API.Application.DTOs
{
    public class ResgatePontosDto
    {
        [JsonIgnore]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "A quantidade de pontos para resgate é obrigatória.")]
        [Range(50, int.MaxValue, ErrorMessage = "O resgate mínimo permitido é de 50 pontos.")]
        public int PontosParaResgatar { get; set; }
    }
}