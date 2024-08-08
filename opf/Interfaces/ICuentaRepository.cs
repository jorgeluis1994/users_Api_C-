using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using opf.models;

namespace opf.Interfaces
{
    public interface ICuentaRepository
    {
        Task<IEnumerable<Cuenta>> GetCuentas();
        Task<Cuenta> AddCuenta(Cuenta cuenta);
        
    }
}