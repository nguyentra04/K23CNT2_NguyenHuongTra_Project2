namespace LibraryManagement2.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public DateTime Timestamp { get; set; } 
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}