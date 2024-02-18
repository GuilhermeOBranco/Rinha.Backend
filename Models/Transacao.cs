using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinha.Backend.API.Models
{
    public class Transacao
    {
        public int Valor { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public DateTime RealizadaEm { get; set; }
    }
}