using System;
using System.Collections.Generic;

namespace QuanlythuvienDB.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int? PublisherId { get; set; }

    public int? YearPublished { get; set; }

    public int? Quantity { get; set; }

    public string? ImagePath { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public string? DownloadLink { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Borrowed> Borroweds { get; set; } = new List<Borrowed>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Cates { get; set; } = new List<Category>();
}
