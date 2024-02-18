using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinha.Backend.API.Models
{
    public class Saldo
    {
        public int Total { get; set; }
        public DateTime DataExtracao { get; set; }
        public int Limite { get; set; }
    }
}