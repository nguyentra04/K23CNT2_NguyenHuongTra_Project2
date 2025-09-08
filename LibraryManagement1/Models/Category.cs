using System;
using System.Collections.Generic;

namespace LibraryManagement.Models;

public partial class Category
{
    public int CateId { get; set; }

    public string CateName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
