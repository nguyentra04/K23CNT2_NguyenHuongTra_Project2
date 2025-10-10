﻿namespace Quanlythuvien.Models
{
    public partial class Author
    {
        public int AuthorId { get; set; }

        public string AuthorName { get; set; } = null!;
        public string? Bio { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}