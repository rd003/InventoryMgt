using System;
using System.Collections.Generic;

namespace InventoryMgt.Data.Models;

public  class Stock
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProductId { get; set; }

    public decimal Quantity { get; set; }

    public virtual Product? Product { get; set; }
}
