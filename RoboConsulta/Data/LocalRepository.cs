using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace RoboConsulta.Data
{
    public class LocalRepository<T> : ILocalRepository<T> where T : class
    {
        private readonly string _connectionString;

        public LocalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<int> InsertAsync(T entity)
        {
            // TODO: Implement using Dapper
            throw new NotImplementedException();
        }
    }
}
