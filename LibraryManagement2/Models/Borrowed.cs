using LibraryManagement2.Models.Entities;
using System;
using System.Collections.Generic;

namespace LibraryManagement2.Models;

public partial class Borrowed
{
    public int BorrowId { get; set; }

    public int? StudentId { get; set; }

    public int? BookId { get; set; }

    public DateOnly? BorrowDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public int? LibraId { get; set; }

    public bool? Status { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Librarian? Libra { get; set; }

    public virtual Student? Student { get; set; }
}
