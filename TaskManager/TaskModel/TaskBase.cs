using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Diagnostics;

using System.IO;

using System.Collections;
using DbModels.DomainModels.DbTasks;
using TaskManager.TaskParamModels;
using DbModels.DataContext;
using TaskManager.Handlers.FileIOHandlers;
using TaskManager.Handlers.ImportHandler;
using TaskManager.Handlers.TaskHandlers;
using TaskManager.Handlers.ConvertHandlers;
using System.Data.Entity;
using TaskManager.Handlers.EmailHandlers.Abstract;
using TaskManager.Handlers.ImportHandlers;
using DbModels.DomainModels.ShClone;



namespace TaskManager.TaskModel
{
    public  class TaskBase
    {
        
        #region Необходимые своства
        public TaskParameters TaskParameters { get; set; }
        public ATaskHandler TaskHandler { get; set; }
        public AFileIOHandler FileIOSubHandler { get; set; }
        public AImportHandler ImportHandler {get;set;}

        public AEmailHandler EmailHandler { get; set; }
        public AConvertSubHandler ConvertHandler { get; set; }

        #endregion

        #region общие свойства
        //public FileSubHandlerParams Parameters { get; set; }
        #endregion

        public TaskBase(DbTask dbTask, string userName, FileHandlerParams parameters, Context context)
        {
           
            
            context.Entry(dbTask).State = EntityState.Unchanged;
            var taskLog = new TaskLog() { DbTask = dbTask,UserName=userName, Status="Created" };
            context.TaskLogs.Add(taskLog);
            context.SaveChanges();
            TaskParameters = new TaskParameters();
            TaskParameters.TaskLog = taskLog;
            this.TaskParameters.DbTask = dbTask;
            this.TaskParameters.Context = context;
            this.TaskParameters.FileHandlerParams = parameters;
            this.TaskParameters.TaskLogger = new TaskLogger(taskLog, context) { TaskName=dbTask.Name};
        }

   


        public void Process()
        {
            if (TaskHandler == null)
            {
                TaskParameters.TaskLogger.LogError(string.Format("Пустой хэндлер для таска {0}", TaskParameters.DbTask == null ? "Пустой таск" : TaskParameters.DbTask.Name));
            }
            else
            {
                ShCloneUpdateLog log = new ShCloneUpdateLog();
                log.Task = TaskParameters.DbTask;
                log.StartTime = DateTime.Now;
                try
                {
                TaskParameters.TaskLog.Status = "Started";
                // результат файлового хендлера
               
                if (FileIOSubHandler != null)
                {
                    // если в обработка файлов не нужна, но нужны какие то парамтеры с формы, то надо использовать мокфайлъхэндлер
                    FileIOSubHandler.Handle();
                }
                // Основная процедура обработки. 
                
                TaskHandler.Handle();
                // если есть потребность в конвертировании, то делаем его
                if (ConvertHandler != null)
                {
                   ConvertHandler.Handle();
                }
                // хендлер импорта. конверт и импорт принимают теже данные

                if (EmailHandler != null)
                {
                    EmailHandler.SendMails();
                }

                if (ImportHandler != null)
                {
                    ImportHandler.Import();
               }
                TaskParameters.TaskLog.Status = "Finished";
                log.Success = true;
                }
                catch (System.Exception ex)
                {
                    TaskParameters.TaskLog.Status = "Error";
                    TaskParameters.TaskLogger.LogError(ex.Message + (ex.InnerException == null ? "" : ex.InnerException.Message));
                    log.Comment = (ex.Message + (ex.InnerException == null ? "" : ex.InnerException.Message));
                }
                log.EndTime = DateTime.Now;
                TaskParameters.Context.ShCloneUpdateLogs.Add(log);
                TaskParameters.Context.SaveChanges();


            }
           
        }






    }
}
