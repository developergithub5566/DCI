namespace DCI.Models.ViewModel
{
    public class RequestorHistoryViewModel
    {
        public int RequestorHistoryId { get; set; } = 0;
        public int DocId { get; set; } = 0;
        public int RequestById { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
