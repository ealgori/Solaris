using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class EmptyAVRDistHandlerResponsibles:ATaskHandler
    {
        public EmptyAVRDistHandlerResponsibles(TaskParameters taskParams) : base(taskParams)
        {
          
        }

        public override bool Handle()
        {
            var handler = new EmptyAVRDistrHandler(TaskParameters, "responsibles");
            handler.Handle();
            return true;
        }
    }
}
