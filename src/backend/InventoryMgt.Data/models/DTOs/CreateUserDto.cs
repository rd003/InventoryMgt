using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.models.DTOs;

public class CreateUserDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Role { get; set; } = string.Empty;
}
