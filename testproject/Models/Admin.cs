using System;
using System.Collections.Generic;

namespace testproject.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string? FullName { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
