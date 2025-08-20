using InventoryMgt.Data.models;
using InventoryMgt.Data.models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Data.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SignupAsync(CreateUserDto userToCreate)
    {
        var user = new User
        {
            FullName = userToCreate.FullName,
            Username = userToCreate.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userToCreate.Password),
            Role = userToCreate.Role
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task LoginAsync(LoginDto loginData)
    {
        var user = await _context.Users.AsNoTracking().SingleAsync(u => u.Username == loginData.Username);
        if (!BCrypt.Net.BCrypt.Verify(loginData.Password, loginData.Password))
        {
            // create 401 exception
        }
    }

}
