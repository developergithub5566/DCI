namespace DCI.Models.ViewModel
{
	public class JobApplicationViewModel
	{
		public int JobApplicantId { get; set; } = 0;
		public string ApplicantName { get; set; } = string.Empty;
		public string ContactNumber { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime? DateofBirth { get; set; } = null;
		public string PositionOffer { get; set; } = string.Empty;
		public int JobSite { get; set; } = 0;
		public int Status { get; set; } = 0;
		public string StatusName { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime DateModified { get; set; } = DateTime.Now;
		public int ModifiedBy { get; set; } = 0;
		public bool IsActive { get; set; } = true;
		public int ActionType { get; set; } = 0;
	}
}
