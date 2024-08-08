using Microsoft.EntityFrameworkCore;
using opf.Interfaces;
using opf.models;
using Dapper;
using System.Data.SqlClient;
using Npgsql;

namespace opf.Repositories
{
    public class ClienteRepository : IClienteRepository
    
    {
        private readonly ApplicationDbContext _context;

         private readonly string _connectionString;
        public ClienteRepository(ApplicationDbContext context,IConfiguration configuration){
            _context=context;
            // Obtener la cadena de conexión del archivo appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection");
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

      public async Task<IEnumerable<Cliente>> GetClientesDapper()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT * FROM \"Clientes\""; // Usa comillas dobles correctamente escapadas
                return await connection.QueryAsync<Cliente>(sql);
            }
        }


    }
}