using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{
    public class ReportGraphViewModel
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
    }
    public class ReportGraphsDataViewModel
    {
        public IList<ReportGraphViewModel> StatusData { get; set; } = new List<ReportGraphViewModel>();
        public IList<ReportGraphViewModel> TypeData { get; set; } = new List<ReportGraphViewModel>();
    }

    //public class ReportGraphStatusViewModel
    //{
    //    public string Doc { get; set; } = string.Empty;
    //    public int Count { get; set; } = 0;
    //}
    //public class ReportGraphDocTypeViewModel
    //{
    //    public string Doc { get; set; } = string.Empty;
    //    public int Count { get; set; } = 0;
    //}
}
