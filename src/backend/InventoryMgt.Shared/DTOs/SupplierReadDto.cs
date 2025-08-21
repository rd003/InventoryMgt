
namespace InventoryMgt.Shared.DTOs;

public class SupplierReadDto
{
    public int Id { get; set; }
    public string SupplierName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxNumber { get; set; }
    public int PaymentTerms { get; set; }
    public bool IsActive { get; set; }
}
