using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class UserWishlist
{
    public int UserId { get; set; }

    public int ProductId { get; set; }

    public bool? IsFavourite { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
