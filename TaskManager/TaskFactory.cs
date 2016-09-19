using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.TaskModel;
using DbModels.DataContext;
using TaskManager.TaskParamModels;


namespace TaskManager
{
    public static  class TaskFactory
    {
        public static TaskQuantifier TaskQuantifier = new TaskQuantifier();

        public static TaskBase GetTask(DbTaskParams dbTask, Context context)
        {

            //if (dbTask.DbTask != null)
            //{
            //    TaskBase task;
            //    if (dbTask.DbTask.Active)
            //        task = new TaskBase(dbTask.DbTask, dbTask.FileHandlerParams, context);
            //    else
            //    {
            //        //taskLogger.LogWarn(string.Format("Task:{0}; Active state:{1}", dbTask.Name, dbTask.Active));
            //        return null;
            //    }
            //    // если какой либо косяк при заполнении хэндлеров
            //    if (TaskQuantifier.FillHandlers(ref task))
            //    {
            //        return task;
            //    }
            //    return null;
            //}
            //else
            //    return null;

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return GetTask(dbTask, userName, context);
        }

        public static TaskBase GetTaskTest(DbTaskParams dbTask, Context context, TestTaskParams param=null )
        {

            //if (dbTask.DbTask != null)
            //{
            //    TaskBase task;
            //    if (dbTask.DbTask.Active)
            //        task = new TaskBase(dbTask.DbTask, dbTask.FileHandlerParams, context);
            //    else
            //    {
            //        //taskLogger.LogWarn(string.Format("Task:{0}; Active state:{1}", dbTask.Name, dbTask.Active));
            //        return null;
            //    }
            //    // если какой либо косяк при заполнении хэндлеров
            //    if (TaskQuantifier.FillHandlers(ref task))
            //    {
            //        return task;
            //    }
            //    return null;
            //}
            //else
            //    return null;

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return GetTaskTest(dbTask, userName, context,param);
        }


        public static TaskBase GetTask(DbTaskParams dbTask,string userName, Context context)
        {

            if (dbTask.DbTask != null)
            {
                TaskBase task;
                if (dbTask.DbTask.Active)
                    task = new TaskBase(dbTask.DbTask, userName, dbTask.FileHandlerParams, context);
                else
                {
                    //taskLogger.LogWarn(string.Format("Task:{0}; Active state:{1}", dbTask.Name, dbTask.Active));
                    return null;
                }
                // если какой либо косяк при заполнении хэндлеров
                if (TaskQuantifier.FillHandlers(ref task))
                {
                    return task;
                }
                return null;
            }
            else
                return null;

        }

        public static TaskBase GetTaskTest(DbTaskParams dbTask, string userName, Context context,TestTaskParams param)
        {

            if (dbTask.DbTask != null)
            {
                TaskBase task;
                //if (dbTask.DbTask.Active)
                    task = new TaskBase(dbTask.DbTask, userName, dbTask.FileHandlerParams, context,param);
               // else
               // {
                    //taskLogger.LogWarn(string.Format("Task:{0}; Active state:{1}", dbTask.Name, dbTask.Active));
              //      return null;
             //   }
                // если какой либо косяк при заполнении хэндлеров
                if (TaskQuantifier.FillHandlers(ref task))
                {
                    return task;
                }
                return null;
            }
            else
                return null;

        }


        public static TaskBase GetTask(int dbTaskId, string UserName,List<CommonFunctions.CommonClasses.TaskTranceParam> parameters, Context context)
        {
            var dbTask = context.DbTasks.FirstOrDefault(dbt => dbt.Id == dbTaskId);
            if (dbTask != null&&dbTask.Active)
            {
                var task = new TaskBase(dbTask,UserName,null,context);
                task.TaskParameters.FileHandlerParams = new FileHandlerParams();
                task.TaskParameters.TaskHandlerParams = new TaskHandlerParams();
                foreach (var par in parameters)
                {
                    if (par.Case == "FileHandlerParams")
                    {
                        if(par.Type=="File")
                        {
                            task.TaskParameters.FileHandlerParams.StreamParameters.Add(par.Key, new FileParams() { });
                        }
                        else
                        {
                        
                        }
                        //task.TaskParameters.FileHandlerParams.
                    }
                    if (par.Case == "TaskHandlerParams")
                    {

                    }
                }
            }
            return null;
        }

    }
}