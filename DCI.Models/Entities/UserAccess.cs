using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCI.Models.Entities
{
    public class UserAccess : IAuditable
    {
        public int UserAccessId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public string? Password { get; set; } 
        public DateTime DateCreated { get; set; } = DateTime.Now;
		public bool IsActive { get; set; }
        public string? PasswordResetToken { get; set; } = null;
        public DateTime? PasswordResetTokenExpiry { get; set; } = null;
        public DateTime? ModifiedDate { get; set; }    

	}
}
