namespace Quanlythuvien.ViewModels
{
    public class BookDetailViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int? YearPublished { get; set; }
        public string PublisherName { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
    }
}
