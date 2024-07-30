using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] User newUser)
        {
            try
            {
                // string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string query = @"
                    INSERT INTO public.users (first_name, last_name, email, date_of_birth, created_at, ""password"", ids_establishments, ids)
                    VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @CreatedAt, @Password, @IdsEstablishments, @Ids);
                ";

                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("FirstName", newUser.FirstName);
                        command.Parameters.AddWithValue("LastName", newUser.LastName);
                        command.Parameters.AddWithValue("Email", newUser.Email);
                        command.Parameters.AddWithValue("DateOfBirth", newUser.DateOfBirth);
                        command.Parameters.AddWithValue("CreatedAt", DateTime.UtcNow);
                        command.Parameters.AddWithValue("Password", newUser.Password);

                        // Convert the IdsEstablishments object to JSON string
                        string jsonIdsEstablishments = JsonConvert.SerializeObject(newUser.IdsEstablishments);
                        command.Parameters.AddWithValue("IdsEstablishments", NpgsqlTypes.NpgsqlDbType.Jsonb, jsonIdsEstablishments);

                        // Convert the Ids object to JSON string
                        string jsonIds = JsonConvert.SerializeObject(newUser.Ids);
                        command.Parameters.AddWithValue("Ids", NpgsqlTypes.NpgsqlDbType.Jsonb, jsonIds);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("User saved successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

     

      public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime DateOfBirth { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Password { get; set; }
            public object IdsEstablishments { get; set; }
            public object Ids { get; set; }
        }
    }

}