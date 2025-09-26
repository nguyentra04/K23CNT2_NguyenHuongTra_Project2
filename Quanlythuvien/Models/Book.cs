using System;
using System.Collections.Generic;

namespace Quanlythuvien.Models;

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

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}

public class BookAuthor
{
    public int BookId { get; set; }
    public Book Book { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; }
}
