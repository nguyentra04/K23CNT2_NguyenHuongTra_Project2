using System;
using System.Collections.Generic;

namespace testproject.Models;

public partial class Librarian
{
    public int LibraId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool? Status { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
