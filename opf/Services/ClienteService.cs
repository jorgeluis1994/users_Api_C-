using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using opf.Interfaces;
using opf.models;

namespace opf.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        // Inyección del repositorio a través del constructor
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<Cliente>> GetClientesDapper()
        {
            return await _clienteRepository.GetClientesDapper();
        }
         public async Task<Cliente> GuardarClienteAsync(Cliente cliente)
        {
            // Lógica adicional de validación o negocio si es necesario
            return await _clienteRepository.AddClienteAsync(cliente);
        }
    }
}