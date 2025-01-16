using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

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

		public static bool IsMinCharacter(int minChar , string input)
		{
			return minChar > input.Length;
		}

		public static bool IsMaxCharacter(int maxChar, string input)
		{
			return maxChar < input.Length;
		}

		public static bool IsContainsNumber(string input) 
		{ 
			return input.Any(char.IsDigit);
		}

		public static bool IsContainsLowerCase(string input)
		{
			return input.Any(char.IsLower);
		}

		public static bool IsContainsUpperCase(string input)
		{
			return input.Any(char.IsUpper);
		}		
	}
}
