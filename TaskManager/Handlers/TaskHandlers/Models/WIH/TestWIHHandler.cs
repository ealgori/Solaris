using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using WIHInteract;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class TestWIHHandler : ATaskHandler
    {
        public TestWIHHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

        public override bool Handle()
        {
            var mailInf = MailInfoFactory.GetGRInfo("TEST", @"C:\Temp\MS14082Fix.log");
            var result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS", true);
            TaskParameters.TaskLogger.LogInfo(result);
            return true;
        }
    }
}
