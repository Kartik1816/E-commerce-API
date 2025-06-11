using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpireTime { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
