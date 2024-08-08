using Microsoft.EntityFrameworkCore;
using opf.Interfaces;
using opf.models;

namespace opf.Repositories
{
    public class ClienteRepository : IClienteRepository
    
    {
        private readonly ApplicationDbContext _context;
        public ClienteRepository(ApplicationDbContext context){
            _context=context;
        }

        public async Task<Cliente> AddClienteAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<IEnumerable<Cliente>> GetClientesAsync()
        {
           return await _context.Clientes.ToListAsync();
        }
    }
}