namespace DCI.Models.ViewModel
{
	public class LoginViewModel
	{
		public int UserId { get; set; } = 0;
		public int UserAccessId { get; set; } = 0;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
