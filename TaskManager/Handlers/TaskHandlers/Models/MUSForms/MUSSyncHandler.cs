//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.Collections;

//namespace TaskManager.Handlers.TaskHandlers.Models.MUSForms
//{
//    public class MUSSyncHandler : ATaskHandler
//    {
//        public MUSSyncHandler(TaskParameters taskParameters) : base(taskParameters) { }
//        public override bool Handle()
//        {
//            TaskParameters.ImportHandlerParams = new ImportHandlerParams();
//            List<MUSApprovedProc> MUSApprovedList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<MUSApprovedProc>("ERUMOMW0009_OHDB_MUS_Approved_Sync", null);
//            if (MUSApprovedList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(MUSApprovedList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество мусов, одобренных в ОД - {0}", MUSApprovedList.Count));
//            List<MUSRejectedProc> MUSRejectedList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<MUSRejectedProc>("ERUMOMW0009_OHDB_MUS_Rejected_Sync", null);
//            if (MUSRejectedList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(MUSRejectedList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество мусов, отреджекченных в ОД - {0}", MUSRejectedList.Count));
//            List<MUSNetworkSyncProc> MUSNetworkList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<MUSNetworkSyncProc>("ERUMOMW0009_OHDB_MUS_Network_Sync", null);
//            if (MUSNetworkList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(MUSNetworkList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Нетворков синхронизированно - {0}", MUSNetworkList.Count));
//            return true;
//        }
//    }
//    public class MUSApprovedProc
//    {
//        public string MUSName { get; set; }
//        public DateTime ApprovedDate { get; set; }
//    }
//    public class MUSRejectedProc
//    {
//        public string MUSName { get; set; }
//        public DateTime RejectedDate { get; set; }
//        public string RejectReason { get; set; }
//    }
//    public class MUSNetworkSyncProc
//    {
//        public string ShWBS { get; set; }
//        public string Network { get; set; }
//        public string SO { get; set; }
//        public string WBS { get; set; }
//    }
//}
