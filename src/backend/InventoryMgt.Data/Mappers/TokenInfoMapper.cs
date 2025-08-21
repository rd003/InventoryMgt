using InventoryMgt.Data.Models;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Data.Mappers;

public static class TokenInfoMapper
{
   public static ReadTokenInfoDto ToReadTokenInfoDto(this TokenInfo tokenInfo)
    {
        return new ReadTokenInfoDto
        {
            Id=tokenInfo.Id,
            Username=tokenInfo.Username,
            RefreshToken=tokenInfo.RefreshToken,
            ExpiredAt=tokenInfo.ExpiredAt
        };
    }

    public static UpdateTokenInfoDto ToUpdateTokenInfoDto(this ReadTokenInfoDto tokenInfo)
    {
        return new UpdateTokenInfoDto
        {
            Id = tokenInfo.Id,
            Username = tokenInfo.Username,
            RefreshToken = tokenInfo.RefreshToken,
            ExpiredAt = tokenInfo.ExpiredAt
        };
    }


    public static TokenInfo ToTokenInfo(this CreateTokenInfoDto tokenInfo)
    {
        return new TokenInfo
        {
            Username = tokenInfo.Username,
            RefreshToken = tokenInfo.RefreshToken,
            ExpiredAt = tokenInfo.ExpiredAt
        };
    }

    public static TokenInfo ToTokenInfo(this UpdateTokenInfoDto tokenInfo)
    {
        return new TokenInfo
        {
            Id = tokenInfo.Id,
            Username = tokenInfo.Username,
            RefreshToken = tokenInfo.RefreshToken,
            ExpiredAt = tokenInfo.ExpiredAt
        };
    }
}
