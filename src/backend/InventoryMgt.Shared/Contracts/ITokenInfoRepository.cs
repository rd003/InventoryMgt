using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Contracts;

public interface ITokenInfoRepository
{
    Task<ReadTokenInfoDto?> GetTokenInfoByUsernameAsync(string username);
    Task<ReadTokenInfoDto> AddTokenInfoAsync(CreateTokenInfoDto tokenInfoToCreate);
    Task UpdateTokenInfoAsync(UpdateTokenInfoDto tokenInfoToUpdate);
    Task DeleteTokenInfoByUsername(string username);
}
