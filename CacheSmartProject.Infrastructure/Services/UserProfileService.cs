

using AutoMapper;
using CacheSmartProject.Domain.Dtos.UserProfile;
using CacheSmartProject.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartCacheProject.Application.Services.Interfaces;

namespace CacheSmartProject.Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public UserProfileService(IConfiguration configuration, IMapper mapper)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _mapper = mapper;
    }

    public async Task<int> CreateAsync(UserProfileCreateDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand(@"
            INSERT INTO UserProfiles (Name, Email, Preferences, LastModified)
            VALUES (@Name, @Email, @Preferences, @LastModified);
            SELECT SCOPE_IDENTITY();", connection);

        command.Parameters.AddWithValue("@Name", dto.Name);
        command.Parameters.AddWithValue("@Email", dto.Email);
        command.Parameters.AddWithValue("@Preferences", (object?)dto.Preferences ?? "{}");
        command.Parameters.AddWithValue("@LastModified", DateTime.UtcNow);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand("DELETE FROM UserProfiles WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<List<UserProfileResponseDto>> GetAllAsync()
    {
        var result = new List<UserProfile>();
        using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand("SELECT * FROM UserProfiles", connection);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new UserProfile
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Preferences = reader.GetString(3),
                LastModified = reader.GetDateTime(4)
            });
        }
        return _mapper.Map<List<UserProfileResponseDto>>(result);
    }

    public async Task<UserProfileResponseDto?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand("SELECT * FROM UserProfiles WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var user = new UserProfile
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Preferences = reader.GetString(3),
                LastModified = reader.GetDateTime(4)
            };
            return _mapper.Map<UserProfileResponseDto>(user);
        }
        return null;
    }

    public async Task<bool> UpdateAsync(UserProfileUpdateDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand(@"
        UPDATE UserProfiles SET 
            Name = @Name,
            Email = @Email,
            Preferences = @Preferences,
            LastModified = @LastModified
        WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", dto.Id);
        command.Parameters.AddWithValue("@Name", dto.Name);
        command.Parameters.AddWithValue("@Email", dto.Email);
        command.Parameters.AddWithValue("@Preferences", dto.Preferences);
        command.Parameters.AddWithValue("@LastModified", DateTime.UtcNow);

        await connection.OpenAsync();
        var affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }
}
