using System.Runtime.CompilerServices;

namespace DCI.Core.Common
{
    public class Utilities
    {
        public static string GetModuleName(string controllername, [CallerMemberName] string methodName = "")
        {
          return String.Format("{0}_{1}: ", controllername,methodName);            
        }
    }
}
