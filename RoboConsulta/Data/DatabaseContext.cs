using Microsoft.Data.Sqlite;
using Dapper;
using RoboConsulta.Models;

namespace RoboConsulta.Data;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
        Initialize();
    }

    private void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS BloodSearchLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BloodType TEXT NOT NULL,
                Status TEXT NOT NULL,
                SearchedAt TEXT NOT NULL
            )";
        command.ExecuteNonQuery();
    }

    public async Task InsertBloodSearchLogAsync(BloodSearchLog log)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = @"
            INSERT INTO BloodSearchLogs (BloodType, Status, SearchedAt)
            VALUES (@BloodType, @Status, @SearchedAt)";

        await connection.ExecuteAsync(query, log);
    }

    public async Task<IEnumerable<BloodSearchLog>> GetLastSearchesAsync(int count = 100)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = @"
            SELECT Id, BloodType, Status, SearchedAt
            FROM BloodSearchLogs
            ORDER BY SearchedAt DESC
            LIMIT @Count";

        return await connection.QueryAsync<BloodSearchLog>(query, new { Count = count });
    }
}
