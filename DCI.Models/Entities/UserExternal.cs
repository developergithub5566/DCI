using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
	public class UserExternal : IAuditable
	{
		public int UserExternalId { get; set; }
		public int UserId { get; set; }
		public string Provider { get; set; }
		public string Identifier { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.Now;

		public virtual User User { get; set; }
	}
}
