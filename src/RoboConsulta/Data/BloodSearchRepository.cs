using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using RoboConsulta.Configuration;
using RoboConsulta.Models;

namespace RoboConsulta.Data;

public class BloodSearchRepository
{
    private readonly string _connectionString;

    public BloodSearchRepository(IOptions<BloodSearchOptions> options)
    {
        _connectionString = $"Data Source={options.Value.DatabasePath}";
        CreateTableIfMissing();
    }

    public async Task InsertAsync(IEnumerable<BloodSearchLog> logs)
    {
        using var connection = new SqliteConnection(_connectionString);

        await connection.ExecuteAsync(
            """
            INSERT INTO BloodSearchLogs (BloodType, Status, SearchedAt)
            VALUES (@BloodType, @Status, @SearchedAt)
            """,
            logs);
    }

    private void CreateTableIfMissing()
    {
        using var connection = new SqliteConnection(_connectionString);

        connection.Execute(
            """
            CREATE TABLE IF NOT EXISTS BloodSearchLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BloodType TEXT NOT NULL,
                Status TEXT NOT NULL,
                SearchedAt TEXT NOT NULL
            )
            """);
    }
}
