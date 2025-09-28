using MySql.Data.MySqlClient;
using System.Data;

namespace barberappAPI.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // 📌 Obtener todos los usuarios
        public async Task<DataTable> GetUsuariosAsync()
        {
            DataTable table = new DataTable();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM usuarios"; // 👈 ahora apunta a tu tabla usuarios
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // Obtener todas las citas
        public async Task<DataTable> GetCitasAsync()
        {
            DataTable table = new DataTable();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM citas"; // apunta a tu tabla citas
                using (var command = new MySqlCommand(query, connection))
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }
    }
}
