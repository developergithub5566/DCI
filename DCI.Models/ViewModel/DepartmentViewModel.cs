namespace DCI.Models.ViewModel
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        public string DepartmentCode { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

		public DateTime DateCreated { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = new DateTime();

        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;
		public string CreatedName { get; set; } = string.Empty;
	}
}
