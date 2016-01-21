//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using TaskManager.TaskParamModels;
//using DbModels.DomainModels.ShClone;
//using TaskManager.Service;


//namespace TaskManager.Handlers.TaskHandlers.Models
//{
//    public class WoTotalHandler : ATaskHandler
//    {
//        public WoTotalHandler(TaskParameters taskParameters) : base(taskParameters) { }
//        public override bool Handle()
//        {
//            List<WOTotalProc> ResultList = new List<WOTotalProc>();
//            List<ShWO> WOList = TaskParameters.Context.ShWOs.ToList();
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество заказов в СХ - {0}", WOList.Count()));
//            foreach (var wo in WOList)
//            {
//                try
//                {
//                    var WoTotal = SHWOService.GetWOTotalPrice(wo, TaskParameters.Context);
//                    if ((!wo.WoTotal.HasValue || WoTotal != wo.WoTotal.Value) && WoTotal != 0)
//                    {
//                        ResultList.Add(new WOTotalProc { WO = wo.WO, WoTotal = WoTotal });
//                    }
//                }
//                catch (Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при расчете Total Amount для заказа{0} - {1}", wo.WO, ex.Message));
//                    continue;
//                }
//            }
//            if (ResultList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { Objects = new System.Collections.ArrayList(ResultList), ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1 });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество заказов, для которых пересчитано значение Total Amount - {0}", ResultList.Count()));
//            return true;
//        }
//    }
//    public class WOTotalProc
//    {
//        public string WO { get; set; }
//        public decimal? WoTotal { get; set; }
//    }
//}