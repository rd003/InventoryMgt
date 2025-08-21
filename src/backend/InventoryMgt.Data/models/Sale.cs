using System;
using System.Collections.Generic;

namespace InventoryMgt.Data.Models;

public  class Sale
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int ProductId { get; set; }

    public DateTime SellingDate { get; set; }

    public decimal? Quantity { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual Product Product { get; set; } = null!;
}
