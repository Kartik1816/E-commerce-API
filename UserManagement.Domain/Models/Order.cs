using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public decimal? Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
