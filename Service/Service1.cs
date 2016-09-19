using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DbModels.DomainModels.DbTasks;
using DbModels.DataContext;

namespace Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public static Task ShCloneTask = Task.Factory.StartNew(() => {
            try
            {
                ShClone.Model.SHCloneBulkCopy.Instance.DoWork();
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{exc.Message} - {exc.InnerException.Message}");
                throw;
            }
           

        });
        public static Task TaskManagerTask = Task.Factory.StartNew(() => {TaskManager.TaskManager.Instance.ProcessTasks(); });
        public string GetStatus()
        {
            return string.Format("ShCloneStatus:{0}; TaskManagerStatus:{1}",ShCloneTask.Status.ToString(), TaskManagerTask.Status.ToString());
        }

        public List<int>  GetActiveTasks()
        {
            return TaskManager.TaskManager.Instance.GetActiveTaskLogs();
        }
        public Service1()
        {
            
          
        }

        public int AddTask(int dbTaskId,string fileName, byte[] file)
        {
            TaskManager.TaskParamModels.FileHandlerParams fileHandlerParams = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                // параметры файла
                TaskManager.TaskParamModels.FileParams fileParams = new TaskManager.TaskParamModels.FileParams();
                fileParams.FileName = fileName;
                MemoryStream stream = new MemoryStream(file);
                stream.Seek(0, SeekOrigin.Begin);
                fileParams.FileStream = stream;

                //параметры файлов
                fileHandlerParams = new TaskManager.TaskParamModels.FileHandlerParams();
                fileHandlerParams.StreamParameters.Add("File", fileParams);

            }
            TaskManager.TaskParamModels.DbTaskParams dbTaskParams = new TaskManager.TaskParamModels.DbTaskParams();
            dbTaskParams.DbTask = (TaskManager.TaskManager.Instance.Context).DbTasks.FirstOrDefault(dbt=>dbt.Id== dbTaskId); // dbTask;
            dbTaskParams.FileHandlerParams = fileHandlerParams;
            
            return TaskManager.TaskManager.Instance.AddDbTask(dbTaskParams);



        }

        //public int AddTask(int dbTaskId, string UserName, List<CommonFunctions.CommonClasses.TaskTranceParam> parameters)
        //{
        //    //return TaskManager.TaskManager.Instance.AddDbTask(dbTaskId, UserName, parameters);
        //    return 0;
        //}

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
