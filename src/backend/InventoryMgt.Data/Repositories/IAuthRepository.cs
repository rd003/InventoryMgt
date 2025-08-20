using InventoryMgt.Data.models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface IAuthRepository
{
    Task<int> SignupAsync(CreateUserDto userToCreate);
    Task LoginAsync(LoginDto loginData);
}
