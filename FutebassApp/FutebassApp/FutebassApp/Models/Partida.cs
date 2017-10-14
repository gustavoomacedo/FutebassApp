using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutebassApp.Models
{
    public class Partida
    {
        [JsonProperty("PartidaId")]
        public int PartidaId { get; set; }

        [JsonProperty("JogadorId")]
        public int JogadorId { get; set; }

        [JsonProperty("Dia")]
        public string Dia { get; set; }

        [JsonProperty("Hora")]
        public string Hora { get; set; }

        [JsonProperty("Observacao")]
        public string Observacao { get; set; }

        [JsonProperty("Preco")]
        public int Preco { get; set; }

        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        [JsonProperty("Local")]
        public string Local { get; set; }
        
        [JsonProperty("Jogador")]
        public virtual Jogador Jogador { get; set; }

    }
}
