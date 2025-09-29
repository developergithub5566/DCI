namespace DCI.Models.ViewModel
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; } = 0;
        public string EmployeeNo { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public bool IsResigned { get; set; } = false;
    }
}
