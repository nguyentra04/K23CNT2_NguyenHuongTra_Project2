using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlythuvien.Models
{
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

        [StringLength(255)]
        public string? DownloadLink { get; set; }

        public bool? Status { get; set; } = true;

        [ForeignKey("PublisherId")]
        public virtual Publisher? Publisher { get; set; }

        public virtual ICollection<Borrowed> Borroweds { get; set; } = new List<Borrowed>();

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>(); 
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}