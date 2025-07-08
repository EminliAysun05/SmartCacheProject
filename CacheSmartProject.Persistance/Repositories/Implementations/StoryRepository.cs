using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CacheSmartProject.Persistence.Repositories.Implementations;

public class StoryRepository : IStoryRepository
{
    private readonly IConfiguration _configuration;

    public StoryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private NpgsqlConnection GetConnection() =>
        new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task AddAsync(Story story)
    {
        var query = @"
            INSERT INTO ""Stories"" (""Title"", ""Content"", ""ImageUrl"", ""LastModified"", ""IsPublished"")
            VALUES (@Title, @Content, @ImageUrl, @LastModified, @IsPublished)";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Title", story.Title);
        command.Parameters.AddWithValue("@Content", story.Content);
        command.Parameters.AddWithValue("@ImageUrl", (object?)story.ImageUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@LastModified", story.LastModified);
        command.Parameters.AddWithValue("@IsPublished", story.IsPublished);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var query = @"DELETE FROM ""Stories"" WHERE ""Id"" = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    public async Task<List<Story>> GetAllAsync()
    {
        var stories = new List<Story>();
        var query = @"SELECT ""Id"", ""Title"", ""Content"", ""ImageUrl"", ""LastModified"", ""IsPublished"" FROM ""Stories""";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            stories.Add(new Story
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2),
                ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                LastModified = reader.GetDateTime(4),
                IsPublished = reader.GetBoolean(5)
            });
        }

        return stories;
    }

    public async Task<Story?> GetByIdAsync(int id)
    {
        var query = @"SELECT ""Id"", ""Title"", ""Content"", ""ImageUrl"", ""LastModified"", ""IsPublished"" 
                      FROM ""Stories"" 
                      WHERE ""Id"" = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Story
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2),
                ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                LastModified = reader.GetDateTime(4),
                IsPublished = reader.GetBoolean(5)
            };
        }

        return null;
    }

    public async Task<DateTime?> GetLastModifiedAsync()
    {
        var query = @"SELECT MAX(""LastModified"") FROM ""Stories""";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result is DBNull or null ? null : Convert.ToDateTime(result);
    }

    public async Task Update(Story story)
    {
        var query = @"
            UPDATE ""Stories""
            SET ""Title"" = @Title,
                ""Content"" = @Content,
                ""ImageUrl"" = @ImageUrl,
                ""LastModified"" = @LastModified,
                ""IsPublished"" = @IsPublished
            WHERE ""Id"" = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", story.Id);
        command.Parameters.AddWithValue("@Title", story.Title);
        command.Parameters.AddWithValue("@Content", story.Content);
        command.Parameters.AddWithValue("@ImageUrl", (object?)story.ImageUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@LastModified", story.LastModified);
        command.Parameters.AddWithValue("@IsPublished", story.IsPublished);

        await command.ExecuteNonQueryAsync();
    }
}
