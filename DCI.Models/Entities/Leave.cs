namespace DCI.Models.Entities
{
    public class Leave
    {
        public int LeaveId { get; set; }


        public DateTime DateFiled { get; set; }


        public int LeaveType { get; set; }


        public string Reason { get; set; }

        public DateTime? DateModified { get; set; }


        public string? ModifiedBy { get; set; }


        public bool IsActive { get; set; }
    }
}
