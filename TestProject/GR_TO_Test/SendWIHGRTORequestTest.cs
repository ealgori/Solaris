using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.TaskParamModels;
using DbModels.DataContext;

using System.Linq;
using TaskManager;

namespace TestProject.GR_TO_Test
{
    [TestClass]
    public class SendWIHGRTORequestTest
    {
        [TestMethod]
        public void SendWIHGRTORequestsHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendWIHGRTORequestsHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }
    }
}
