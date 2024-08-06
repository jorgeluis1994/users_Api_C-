using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace opf.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
         private readonly string _connectionString;
         private readonly IConfiguration _configuration;


        // Constructor que recibe IConfiguration para acceder a la configuración.
        public UsersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _configuration=configuration;
        }
    
    
       [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var dataTable = new DataTable();

             var token = GenerateJwtToken("Jorgeortiz");

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

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveUser([FromBody] User newUser)
        {
            try
            {
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
                        command.Parameters.AddWithValue("Password", hashedPassword);
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
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User newUser)	{
            try {

            // Hash the password before saving
            // string hashedPassword = HashPassword(newUser.Password);
                // Definir la consulta SQL con parámetros
            string query = "SELECT * FROM users WHERE email = @Email AND password = @Password";
            // Crear la tabla de datos para almacenar los resultados
            DataTable resultTable = new DataTable();
            await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                    // Añadir los parámetros al comando
                    command.Parameters.AddWithValue("@Email", newUser.Email);
                    command.Parameters.AddWithValue("@Password", newUser.Password);
                    // Ejecutar la consulta y llenar la tabla de datos
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(resultTable);
                    }
                    
                       
                    }
                    
               
                }
                // Verificar si se encontró el usuario
            if (resultTable.Rows.Count > 0)
            {
                var user = resultTable.Rows[0];
                var token = GenerateJwtToken(user.ItemArray.ToString());

                return Ok(new { Token = token });
            }
            else
            {
                // Usuario no encontrado
                return Unauthorized("Invalid email or password");
            }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        private static string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                return Convert.ToBase64String(hashBytes);
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