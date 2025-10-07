namespace Quanlythuvien.ViewModels
{
    public class HomeBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; } 
        public string PublisherName { get; set; } 
        public int? YearPublished { get; set; }
        public string ImagePath { get; set; }
        public string CateName { get; set; }

        public HomeBookViewModel()
        {
            Authors = new List<string>();
        }
    }
}