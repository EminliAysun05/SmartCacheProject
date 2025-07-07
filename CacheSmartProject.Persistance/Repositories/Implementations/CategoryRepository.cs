
using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace CacheSmartProject.Persistence.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly IConfiguration _configuration;

    public CategoryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private NpgsqlConnection GetConnection() =>
        new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task AddAsync(Category category)
    {
        var query = @"
        INSERT INTO ""Categories"" (""Name"", ""ParentId"", ""LastModified"", ""IsActive"")
        VALUES (@name, @ParentId, @lastModified, @isActive)";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);

        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@ParentId", (object?)category.ParentId ?? DBNull.Value); 
        command.Parameters.AddWithValue("@lastModified", category.LastModified);
        command.Parameters.AddWithValue("@isActive", category.IsActive);

        await command.ExecuteNonQueryAsync();
    }

   
    public async Task<bool> Delete(int id)
    {
        var query = @"DELETE FROM ""Categories"" WHERE ""Id"" = @id";
        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0; 
    }
    public async Task<List<Category?>> GetAllAsync()
    {
        var categories = new List<Category?>();
        var query = @"SELECT ""Id"", ""Name"", ""ParentId"", ""LastModified"", ""IsActive"" FROM ""Categories""";


        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                ParentId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                LastModified = reader.GetDateTime(3),
                IsActive = reader.GetBoolean(4)
            });
        }

        return categories;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var query = @"SELECT ""Id"", ""Name"", ""ParentId"", ""LastModified"", ""IsActive"" 
                      FROM ""Categories"" WHERE ""Id"" = @id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                ParentId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                LastModified = reader.GetDateTime(3),
                IsActive = reader.GetBoolean(4)
            };
        }

        return null;
    }

    public async Task<DateTime?> GetLastModifiedAsync()
    {
        var query = @"SELECT MAX(""LastModified"") FROM ""Categories""";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result is DBNull or null ? null : Convert.ToDateTime(result);
    }

   



    public async Task<bool> Update(Category category)
    {
        var query = @"
            UPDATE ""Categories""
            SET ""Name"" = @name,
                ""LastModified"" = @lastModified,
                ""IsActive"" = @isActive
            WHERE ""Id"" = @id";
        using var connection = GetConnection();
        await connection.OpenAsync();
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", category.Id);
        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@LastModified", category.LastModified);

        int affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

}
