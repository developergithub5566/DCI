using System.Runtime.CompilerServices;

namespace DCI.Core.Common
{
	public class Utilities
	{
		public static string GetModuleName(string controllername, [CallerMemberName] string methodName = "")
		{
			return String.Format("{0}_{1}: ", controllername, methodName);
		}
		public static string GetUsernameByEmail(string email)
		{
			try
			{
				int atIndex = email.IndexOf('@');
				
				if (atIndex >= 0)
				{
					return email.Substring(0, atIndex);					
				}
				else
				{
					return string.Empty;
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
			
		}
	}
}
