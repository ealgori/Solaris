using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.TaskParamModels
{
    public class FileHandlerParams
    {
        public Dictionary<string, FileParams> StreamParameters { get; set; }
        public Dictionary<string, ObjectParams> ObjectParameters { get; set; }
        public FileHandlerParams()
        {
            StreamParameters = new Dictionary<string, FileParams>();
            ObjectParameters = new Dictionary<string, ObjectParams>();
        }
    }
}