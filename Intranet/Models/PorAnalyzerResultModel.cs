using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbModels.Models;

namespace Intranet.Models
{
    public class PorAnalyzerResultModel
    {
        public PorAnalyzerResultModel()
        {
            Errors = new List<string>();
        }
        public SubContractor Subcontractor { get; set; }
       // public SubContractor SourceSubontractor { get; set; }
        public List<CompareReportModel> CompareReportModelList { get; set; }
        public List<string> UnApprovedRevisions { get; set; }
        public List<string> UncomparablePLS { get; set; }
        public List<string> UncomparableItems { get; set; }
        public List<string> Errors { get; set; }
    }
}