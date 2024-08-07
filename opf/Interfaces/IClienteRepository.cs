using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using opf.models;

namespace opf.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetClientesAsync();
        Task<Cliente> AddClienteAsync(Cliente cliente);
    }
}