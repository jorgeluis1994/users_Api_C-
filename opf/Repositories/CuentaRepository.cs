using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using opf.Interfaces;
using opf.models;

namespace opf.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly ApplicationDbContext _context;
        public CuentaRepository(ApplicationDbContext context){
            _context=context;

        }
        public async Task<Cuenta> AddCuenta(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();
            return cuenta;
            
        }

        public async Task<IEnumerable<Cuenta>> GetCuentas()
        {
            return await _context.Cuentas.ToListAsync();
        }
    }
}