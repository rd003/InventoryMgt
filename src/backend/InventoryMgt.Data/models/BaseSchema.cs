
namespace InventoryMgt.Data.Models;

public class BaseSchema
{
    public int Id { get; set; }

    // These things are getting generated automatically, so they are not needed
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
}