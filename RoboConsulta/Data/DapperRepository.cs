using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace RoboConsulta.Data
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        private readonly string _connectionString;

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }

        public Task<T?> GetByIdAsync(int id)
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(T entity)
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T entity)
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }
    }
}
