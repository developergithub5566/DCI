using DCI.Models.Configuration;

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
    }
}
