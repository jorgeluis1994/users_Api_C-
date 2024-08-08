using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opf.models
{
    public class Cuenta
    {
        public int CuentaID { get; set; }
        public int ClienteID { get; set; }
        public string Tipo { get; set; }
        public decimal Saldo { get; set; }

        // Relación muchos a uno con Cliente
        public Cliente Cliente { get; set; }
    }
}