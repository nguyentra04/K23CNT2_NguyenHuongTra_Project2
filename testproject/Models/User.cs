using System;
using System.Collections.Generic;

namespace testproject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Librarian? Librarian { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
