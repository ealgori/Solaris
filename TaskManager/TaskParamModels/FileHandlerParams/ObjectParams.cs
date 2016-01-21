using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace TaskManager.TaskParamModels
{
    public class ObjectParams
    {
        public ArrayList Objects { get; set; }
       // public string FileName { get; set; }
        public Type ObjectType { get; set; }
        public Dictionary<string, object> ObjectParameters { get; set; }

        public ObjectParams()
        {
            ObjectParameters = new Dictionary<string, object>();
            Objects = new ArrayList();
        }
    }
}