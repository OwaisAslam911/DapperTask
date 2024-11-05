using Dapper;
using DapperTask.Models;
using Microsoft.Data.SqlClient;

namespace DapperTask.Repositories
{
    public class PositionRepository
    {
        private readonly string _connectionString;

        public PositionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Positions>> GetAllAsync()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                string query = "SELECT * FROM Positions";  // Adjust query as per your schema
                return await dbConnection.QueryAsync<Positions>(query);
            }
        }

        // Other methods like GetById, Add, Update, Delete, etc.
    }
}
