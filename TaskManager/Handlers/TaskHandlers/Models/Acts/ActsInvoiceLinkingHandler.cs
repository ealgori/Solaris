using DbModels.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;
using TaskManager.TaskParamModels;
using System.Collections;
namespace TaskManager.Handlers.TaskHandlers.Models.Acts
{
    public class ActsInvoiceLinkingHandler : ATaskHandler
    {
        public ActsInvoiceLinkingHandler(TaskParameters taskParameters):base(taskParameters)
        {

        }
        public override bool Handle()
        {
            var shInvoices = TaskParameters.Context.ShInvoices.AsNoTracking().Where(i=>!string.IsNullOrEmpty(i.TOId)
                &&(i.DocumentDate.HasValue)
                ).ToList();
            var shActs = TaskParameters.Context.ShActs.AsNoTracking().Where(a=>a.ActApprovedDate.HasValue).ToList();
          
            // исключив из всех, привязанные инвойсы, получаем непривязанные
            var notlinkedInvoices = shInvoices.Where(i => !i.ActId.HasValue).ToList();
            List<int> linkedInvoiceNames = new List<int>();
            List<ActInvoiceImportModel> models = new List<ActInvoiceImportModel>();
            foreach (var shAct in shActs)
            {
                var shTo = TaskParameters.Context.ShTOes.AsNoTracking().FirstOrDefault(t => t.TO==shAct.TOId);
                if(shTo!=null)
                {
                    var toInvoices = shInvoices.Where(t => t.TOId == shTo.TO).ToList();
                    var actTotal = (shAct.ObshayaStoimost).FinanceRound(0);
                    var actTotalVAT = (shAct.ObshayaStoimost * 1.18M).FinanceRound(0);
                    var invoicesTotal = toInvoices.Sum(i => i.TotalAmount).FinanceRound(0);
                    var invoices = new List<ShInvoice>();
                    ShInvoice closestInvoice = null;

                    //первый случай - сумма акта равно суммае одного или нескольких из неподвязанных инвойсов
                    invoices = toInvoices.Where(i=>!i.ActId.HasValue).Where(i => i.TotalAmount.FinanceRound(0) == actTotal).ToList();
                    
                    if (invoices.Count > 0)
                    {
                        closestInvoice = GetNearest(invoices, shAct.ActApprovedDate.Value, linkedInvoiceNames);
                    }
                    else
                    {

                        // второй случай - сумма акта равна сумме всех инвойсов
                        if (actTotal == invoicesTotal)
                        {
                            closestInvoice = GetNearest(toInvoices, shAct.ActApprovedDate.Value, linkedInvoiceNames);
                        }
                        else
                        {
                            // третий случай - сумма акта *1.18 равна сумме одного или нескольких неподвязанных инвойсов
                            invoices = toInvoices.Where(i => !i.ActId.HasValue).Where(i => i.TotalAmount.FinanceRound(0) == actTotalVAT).ToList();
                            if (invoices.Count > 0)
                            {
                                closestInvoice = GetNearest(invoices, shAct.ActApprovedDate.Value, linkedInvoiceNames);
                            }
                            else
                            {

                                // четвертый случай - сумма акта *1.18 равна сумме всех инвойсов
                                if (actTotalVAT == invoicesTotal)
                                {
                                    closestInvoice = GetNearest(toInvoices, shAct.ActApprovedDate.Value, linkedInvoiceNames);
                                }
                            }
                        }
                    }


                    if (closestInvoice != null)
                        {

                            linkedInvoiceNames.Add(closestInvoice.InvoiceId);
                            models.Add(new ActInvoiceImportModel()
                            {
                                Act = shAct.Act,
                                InvoiceId = closestInvoice.InvoiceId,
                                ActApproveDate = shAct.ActApprovedDate,
                                ActObshSt = shAct.ObshayaStoimost,
                                InvDocDate = closestInvoice.DocumentDate,
                                InvoiceTA = closestInvoice.TotalAmount
                            });
                        }
                    }
                
            }
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(models) });
           
            return true;
        }


        public ShInvoice GetNearest(List<ShInvoice> invoices, DateTime date, List<int> alreadyLinkedInvoices)
        {
            var closestInvoice = invoices.Where(i => !i.ActId.HasValue)
                .Where(i=>!alreadyLinkedInvoices.Contains(i.InvoiceId))
                .OrderBy(i =>
                         Math.Abs((date - i.DocumentDate.Value).TotalDays)
                         ).FirstOrDefault();
            return closestInvoice;
        }

        public class ActInvoiceImportModel
        {
            public int InvoiceId { get; set; }
            public string Act { get; set; }
          
            public decimal? InvoiceTA { get; set; }
            public decimal? ActObshSt { get; set; }
            public DateTime? InvDocDate { get; set; }
            public DateTime? ActApproveDate { get; set; }
        }
    }




    //public class POSyncHandler : ATaskHandler
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
}
