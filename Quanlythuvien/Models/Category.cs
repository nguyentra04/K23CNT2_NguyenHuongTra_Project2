namespace Quanlythuvien.Models
{
    public partial class Category
    {

        public int CateId { get; set; }
        public string CateName { get; set; } = null!;

        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}