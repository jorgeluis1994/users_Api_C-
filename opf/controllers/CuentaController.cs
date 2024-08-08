using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using opf.Services;

namespace opf.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentaController : ControllerBase
    {
        private readonly CuentaService _cuentaService;

         // Inyectamos el servicio en el constructor
        public CuentaController(CuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            return Ok(await _cuentaService.GetClientesAsync());
   
        }
        
    }
}