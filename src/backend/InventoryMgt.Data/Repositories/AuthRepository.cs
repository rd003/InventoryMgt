using InventoryMgt.Shared.CustomExceptions;
using InventoryMgt.Data.Mappers;
using InventoryMgt.Data.Models;
using InventoryMgt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Shared.Repositories;

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

    public async Task<UserReadDto> LoginAsync(LoginDto loginData)
    {
        var user = await _context.Users
               .AsNoTracking()
               .SingleOrDefaultAsync(u => u.Username == loginData.Username);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credential");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credential");
        }

        return user.ToUserReadDto();
    }

    public async Task<UserReadDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Username == username);
        if (user == null) return default;
        return user.ToUserReadDto();
    }

}
