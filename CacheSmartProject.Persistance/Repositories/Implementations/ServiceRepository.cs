using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CacheSmartProject.Persistence.Repositories.Implementations;

public class ServiceRepository : IServiceRepository
{
    private readonly IConfiguration _configuration;

    public ServiceRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private NpgsqlConnection GetConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task AddAsync(Service service)
    {
        var query = @"INSERT INTO ""Services"" (""Name"", ""Description"", ""Price"", ""LastModified"", ""IsActive"")
             VALUES (@Name, @Description, @Price, @LastModified, @IsActive)";

        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);

        command.Parameters.AddWithValue("Name", service.Name);
        command.Parameters.AddWithValue("Description", (object?)service.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("Price", service.Price);
        command.Parameters.AddWithValue("LastModified", service.LastModified);
        command.Parameters.AddWithValue("IsActive", service.IsActive);

        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<bool> Delete(int id)
    {
        var query = @"DELETE FROM ""Services"" WHERE ""Id"" = @Id";
        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0; 
    }

    public async Task<List<Service>> GetAllAsync()
    {
        var services = new List<Service>();
        var query = @"SELECT ""Id"", ""Name"", ""Description"", ""Price"", ""LastModified"", ""IsActive"" FROM ""Services""";

        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            services.Add(new Service
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                LastModified = reader.GetDateTime(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return services;
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        var query = @"SELECT ""Id"", ""Name"", ""Description"", ""Price"", ""LastModified"", ""IsActive"" FROM ""Services"" WHERE ""Id"" = @Id";
        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("Id", id);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Service
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                LastModified = reader.GetDateTime(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<DateTime?> GetLastModifiedAsync()
    {
        var query = @"SELECT MAX(""LastModified"") FROM ""Services""";
        await using var connection = GetConnection();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result is DBNull ? null : Convert.ToDateTime(result);
    }

    public async Task<bool> Update(Service service)
    {
        var query = @"
        UPDATE ""Services""
        SET ""Name"" = @Name,
            ""Description"" = @Description,
            ""Price"" = @Price,
            ""LastModified"" = @LastModified,
            ""IsActive"" = @IsActive
        WHERE ""Id"" = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("Name", service.Name);
        command.Parameters.AddWithValue("Description", (object?)service.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("Price", service.Price);
        command.Parameters.AddWithValue("LastModified", service.LastModified);
        command.Parameters.AddWithValue("IsActive", service.IsActive);
        command.Parameters.AddWithValue("Id", service.Id);


        var affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

}

