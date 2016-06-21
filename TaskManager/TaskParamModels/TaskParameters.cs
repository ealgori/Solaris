using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbModels.DomainModels.DbTasks;
using DbModels.DataContext;


namespace TaskManager.TaskParamModels
{
    public class TaskParameters
    {
        #region GeneralProps
        public DbTask DbTask { get; set; }
        public Context Context { get; set; }
        public TaskLog TaskLog { get; set; }
        public TaskLogger TaskLogger { get; set; }

        #endregion

        #region HandlersParams
        public TaskHandlerParams TaskHandlerParams { get; set; }
        public FileHandlerParams FileHandlerParams { get; set; }
        public ConvertHandlerParams ConvertHandlerParams { get; set; }
        public ImportHandlerParams ImportHandlerParams { get; set; }

        public EmailHandlerParams EmailHandlerParams { get; set; }

       
        #endregion

        public TaskParameters()
        {
            TaskHandlerParams = new TaskHandlerParams();
            FileHandlerParams = new FileHandlerParams();
            ConvertHandlerParams = new ConvertHandlerParams();
            ImportHandlerParams = new ImportHandlerParams();
            EmailHandlerParams = new EmailHandlerParams();

        }
    }
}