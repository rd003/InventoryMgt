using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Shared.DTOs;

public class TokenModel
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

