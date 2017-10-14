using Newtonsoft.Json;

namespace FutebassApp.Models
{
    public class Jogador
    {
        [JsonProperty("JogadorId")]
        public int JogadorId { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Cidade")]
        public string Cidade { get; set; }

        [JsonProperty("Foto")]
        public string Foto { get; set; }

        [JsonProperty("Nivel")]
        public string Nivel { get; set; }

        [JsonProperty("IdSocial")]
        public string IdSocial { get; set; }
    }
}
