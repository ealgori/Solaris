using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace TaskManager.TaskParamModels
{
    public class ImportParams
    {
        public string ImportFileNearlyName { get; set; }
        public ArrayList Objects { get; set; }
    }
}