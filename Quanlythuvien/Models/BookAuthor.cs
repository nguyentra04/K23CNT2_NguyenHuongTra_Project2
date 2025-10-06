namespace Quanlythuvien.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public virtual Book Book { get; set; } = null!;
        public virtual Author Author { get; set; } = null!;
    }
    }