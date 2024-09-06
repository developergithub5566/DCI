using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.Entities
{
	public class JobApplicant
	{
		public int JobApplicantId { get; set; } = 0;
		public string ApplicantName { get; set; } = string.Empty;
		public string ContactNumber { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime? DateofBirth { get; set; } = DateTime.Now;
		public string PositionOffer { get; set; } = string.Empty;
		public int JobSite { get; set; } = 0;
		public int Status { get; set; } = 0;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
	}
}
