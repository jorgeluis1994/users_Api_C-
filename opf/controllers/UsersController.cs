using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace opf.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
         private readonly string _connectionString;

        // Constructor que recibe IConfiguration para acceder a la configuración.
        public UsersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
    
    
       [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var dataTable = new DataTable();

            try
            {
                // Crea una conexión a la base de datos usando la cadena de conexión.
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Crea un comando SQL para obtener todos los usuarios.
                    using (var command = new NpgsqlCommand("SELECT * FROM users", connection))
                    {
                        // Usa un DataAdapter para llenar el DataTable con los resultados de la consulta.
                        using (var dataAdapter = new NpgsqlDataAdapter(command))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }

                // Convierte el DataTable a una lista de diccionarios.
                var result = dataTable.AsEnumerable()
                    .Select(row => dataTable.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col]))
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Manejo de errores: retorna un código de estado 500 si ocurre una excepción.
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}