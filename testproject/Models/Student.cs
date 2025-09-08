using System;
using System.Collections.Generic;

namespace testproject.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public bool? Status { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
