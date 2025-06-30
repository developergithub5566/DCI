using System.ComponentModel.DataAnnotations.Schema;

namespace DCI.Trigger
{
    [Table("tbl_raw_logs")]
    public class TblRawLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string FULL_NAME { get; set; }
        public DateTime DATE_TIME { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
}
