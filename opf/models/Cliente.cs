using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opf.models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        // Relación uno a muchos con Cuentas
        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    }
}