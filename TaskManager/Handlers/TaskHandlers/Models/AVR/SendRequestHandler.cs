using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using MailProcessing;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class SendRequestHandler:ATaskHandler
    {
        public SendRequestHandler(TaskParameters taskParameters):base(taskParameters)
        {

        }
        #region template
       

        #endregion
     

        public override bool Handle()
        {
         
            var interact = new RedemptionMailProcessor("SOLARIS");
            List<ShVCRequestImport> requestList = new List<ShVCRequestImport>();
            var requests = TaskParameters.Context.ShVCRequests.Where(r => r.SendRequest == true && !r.RequestSend.HasValue).ToList();
            foreach (var request in requests)
	        {
                var expectedMailName = Path.GetFileName(request.Attachments) + ".msg";
                var mailPath = Path.Combine(request.Attachments,expectedMailName);
                if(File.Exists(mailPath))
                {
                    var result = interact.SendMailByTemplate(mailPath);
                    if(!string.IsNullOrEmpty(result))
                    {
                        ShVCRequestImport model = new ShVCRequestImport() { Id = request.Id, RequestSend=DateTime.Now };
                        requestList.Add(model);
                        // запишем в схклон
                        request.RequestSend = DateTime.Now;


                    }
                }
	        }


                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
            return true;
        }
    }
}
