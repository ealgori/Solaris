using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Diagnostics;

using System.Data;
using TaskManager.TaskModel;
using TaskManager.TaskParamModels;
using DbModels.DataContext;
using DbModels.DomainModels.DbTasks;
using System.Threading;
using CommonFunctions.Extentions;
using CommonFunctions;
using MailProcessing;



using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TaskManager
{
    public class TaskManager
    {

        #region Singleton
        private static volatile TaskManager instance;
        private static object syncRoot = new Object();
        
        private TaskManager() { }

        public static TaskManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TaskManager();
                    }
                }

                return instance;
            }
        }


        #endregion
        public  Logger logger = LogManager.GetCurrentClassLogger();
        private readonly object _locker = new object();
        #region Task Add Remove
        /// <summary>
        /// Наблюдаем за активными тасками. паттерн наблюдатель. Создаваемы таски автоматически отмечаются здесь
        /// </summary>
        /// 
        public volatile List<TaskBase> TasksList = new List<TaskBase>();
        /// <summary>
        /// получаем список активных тасков
        /// </summary>
        /// <returns></returns>
        public List<int> GetActiveTaskLogs()
        {
            return TasksList.Select(tl => tl.TaskParameters.TaskLog.Id).ToList();
        }
      

        /// <summary>
        /// Флаг готовности. Для обеспечения целостности информации, новая итерацию будет начинаться только тогда, когда все таски были выполенны.
        /// </summary>
        public bool Ready { 
            get 
            {
               
                if (TasksList != null)
                {
                    if (TasksList.Count == 0)
                    {
                       
                        return true;
                    }
                    else
                        return false;

                }
                else
                {
                    return false;
                }
            } 
            private set { } }

        /// <summary>
        /// Добавиить таск в список для выполеннеия. Таски добавляются в конструкторе ТаскБэйз
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool AddTask(TaskBase task)
        {
            if (task != null)
            {
                if (task.TaskParameters.DbTask != null)
                {

                    lock (_locker)
                    {


                        if (TasksList == null)
                            TasksList = new List<TaskBase>();
                        TasksList.Add(task);
                        LogInfo(string.Format("----->>> Добавлен таск:{0}", task.TaskParameters.DbTask.Description));
                        return true;
                    }


                }
                else
                {
                    LogError("Попытка добавить в очередь таск с налловским дбтаском");
                    return false;
                }
            }
            else
            {
                LogError(string.Format("Попытка добавить пустой таск . Конструктор не вернул экземпляр таска. неверные настройки в хендлерах."));
                return false;
            }
        }

        //public bool AddTasks(List<TaskBase> tasks)
        //{
        //    lock (_locker)
        //    {
        //        foreach (var task in tasks)
        //        {
        //                AddTask(task);
        //        }
        //        return true;
        //    }
        //}

        public int AddDbTask(DbTaskParams dbTask)
        {
         //   lock (_locker)
            {
                TaskBase task;
                if ((task = TaskFactory.GetTask(dbTask, Context)) != null)
                {
                     AddTask(task);
                     return task.TaskParameters.TaskLog.Id;
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool AddDbTasks(List<DbTaskParams> dbTasks)
        {
          //  lock (_locker)
            {
                foreach (var task in dbTasks)
                {

                    var taskBase = TaskFactory.GetTask(task, Context);
                    AddTask(taskBase);
                }
                return true;
            }
        }

        public int AddDbTask(int dbTaskId, string UserName, List<CommonFunctions.CommonClasses.TaskTranceParam> parameters)
        {
            //   lock (_locker)
            {
                TaskBase task;
                if ((task = TaskFactory.GetTask(dbTaskId, UserName,parameters, Context)) != null)
                {
                    AddTask(task);
                    return task.TaskParameters.TaskLog.Id;
                }
                else
                {
                    return -1;
                }
            }
        }

        public void SheduleDbUpdate(List<DataTable> objects)
        {
            // while (true)
            {
                //  Intranet.Projects.Tasks.TaskManager.Instance.ALLOWTASKADD = false;
                // if (Intranet.Projects.Tasks.TaskManager.Instance.TasksList.Count==0)
                {
                    //PreparedExtractedFilesToLists(context);
                    //DBLOCK = true;
                    try
                    {

                        var shUpdateDbTask = Context.DbTasks.FirstOrDefault(t => t.Name == "ShCloneUpdate" && t.Active == true);
                        if (shUpdateDbTask == null)
                        {
                            logger.Error("В базе отсутствует дбтаск для обновления базы схклона");
                            return;
                        }
                        else
                        {
                            List<DbTaskParams> tasks = new List<DbTaskParams>();
                            // таск обновления базы
                            #region ShUpdate
                            FileHandlerParams parameters = new FileHandlerParams();
                            ObjectParams oParams = new ObjectParams();
                            oParams.Objects.Add(objects);
                            parameters.ObjectParameters.Add("Objects", oParams);



                            lock (_locker)
                            {
                                tasks.Add(new DbTaskParams() { DbTask = shUpdateDbTask, FileHandlerParams = parameters });
                                //Context.Entry(shUpdateDbTask).State = System.Data.EntityState.Detached;
                            #endregion
                                // тут мы еще не знаем, успешно ли обновление сх клона. поэтому тригерные таски будут добавляться в хендлере схклона
                                // таски помеченные как триггерные
                                #region TriggeredTasks
                                var dbTasks = Context.DbTasks.Where(dt => dt.Triggered == true && dt.Active == true && dt.OperationalType == "onupdate").OrderBy(dbt => dbt.Order).ToList();
                                foreach (var dbtask in dbTasks)
                                {

                                    if (ShouldStart(dbtask))
                                        tasks.Add(new DbTaskParams() { DbTask = dbtask });
                                    // Context.Entry(dbtask).State = System.Data.EntityState.Detached;
                                }

                                #endregion

                                TaskManager.Instance.AddDbTasks(tasks);
                            }
                        }

                    }
                    catch (System.Exception ex)
                    {
                        LogError(ex.Message);
                        if (ex.InnerException != null)
                        {
                            LogError(ex.InnerException.Message);
                        }

                    }




                }

            }
        }

        public bool ShouldStart(DbTask dbtask, DateTime? currentDateTime = null)
        {

            if (!currentDateTime.HasValue)
            {
                currentDateTime = DateTime.Now;
            }
            // если не указан интервал, то всегда срабатывать
            if (!string.IsNullOrEmpty(dbtask.Interval) && (dbtask.StartDate.HasValue))
            {
                TimeSpan span = new TimeSpan();
                // если непонятное значение в таймспан, то сработать
                if (TimeSpan.TryParse(dbtask.Interval, out span))
                {
                    if (dbtask.StartDate.Value > currentDateTime)
                        return false;
                    //var test = Context.ShCloneUpdateLogs.Where(l => l.Task.Id == dbtask.Id && l.EndTime.HasValue).OrderByDescending(i => i.Id).ToList();
                    //var test1 = Context.ShCloneUpdateLogs.Where(l => l.Task.Id == dbtask.Id && l.EndTime.HasValue).OrderByDescending(i => i.StartTime).ToList();
                    //var test2 = test.FirstOrDefault();
                    var lastWork = Context.ShCloneUpdateLogs.Where(l => l.Task.Id == dbtask.Id && l.EndTime.HasValue).OrderByDescending(i => i.Id).FirstOrDefault();
                    // если непонятно когда обновлялось, то сработать
                    if (lastWork != null)
                    {
                        var nextForecastDate = dbtask.StartDate.Value;
                        var now = currentDateTime;
                        // считаем теоретическое время след срабатывания
                        while (nextForecastDate < currentDateTime)
                        {
                            nextForecastDate = nextForecastDate.Add(span);
                        }

                        var lastForecastDate = nextForecastDate.Add(-span);

                        if (lastWork.EndTime.Value < lastForecastDate)
                        {
                            return true;
                        }
                        else
                        {
                            logger.Info(string.Format("{0} - через {1} ", dbtask.Name, (nextForecastDate - now).ToString()));
                            return false;
                        }



                    }
                    else // не выполнялось еще
                    {
                        return true;
                    }
                }
                else// кривой таймспан
                {
                    return true;
                }

            }
            else // не заполнены старт или интервал
            {
                return true;
            }
        }

        public void SheduleDbFreeTasks()
        {
            // while (true)
            {
                //  Intranet.Projects.Tasks.TaskManager.Instance.ALLOWTASKADD = false;
                // if (Intranet.Projects.Tasks.TaskManager.Instance.TasksList.Count==0)
                {
                    //PreparedExtractedFilesToLists(context);
                    //DBLOCK = true;
                    try
                    {
                        {
                            List<DbTaskParams> tasks = new List<DbTaskParams>();
                            lock (_locker)
                            {

                                // тут мы еще не знаем, успешно ли обновление сх клона. поэтому тригерные таски будут добавляться в хендлере схклона
                                // таски помеченные как триггерные
                                #region TriggeredTasks
                                var dbTasks = Context.DbTasks.Where(dt => dt.Triggered == true && dt.Active == true && dt.OperationalType == "always").OrderBy(dbt => dbt.Order).ToList();
                                foreach (var dbtask in dbTasks)
                                {

                                    if (ShouldStart(dbtask))
                                        tasks.Add(new DbTaskParams() { DbTask = dbtask });

                                }

                                #endregion

                                TaskManager.Instance.AddDbTasks(tasks);
                            }
                        }

                    }
                    catch (System.Exception ex)
                    {
                        LogError(ex.Message);
                    }




                }

            }
        }

        /// <summary>
        /// Удалить таск из списка. Удаляются самим таск менеджером
        /// </summary>
        /// <param name="task"></param>
        public void RemoveTask(TaskBase task)
        {
            if (TasksList == null)
                TasksList = new List<TaskBase>();
            try
            {
                TasksList.Remove(task);

                LogInfo(string.Format("<<<-----  Удален таск:{0}", task.TaskParameters.DbTask.Name));
                LogInfo(string.Format("<<<-----  В очереди: {0}------>>>", string.Join(", ", TasksList.Select(tl => tl.TaskParameters.DbTask.Name).ToList())));

            }
            catch (System.Exception ex)
            {
                LogError(string.Format("Таск отсутсвтует в списке активных тасков. таск:{0}", task.TaskParameters.DbTask.Description));
            }
            finally
            {
                task = null;
            }
          


        }
        #endregion
        //#region AllowTaskAdd
        ///// <summary>
        ///// Разрешено ли добавлять новые таски. Если нет, значит база готова к обновлению.
        ///// </summary>
        //public  bool ALLOWTASKADD
        //{
        //    get
        //    {
        //        return _ALLOWTASKADD;
        //    }
        //    set
        //    {
        //        LogInfo(string.Format("ALLOWTASKADD:{0}", value));
        //        _ALLOWTASKADD = value;

        //    }
        //}
        //private volatile bool _ALLOWTASKADD;
        /// <summary>
        /// Последовательная обработка тасков
        /// </summary>
      //  #endregion

        bool IsLocked()
        {
            bool result = false;
            try
            {
                if (!Monitor.TryEnter(_locker))
                    result = true;
                else
                    Monitor.Exit(_locker);

            }
            catch (Exception exc)
            {
                logger.Error("Долбаная ошибка синхронизации. "+ exc.Message);
            }
        
            return result;
        } 

        public void ProcessTasks()
        {
            
            //RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
            //var mails = processor.GetMails(new List<string> { " created" });
            
            //var hHeap = Heap.HeapCreate(Heap.HeapFlags.HEAP_GENERATE_EXCEPTIONS, 0, 0);
            //// if the FriendlyName is "heap.vshost.exe" then it's using the VS Hosting Process and not "Heap.Exe"
            //Console.WriteLine(AppDomain.CurrentDomain.FriendlyName + " heap created");
            //uint nSize = 100 * 1024 * 1024;
            //ulong nTot = 0;

            //for (int i = 0; i < 1000; i++)
            //{
            //    try
            //    {
            //        var ptr = Heap.HeapAlloc(hHeap, 0, nSize);
            //        nTot += nSize;


            //    }
            //    catch (Exception ex)
            //    {

            //        Console.WriteLine(String.Format("Max TM heap size: {0} ", nTot.ToFileSize()));
            //        break;
            //    }
            //}



            //Heap.HeapDestroy(hHeap);


           
            while (true)
            {

                if (!IsLocked())
                {
                    lock (_locker)
                    {
                        var task = TasksList.FirstOrDefault();
                        if (task != null)
                        {

                            logger.Debug("------>>> Старт таска: " + task.TaskParameters.DbTask.Name);
                            task.Process();
                            RemoveTask(task);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            var changed = Context.ChangeTracker.Entries();
                            foreach (var ch in changed)
                            {
                                //  Console.WriteLine(ch.State);
                                //if (ch.State != EntityState.Unchanged)
                                //{

                                //}
                            }
                            Context.SaveChanges();
                            //Context.
                        }
                        else
                        {
                            if (Context.ChangeTracker.Entries().Count() > 300)
                            {
                                Context.SaveChanges();
                                Context.Dispose();
                                Context = new Context();
                                logger.Debug("Произведено пересоздание контекста");
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(5000);
            }
          
        }

        public Context Context = new Context();

        #region Log
        private void LogInfo(string message)
        {
            logger.Info(message);
            Debug.WriteLine(message);
        }

        private void LogError(string message)
        {
            logger.Error(message);
            Debug.WriteLine(message);
        }
        #endregion
    }
}