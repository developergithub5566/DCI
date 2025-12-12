using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCI.Models.Entities
{
    public class tbl_wfh_logs : IAuditable
    {
        public int ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FULL_NAME { get; set; }
        public DateTime DATE_TIME { get; set; }
        public DateTime CREATED_DATE {  get; set; }
        public string CREATED_BY { get; set; }
        public int STATUS { get; set; }
        [Column(TypeName = "decimal(18,10)")]
        public decimal LATITUDE { get; set; }
        [Column(TypeName = "decimal(18,10)")]
        public decimal LONGITUDE { get; set; }
    }
}
