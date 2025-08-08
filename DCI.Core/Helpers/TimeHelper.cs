namespace DCI.Core.Helpers
{
    public class TimeHelper
    {
        public static string ConvertMinutesToHHMM(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours:D2} hour(s) and {minutes:D2} minute(s)";
        }
    }
}
