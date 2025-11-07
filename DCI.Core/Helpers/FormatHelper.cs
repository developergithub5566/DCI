namespace DCI.Core.Helpers
{
    public class FormatHelper
    {
        public static string GetFormattedRequestNo(int totalRecords)
        {
            int setA = totalRecords % 1000;          
            string formatted = setA.ToString("D4");          
            return $"{formatted}";
        }

        public static string GetLeaveTypeName(int leavetypeid)
        {
            if (leavetypeid <= 4 )
                return "Leave";
            else if (leavetypeid == 5)
                return "Special Leave";
            else if (leavetypeid == 6)
                return "Maternity Leave";
            else if (leavetypeid == 7)
                return "Paternity Leave";
            else if (leavetypeid == 8 || leavetypeid == 15)
                return "Official Business";
            else if (leavetypeid == 9 || leavetypeid == 10)
                return "Monetization";
            else if (leavetypeid == 11)
                return "Undertime";
            else if (leavetypeid == 12)
                return "Late";
            else { return "Others"; }
        }
    }
}
