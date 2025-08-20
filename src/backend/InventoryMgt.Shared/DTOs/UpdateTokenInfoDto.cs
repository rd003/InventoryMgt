using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Shared.DTOs;

public class UpdateTokenInfoDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)] public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiredAt { get; set; }

}
