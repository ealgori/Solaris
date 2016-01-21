using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.TaskParamModels
{
    public class ImportHandlerParams
    {
        public List<ImportParams> ImportParams { get; set; }

        public ImportHandlerParams()
        {
            ImportParams = new List<ImportParams>();
        }
    }
}