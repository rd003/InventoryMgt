using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Repositories;

public interface IAuthRepository
{
    Task<int> SignupAsync(CreateUserDto userToCreate);
    Task<UserReadDto> LoginAsync(LoginDto loginData);
    Task<UserReadDto?> GetUserByUsernameAsync(string username);
}
