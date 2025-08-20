using System;
using System.Collections.Generic;

namespace InventoryMgt.Data.Models;

public class Category
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Category> InverseCategoryNavigation { get; set; } = new List<Category>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
