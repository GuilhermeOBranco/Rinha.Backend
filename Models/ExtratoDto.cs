using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinha.Backend.API.Models
{
    public class ExtratoDto
    {
        public Saldo Saldo { get; set; }
        public List<Transacao> UltimasTransacoes { get; set; }
    }
}