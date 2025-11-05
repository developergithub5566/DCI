namespace DCI.Core.Helpers
{
    public class FormatHelper
    {
        public static string GetFormattedRequestNo(int totalRecords)
        {
            int setA = totalRecords % 1000;
           // int setB = totalRecords / 1000;
            string formatted = setA.ToString("D4");
           // string formattedB = setB.ToString("D4");
            return $"{formatted}";
        }
    }
}
