using System;
using System.Collections.Generic;

namespace testproject.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string Author1 { get; set; } = null!;

    public string? Bio { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
