using System;
using System.Collections.Generic;

namespace InventoryMgt.Data.models;

public  class Purchase
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProductId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal Quantity { get; set; }

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string? PurchaseOrderNumber { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
