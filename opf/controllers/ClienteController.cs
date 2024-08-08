using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using opf.models;
using opf.Services;

namespace opf.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {

        private readonly ClienteService _clienteService;

         // Inyectamos el servicio en el constructor
        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            return Ok(await _clienteService.GetClientesDapper());
   
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clienteService.GuardarClienteAsync(cliente);
            return CreatedAtAction(nameof(Post), new { id = result.ClienteID }, result);
        }
        
    }
}