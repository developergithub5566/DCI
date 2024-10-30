namespace DCI.Models.ViewModel
{
	public class PermissionViewModel
	{
		public bool CanView { get; set; } = false;
		public bool CanAdd { get; set; } = false;
		public bool CanEdit { get; set; } = false;
		public bool CanDelete { get; set; } = false;
	}
}
