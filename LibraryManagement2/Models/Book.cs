using System;
using System.Collections.Generic;

namespace LibraryManagement2.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int? AuthorId { get; set; }

    public string? Publisher { get; set; }

    public int? YearPublished { get; set; }

    public int? CateId { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public virtual Author? Author { get; set; }

    public virtual ICollection<Borrowed> Borroweds { get; set; } = new List<Borrowed>();

    public virtual Category? Cate { get; set; }
}
