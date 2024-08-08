using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using opf.Interfaces;
using opf.models;
using opf.Repositories;

namespace opf.Services
{
    public class CuentaService
    {

        private readonly ICuentaRepository _cuentaRepository;
        public CuentaService(ICuentaRepository cuentaRepository){
            _cuentaRepository=cuentaRepository;

        }

        public async Task<IEnumerable<Cuenta>> GetClientesAsync()
        {
            return await _cuentaRepository.GetCuentas();
        }
        
    }
}