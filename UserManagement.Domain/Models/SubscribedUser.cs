using System;
using System.Collections.Generic;

namespace UserManagement.Domain.Models;

public partial class SubscribedUser
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;
}
