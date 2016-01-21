using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbModels.DomainModels.DbTasks;


namespace TaskManager.TaskParamModels
{
    /// <summary>
    /// Используется для создания тасков из дбтасков. Инкапсулирует параметры. Используется вроде только в таскфактори. Еще при создании тасков из дбтасков. 
    /// </summary>
    public class DbTaskParams
    {
        public DbTask DbTask { get; set; }
        public FileHandlerParams FileHandlerParams { get; set; }
    }
}