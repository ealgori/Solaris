//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.Collections;

//namespace TaskManager.Handlers.TaskHandlers.Models.PO
//{
//    public class POSyncHandler : ATaskHandler
//    {
//        public POSyncHandler(TaskParameters taskParameters) : base(taskParameters) { }
//        public override bool Handle()
//        {
//            TaskParameters.ImportHandlerParams = new ImportHandlerParams();
//            List<POApprovedProc> MUSApprovedList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<POApprovedProc>("ERUMOMW0009_OHDB_PO_Approved_Sync", null);
//            if (MUSApprovedList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(MUSApprovedList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество ПОРов, одобренных в ОД - {0}", MUSApprovedList.Count));
//            List<PORejectedProc> MUSRejectedList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<PORejectedProc>("ERUMOMW0009_OHDB_PO_Rejected_Sync", null);
//            if (MUSRejectedList.Count > 0)
//            {

//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(MUSRejectedList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество ПОРов, отреджекченных в ОД - {0}", MUSRejectedList.Count));
//            List<PONumberSyncProc> MUSNetworkList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<PONumberSyncProc>("ERUMOMW0009_OHDB_PO_Number_Sync", null);
//            if (MUSNetworkList.Count > 0)
//            {

//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(MUSNetworkList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Синхронизированно номеров ПО - {0}", MUSNetworkList.Count));
//            return true;
//        }
//    }
//    public class POApprovedProc
//    {
//        public string POR { get; set; }
//        public DateTime ApprovedDate { get; set; }
//    }
//    public class PORejectedProc
//    {
//        public string POR { get; set; }
//        public DateTime RejectedDate { get; set; }
//        public string RejectReason { get; set; }

//    }
//    public class PONumberSyncProc
//    {
//        public string POR { get; set; }
//        public string PONumber { get; set; }

//    }
//}
