using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class OrderProduct
{
    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public string? CategoryName { get; set; }

    public string? ProductName { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
