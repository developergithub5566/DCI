using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class WfhApplication : IAuditable
    {
        [Key]
        public int Id { get; set; }


        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }


        public string? First_In { get; set; }


        public string? Last_Out { get; set; }


        public string? Late { get; set; }
        public string? Clock_Out { get; set; }


        public string? Under_Time { get; set; }

        public string? Overtime { get; set; }


        public string? Total_Hours { get; set; }


        public string? Total_Working_Hours { get; set; }

        public int? Status { get; set; }
    }
}
