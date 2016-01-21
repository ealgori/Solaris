using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace TaskManager.TaskParamModels
{
    /// <summary>
    /// вся информация о файле, которая может потребоваться
    /// </summary>
    public class FileParams
    {
        public MemoryStream FileStream { get; set; }
        public string FileName { get; set; }
        public Type ObjectType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public FileParams()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}