using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string SupplierName { get; set; } = null!;

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? TaxNumber { get; set; }

    [Range(0, int.MaxValue)]
    public int PaymentTerms { get; set; } = 30;

    public bool IsActive { get; set; }
}
