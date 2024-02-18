using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rinha.Backend.API.Models
{
    public class TransacaoRequest
    {
        public int Valor { get; set; }
        public string Tipo { get; set; }
        [MaxLength(10)]
        public string Descricao { get; set; }
    }
}