using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DapperTask.Models;

namespace DapperTask.Repositories
{
    public class OrganizationRepository
    {
        private readonly string _connectionString;

        public OrganizationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to get all organizations
        public async Task<IEnumerable<Organizations>> GetAllAsync()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Organizations";  // Adjust table name if needed
                return await dbConnection.QueryAsync<Organizations>(query);
            }
        }

        internal Departments GetById(int departmentId)
        {
            throw new NotImplementedException();
        }
    }
}
