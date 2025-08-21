using InventoryMgt.Data.Models;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Data.Mappers;

public static class UserMapper
{
    public static UserReadDto ToUserReadDto(this User user)
    {
        return new UserReadDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Role = user.Role
        };
    }
}
