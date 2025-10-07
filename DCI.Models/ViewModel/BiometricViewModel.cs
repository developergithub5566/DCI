namespace DCI.Models.ViewModel
{
    public class BiometricViewModel
    {
        public int UserId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string EmployeeNo { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateTimeInOut { get; set; } 
    }
}
