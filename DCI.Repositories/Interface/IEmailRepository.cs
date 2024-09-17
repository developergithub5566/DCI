namespace DCI.Repositories.Interface
{
	public interface IEmailRepository : IDisposable
	{
		Task<bool> IsExistsEmail(string email);
		Task SendPasswordReset(string email);
		Task SendUploadFile(string email, string docno);
	}
}
