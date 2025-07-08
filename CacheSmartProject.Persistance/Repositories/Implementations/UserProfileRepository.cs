using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CacheSmartProject.Persistence.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IConfiguration _configuration;

    public UserProfileRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
        => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<List<UserProfile>> GetAllAsync()
    {
        var users = new List<UserProfile>();
        var query = "SELECT Id, Name, Email, Preferences, LastModified FROM UserProfiles";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new UserProfile
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Preferences = reader.GetString(3),
                LastModified = reader.GetDateTime(4)
            });
        }

        return users;
    }

    public async Task<UserProfile?> GetByIdAsync(int id)
    {
        var query = "SELECT Id, Name, Email, Preferences, LastModified FROM UserProfiles WHERE Id = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserProfile
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Preferences = reader.GetString(3),
                LastModified = reader.GetDateTime(4)
            };
        }

        return null;
    }

    public async Task AddAsync(UserProfile user)
    {
        var query = @"
            INSERT INTO UserProfiles (Name, Email, Preferences, LastModified)
            VALUES (@Name, @Email, @Preferences, @LastModified)";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Preferences", user.Preferences);
        command.Parameters.AddWithValue("@LastModified", user.LastModified);

        await command.ExecuteNonQueryAsync();
    }

    public async Task Update(UserProfile user)
    {
        var query = @"
            UPDATE UserProfiles
            SET Name = @Name,
                Email = @Email,
                Preferences = @Preferences,
                LastModified = @LastModified
            WHERE Id = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Preferences", user.Preferences);
        command.Parameters.AddWithValue("@LastModified", user.LastModified);
        command.Parameters.AddWithValue("@Id", user.Id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id)
    {
        var query = "DELETE FROM UserProfiles WHERE Id = @Id";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<DateTime?> GetLastModifiedAsync()
    {
        var query = "SELECT MAX(LastModified) FROM UserProfiles";

        using var connection = GetConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result is DBNull or null ? null : Convert.ToDateTime(result);
    }

   
}
