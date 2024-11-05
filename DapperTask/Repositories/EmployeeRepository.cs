using DapperTask.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DapperTask.Repositories  // <-- Ensure the namespace is correct
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Employees>> GetAllAsync()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                string query = "SELECT * FROM Departments";  // Adjust query as per your schema
                return await dbConnection.QueryAsync<Employees>(query);
            }
        }

        // Add other methods for CRUD operations (Add, GetById, Update, Delete, etc.)
    }
}
