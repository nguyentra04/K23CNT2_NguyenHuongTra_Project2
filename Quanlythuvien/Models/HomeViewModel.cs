namespace Quanlythuvien.ViewModels
{
    public class HomeViewModel
    {
        public List<NewsViewModel> NewsList { get; set; } = new List<NewsViewModel>();
        public List<EventViewModel> EventList { get; set; } = new List<EventViewModel>();
    }

   

    public class NewsViewModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string? Summary { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImagePath { get; set; }
    }

    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImagePath { get; set; }
    }
}
