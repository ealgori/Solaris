using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Data.SqlClient;

using System.Data;
using TaskManager.TaskParamModels;
using DbModels.DomainModels.ShClone;

namespace TaskManager.Handlers.TaskHandlers.ShClone
{
    public class SHCloneBulkCopyHandler : ATaskHandler
    {
        public SHCloneBulkCopyHandler(TaskParameters parameters)
            : base(parameters)
        {

        }
        public override bool Handle()
        {

            //try
            //{
            //    TaskParameters.TaskLogger.LogDebug("Чистим контекст:");
            //    // чистим контекст
            //    foreach (var item in TaskParameters.TaskLogger.context.ChangeTracker.Entries())
            //    {
            //        item.State = EntityState.Detached;
            //        TaskParameters.TaskLogger.LogDebug(item.Entity.GetType() + " was detached from context");
            //    }

            //}
            //catch(Exception exc)
            //{
            //    TaskParameters.TaskLogger.LogError("Ошибка при очистке контекста "+exc.Message);
            //}
            try
            {
                var types = TaskParameters.Context.ChangeTracker.Entries().GroupBy(gr => gr.Entity.GetType()).Select(gro => new { type = gro.Key.ToString(), count = gro.Count() });// (ent=>ent.Entity.GetType()).Distinct();
                types.ToList().ForEach(ty => { TaskParameters.TaskLogger.LogDebug(ty.type + " " + ty.count); });
                TaskParameters.TaskLogger.LogDebug("Всего:"+TaskParameters.Context.ChangeTracker.Entries().Count());
            }
            catch
            {
                TaskParameters.TaskLogger.LogDebug("ошибка при анализе ChangeTracker");
            }

            List<DataTable> objects = (List<DataTable>)TaskParameters.FileHandlerParams.ObjectParameters.FirstOrDefault().Value.Objects[0];
            System.Diagnostics.Stopwatch fullWatch = new System.Diagnostics.Stopwatch();
          
            fullWatch.Start();
            TaskParameters.TaskLogger.LogWarn("..!!..!!..!!..Очищаем таблицы ");
            // Очищаем таблицы
            string connectString1 = System.Configuration.ConfigurationManager.
                   ConnectionStrings["LocalDb"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(connectString1))
            {
                // Opening the connection automatically enlists it in the  
                // TransactionScope as a lightweight transaction.
                connection1.Open();
                var tables = objects.Select(tab => tab.TableName).Distinct().Reverse();
                foreach (var table in tables)
                {

                    string command = string.Format("DELETE FROM {0}", table);
                    // Create the SqlCommand object and execute the first command.
                    SqlCommand command1 = new SqlCommand(command, connection1);
                    try
                    {
                        var returnValue = command1.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        TaskParameters.TaskLogger.LogError("Ошибка дропа таблицы "+table);
                    }

                    // writer.WriteLine("Rows to be affected by command1: {0}", returnValue);
                }

            }

            TaskParameters.TaskLogger.LogWarn("..!!..!!..!!..Понеслась! ");
            var updateLog = new ShCloneUpdateLog();
            updateLog.Task = TaskParameters.DbTask;
            TaskParameters.Context.ShCloneUpdateLogs.Add(updateLog);
            TaskParameters.Context.SaveChanges();
            updateLog.StartTime = DateTime.Now;
            try
            {
                {

                   
                    using (var loader =  new System.Data.SqlClient.SqlBulkCopy(connectString1))
                    {
                        foreach (var table in objects)
                        {
                            try
                            {
                                loader.ColumnMappings.Clear();
                                foreach (DataColumn col in table.Columns)
                                {
                                    loader.ColumnMappings.Add(new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName));
                                }

                                loader.DestinationTableName = table.TableName;
                                loader.WriteToServer(table);
                                table.Dispose();
                            }
                            catch(Exception exc)
                            {
                                TaskParameters.TaskLogger.LogError("Ошибка при заливке информации в базу. Таблица "+table.TableName+" Message: " + exc.Message);
                            }

                        }

                    }
                    
                    //GC.SuppressFinalize(objects);
                    objects = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    updateLog.Success = true;


                }

            }
           
            catch (Exception ex)
            {
                TaskParameters.TaskLogger.LogError("Ошибка при заливке информации в базу. Message: " + ex.Message);
                return false;
            }
           

            fullWatch.Stop();
            TaskParameters.TaskLogger.LogWarn("..!!..!!..!!..Успешно. Общее время обновления:" + fullWatch.Elapsed.TotalSeconds + " секунд");
            updateLog.Comment = fullWatch.Elapsed.TotalSeconds.ToString();
            updateLog.EndTime = DateTime.Now;
            TaskParameters.Context.SaveChanges();

            #region TriggeredTasks
            //var dbTasks = TaskParameters.Context.DbTasks.Where(dt => dt.Triggered == true && dt.Active == true).OrderBy(dbt => dbt.Order).ToList();
            //List<DbTaskParams> tasks = new List<DbTaskParams>();
            //foreach (var dbtask in dbTasks)
            //{
            //    tasks.Add(new DbTaskParams() { DbTask = dbtask });
            //    // Context.Entry(dbtask).State = System.Data.EntityState.Detached;
            //}
            //TaskManager.Instance.AddDbTasks(tasks);
            #endregion
            return true;

        }
    }
}