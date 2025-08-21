using InventoryMgt.Data.Mappers;
using InventoryMgt.Data.Models;
using InventoryMgt.Shared.Contracts;
using InventoryMgt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Data.Repositories;

public class TokenInfoRepository : ITokenInfoRepository
{
    private readonly AppDbContext _context;

    public TokenInfoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReadTokenInfoDto?> GetTokenInfoByUsernameAsync(string username)
    {
        var tokenInfo = await _context.TokenInfos.AsNoTracking().SingleOrDefaultAsync(t => t.Username == username);
        return tokenInfo == null ? default : tokenInfo.ToReadTokenInfoDto();
    }

    public async Task<ReadTokenInfoDto> AddTokenInfoAsync(CreateTokenInfoDto tokenInfoToCreate)
    {
        var tokenInfo = tokenInfoToCreate.ToTokenInfo();
        _context.TokenInfos.Add(tokenInfo);
        await _context.SaveChangesAsync();
        return new ReadTokenInfoDto
        {
            Id = tokenInfo.Id,
            Username = tokenInfo.Username,
            ExpiredAt = tokenInfo.ExpiredAt,
            RefreshToken = tokenInfo.RefreshToken
        };
    }

    public async Task UpdateTokenInfoAsync(UpdateTokenInfoDto tokenInfoToUpdate)
    {
        var tokenInfo = tokenInfoToUpdate.ToTokenInfo();
        _context.TokenInfos.Update(tokenInfo);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteTokenInfoByUsername(string username)
    {
        await _context.TokenInfos.Where(t => t.Username == username).ExecuteDeleteAsync();
    }
}
