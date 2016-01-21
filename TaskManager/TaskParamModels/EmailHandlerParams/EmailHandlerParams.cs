using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.TaskParamModels
{
    public class EmailHandlerParams
    {
        public List<EmailParams> EmailParams { get; set; }
        /// <summary>
        /// Возбуждать исключения, чтобы привести с срыву таска, и импорт не отработает
        /// </summary>

        public EmailHandlerParams()
        {
            EmailParams = new List<EmailParams>();

        }
    }
}
