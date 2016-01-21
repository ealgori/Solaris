using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonFunctions
{
    public class CommonClasses
    {
        /// <summary>
        /// класс испльзующихся для передачи параметров для таска от сайта сервису
        /// </summary>
        public class TaskTranceParam
        {
            public string Key { get; set; }
            public string Type { get; set; }
            public object Value { get; set; }
            public string Case { get; set; }
        }
    }
}
