using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.IO;

using System.Collections;
using CommonFunctions.Extentions;
using DbModels.DomainModels.ShClone;
using Microsoft.Data.Extensions;
using DbModels.DomainModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Billing
{
    public class DataToSH : ATaskHandler
    {
        public DataToSH(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            // Подгрузить инфу по фактурам и инвойсам. а Так же Передано на оплату в бухг. и пр.
            string command1 = @"select '' as ShAVR, [PO Number] as PO,
					 [Invoice Number] as Invoice, 
                      [Factura Number] as Factura, [document date] as DocumentDate, 
                     [total amount] as TotalAmount, 
                    [receiving date] as ReceivingDate, 
                      [approved by od] as ApprovedByOD, comments as Comments, 
                     [passed to finance] as PassedToFinance, 
                     [scanned to ocr wf] as ScannedToocrwf, 
                      [sent to subcontractor] as SentToSubcontractorInv, 
                      [Delivery Note Number] as DeliveryNoteInv, [Item ID] as ItemID
from dbo.[Accounts_Payable_CU_Russia]
where [PO Number] is not null AND [Approved by OD]='Approved' 
order by [item id] ";


            string command2 = @"SELECT '' AS ShAVR
	  ,[Item ID]  as ItemID
      ,[SiteID]
      ,[PurchasingDocument] AS PurchasingDocument
      ,[Status]
      ,[Vendor]
      ,[DeliveryNoteNumber] AS DeliveryNote
      ,[PO_Creation_Date] AS POCreationDate
      ,[PO_Date_In_SAP] AS PODateInSAP
      ,[Legal_Sign_Seal] AS LegalSignSeal
      ,[Sent_To_Subcontractor] AS SentToSubcontractor
      ,[Received_From_Subcontractor] AS ReceivedFromSubcontractor
      ,[Reject_From_Subcontractor] AS RejectFromSubcontractor
      ,[Sent_To_Sourcing] AS SentToSourcing
      ,[Approved_By_Sourcing] AS ApprovedBySourcing
      ,[Rejected_By_Sourcing] AS RejectedBySourcing
      ,[Sent_To_OD] AS SentToOD
      ,[Approved_By_OD] AS ApprovedByOD
      ,[Rejected_By_OD] AS RejectedByOD
      ,[Comments] 
    FROM [POFlow_Temp].[dbo].[POFlow_CU_Russia]
    where [PurchasingDocument]  is not NULL
    order by [Item ID]";

            //  string command2 = @"SELECT '' AS ShAVR
            //	  ,[ItemID]  as ItemID
            //      ,[SiteID]
            //      ,[PurchasingDocument] AS PurchasingDocument
            //      ,[Status]
            //      ,[Vendor]
            //      ,[DeliveryNoteNumber] AS DeliveryNote
            //      ,[PO_Creation_Date] AS POCreationDate
            //      ,[PO_Date_In_SAP] AS PODateInSAP
            //      ,[Legal_Sign_Seal] AS LegalSignSeal
            //      ,[Sent_To_Subcontractor] AS SentToSubcontractor
            //      ,[Received_From_Subcontractor] AS ReceivedFromSubcontractor
            //      ,[Reject_From_Subcontractor] AS RejectFromSubcontractor
            //      ,[Sent_To_Sourcing] AS SentToSourcing
            //      ,[Approved_By_Sourcing] AS ApprovedBySourcing
            //      ,[Rejected_By_Sourcing] AS RejectedBySourcing
            //      ,[Sent_To_OD] AS SentToOD
            //      ,[Approved_By_OD] AS ApprovedByOD
            //      ,[Rejected_By_OD] AS RejectedByOD
            //      ,[Comments] 
            //    FROM [POFlow_Temp].[dbo].[POFlow_VimpelcomMS]
            //    where dbo.[POFlow_VimpelcomMS].[PurchasingDocument]  is not NULL
            //    order by [ItemID]";
            string command3 = @"SELECT [Item ID]
      ,[SiteID]
      
      ,[Status]
      ,[Vendor]
      ,[DeliveryNoteNumber]
      ,[PO_Creation_Date]
      ,[PO_Date_In_SAP]
      ,[Legal_Sign_Seal]
      ,[Sent_To_Subcontractor]
      ,[Received_From_Subcontractor]
      ,[Reject_From_Subcontractor]
      ,[Sent_To_Sourcing]
      ,[Approved_By_Sourcing]
      ,[Rejected_By_Sourcing]
      ,[Sent_To_OD]
      ,[Approved_By_OD]
      ,[Rejected_By_OD]
      ,[Comments]
       ,[PurchasingDocument]
  FROM [POFlow_Temp].[dbo].[POFlow_CU_Russia]";
            //string command3 = @"SELECT [ItemID]
            //      ,[SiteID]
            //      
            //      ,[Status]
            //      ,[Vendor]
            //      ,[DeliveryNoteNumber]
            //      ,[PO_Creation_Date]
            //      ,[PO_Date_In_SAP]
            //      ,[Legal_Sign_Seal]
            //      ,[Sent_To_Subcontractor]
            //      ,[Received_From_Subcontractor]
            //      ,[Reject_From_Subcontractor]
            //      ,[Sent_To_Sourcing]
            //      ,[Approved_By_Sourcing]
            //      ,[Rejected_By_Sourcing]
            //      ,[Sent_To_OD]
            //      ,[Approved_By_OD]
            //      ,[Rejected_By_OD]
            //      ,[Comments]
            //       ,[PurchasingDocument]
            //  FROM [POFlow_Temp].[dbo].[POFlow_VimpelcomMS]";
            string command4 = @"SELECT  [Item ID]
      ,[Table Name]
      ,[Document Type]
      ,[Payment Terms]
      ,[Site ID]
      ,[PO Number]
      ,[Vendor Name]
      ,[Invoice Number]
      ,[Factura Number]
      ,[Document Date]
      ,[Receiving Date]
      ,[LAS Responsible]
      ,[Total Amount]
      ,[Approved by OD]
      ,[Passed to Finance]
      ,[Comments]
      ,[Scanned to OCR WF]
      ,[Sent to Subcontractor]
      ,[Delivery Note Number]
      ,[Responsible Person]
  FROM [POFlow_Temp].[dbo].[Accounts_Payable_CU_Russia]

    WHERE [Approved by OD] ='Approved'"
                ;


           var InvoiceVymManSerResult = CommonFunctions.StaticHelpers.GetStoredProcDataFromServer<InvoiceVymManSer>("MAStorage", command1);
           //avr
            var POVympelcomManSerResult = CommonFunctions.StaticHelpers.GetStoredProcDataFromServer<POVympelcomManSer>("MAStorage", command2);
            //to
            var TOVympelcommanSerResult = CommonFunctions.StaticHelpers.GetStoredProcDataFromServer<TOVympelcomManSer>("MAStorage", command3);
            //var InvoiceVymManSerResult =new List<InvoiceVymManSer>();
            //var POVympelcomManSerResult = new List<POVympelcomManSer>();
           // var TOVympelcommanSerResult = new List<TOVympelcomManSer>();
            var TOPayVympelcommanSerResult = CommonFunctions.StaticHelpers.GetStoredProcDataFromServer<ShInvoice>("MAStorage", command4, x =>
                new ShInvoice
                {
                    InvoiceId = x.Field<int>("Item ID"),
                    TableName = x.Field<string>("Table Name"),
                    DocumentType = x.Field<string>("Document Type"),
                    PaymentTerms = x.Field<string>("Payment Terms"),
                    SiteID = x.Field<string>("Site ID"),
                    PONumber = x.Field<string>("PO Number"),
                    VendorName = x.Field<string>("Vendor Name"),
                    InvoiceNumber = x.Field<string>("Invoice Number"),
                    FacturaNumber = x.Field<string>("Factura Number"),
                    DocumentDate = x.Field<DateTime?>("Document Date"),
                    ReceivingDate = x.Field<DateTime?>("Receiving Date"),
                    LASResponsible = x.Field<string>("LAS Responsible"),
                    TotalAmount = x.Field<decimal?>("Total Amount"),
                    ApprovedByOD = x.Field<string>("Approved by OD"),
                    PassedToFinance = x.Field<DateTime?>("Passed to Finance"),
                    Comments = x.Field<string>("Comments"),
                    ScannedToOCRWF = x.Field<DateTime?>("Scanned to OCR WF"),
                    SentToSubcontractor = x.Field<DateTime?>("Sent to Subcontractor"),
                    DeliveryNoteNumber = x.Field<string>("Delivery Note Number"),
                    ResponsiblePerson = x.Field<string>("Responsible Person")


                });

            foreach (var item in InvoiceVymManSerResult)
            {
                //ShAVR shAvr = TaskParameters.Context.ShAVRf.FirstOrDefault(avr => avr.PurchaseOrderNumber == item.PO);
                //if (shAvr != null)
                //{
                //    item.ShAVR = shAvr.AVRId;
                //}
                //else
                //{
                    ShAVR shAvr = TaskParameters.Context.ShAVRs.FirstOrDefault(avr => avr.PurchaseOrderNumber == item.PO);
                    if (shAvr != null)
                    {
                        item.ShAVR = shAvr.AVRId;
                    }
                //}
            }
            InvoiceVymManSerResult = InvoiceVymManSerResult.Where(avr => !string.IsNullOrEmpty(avr.ShAVR)).ToList();

          //  var shAvrfs = TaskParameters.Context.ShAVRf.ToList();
            var shAvrss = TaskParameters.Context.ShAVRs.ToList();

            var testPO = "4512622268";
            var row = POVympelcomManSerResult.FirstOrDefault(r => r.PurchasingDocument == "4512622268");
              

            foreach (var item in POVympelcomManSerResult)
            {
                
                //ShAVR shAvr = shAvrfs.FirstOrDefault(avr => avr.PurchaseOrderNumber == item.PurchasingDocument);
                //if (shAvr != null)
                //{
                //    item.ShAVR = shAvr.AVRId;
                //}
                //else
                //{
                    ShAVR shAvr = shAvrss.FirstOrDefault(avr => avr.PurchaseOrderNumber == item.PurchasingDocument);
                    if (shAvr != null)
                    {
                        item.ShAVR = shAvr.AVRId;
                    }
                //}
            }
            POVympelcomManSerResult = POVympelcomManSerResult.Where(avr => !string.IsNullOrEmpty(avr.ShAVR)).ToList();
            var shToes = TaskParameters.Context.ShTOes.ToList();

            foreach (var shTO in shToes)
            {

                var toManSer = TOVympelcommanSerResult.FirstOrDefault(to => to.PurchasingDocument == shTO.PONumber);
                if (toManSer != null)
                {
                    toManSer.TO1 = shTO.TO;
                }

            }
            TOVympelcommanSerResult = TOVympelcommanSerResult.Where(to => !string.IsNullOrEmpty(to.TO1)).ToList();

            List<ShInvoice> importInvoices = new List<ShInvoice>();
            TOPayVympelcommanSerResult = TOPayVympelcommanSerResult.Where(p => !string.IsNullOrEmpty(p.PONumber)&&
                (!string.IsNullOrEmpty(p.InvoiceNumber))).ToList();
            for (int i = 0; i < TOPayVympelcommanSerResult.Count; i++)
			{
                decimal dec = (decimal) TOPayVympelcommanSerResult[i].TotalAmount;
                dec = dec.FinanceRound();
                TOPayVympelcommanSerResult[i].TotalAmount = dec;

              
			}
            var _shInvoices = TaskParameters.Context.ShInvoices.ToList();
            foreach (var ODInvoice in TOPayVympelcommanSerResult)
            {
                //TO
                System.Diagnostics.Debug.WriteLine(ODInvoice.PONumber);
                var shTO = TaskParameters.Context.ShTOes.FirstOrDefault(t => t.PONumber == ODInvoice.PONumber);
                if (shTO != null)
                {
                   if(string.IsNullOrEmpty(shTO.PONumber))
                   {
                       continue;
                   }
                    ShInvoice importInvoice = null;
                    var shInvoices = _shInvoices.Where(i => i.TOId == shTO.TO);

                    var shInvoice = shInvoices.FirstOrDefault(i =>
                        ODInvoice.InvoiceNumber == i.InvoiceNumber
                        && ODInvoice.TotalAmount == i.TotalAmount
                        && ODInvoice.DocumentDate == i.DocumentDate
                        );
                    if (shInvoice != null)
                    {
                        if (ODInvoice.TableName == shInvoice.TableName)
                            if (ODInvoice.DocumentType == shInvoice.DocumentType)
                                if (ODInvoice.PaymentTerms == shInvoice.PaymentTerms)
                                    if (ODInvoice.SiteID == shInvoice.SiteID)
                                        if (ODInvoice.PONumber == shInvoice.PONumber)
                                            if (ODInvoice.FacturaNumber == shInvoice.FacturaNumber)
                                                if (ODInvoice.ReceivingDate == shInvoice.ReceivingDate)
                                                    if (ODInvoice.LASResponsible == shInvoice.LASResponsible)
                                                        if (ODInvoice.TotalAmount == shInvoice.TotalAmount)
                                                            if (ODInvoice.ApprovedByOD == shInvoice.ApprovedByOD)
                                                                if (ODInvoice.PassedToFinance == shInvoice.PassedToFinance)
                                                                    if (ODInvoice.Comments == shInvoice.Comments)
                                                                        if (ODInvoice.ScannedToOCRWF == shInvoice.ScannedToOCRWF)
                                                                            if (ODInvoice.SentToSubcontractor == shInvoice.SentToSubcontractor)
                                                                                if (ODInvoice.DeliveryNoteNumber == shInvoice.DeliveryNoteNumber)
                                                                                    if (ODInvoice.ResponsiblePerson == shInvoice.ResponsiblePerson)
                                                                                    {
                                                                                        continue;
                                                                                    }
                        ODInvoice.InvoiceId = shInvoice.InvoiceId;
                        ODInvoice.TOId = shTO.TO;
                       
                        importInvoices.Add(ODInvoice);


                    }
                    else
                    {
                        ODInvoice.InvoiceId = -1000;
                        ODInvoice.TOId = shTO.TO;
                        importInvoices.Add(ODInvoice);
                    }


                }

                //AVR



            }
            var lastInvoices = TOPayVympelcommanSerResult.GroupBy(i => new { i.PONumber, i.InvoiceNumber }).Select(g=>g.OrderByDescending(i=>i.InvoiceId).FirstOrDefault());
            foreach (var ODInvoice in lastInvoices)
         
            {
                var shAVR = TaskParameters.Context.ShAVRs.FirstOrDefault(t => t.PurchaseOrderNumber == ODInvoice.PONumber);
                if (shAVR != null)
                {
                    if (string.IsNullOrEmpty(shAVR.PurchaseOrderNumber))
                    {
                        continue;
                    }

                    ShInvoice importInvoice = null;
                    var shInvoice = TaskParameters.Context.ShInvoices.FirstOrDefault(i => i.AVRid == shAVR.AVRId);


                    if (shInvoice != null)
                    {



                        if (ODInvoice.TableName == shInvoice.TableName)
                            if (ODInvoice.DocumentType == shInvoice.DocumentType)
                                if (ODInvoice.PaymentTerms == shInvoice.PaymentTerms)
                                    if (ODInvoice.SiteID == shInvoice.SiteID)
                                        if (ODInvoice.PONumber == shInvoice.PONumber)
                                            if (ODInvoice.FacturaNumber == shInvoice.FacturaNumber)
                                                if (ODInvoice.ReceivingDate == shInvoice.ReceivingDate)
                                                    if (ODInvoice.LASResponsible == shInvoice.LASResponsible)
                                                        if (ODInvoice.TotalAmount == shInvoice.TotalAmount)
                                                            if (ODInvoice.ApprovedByOD == shInvoice.ApprovedByOD)
                                                                if (ODInvoice.PassedToFinance == shInvoice.PassedToFinance)
                                                                    if (ODInvoice.Comments == shInvoice.Comments)
                                                                        if (ODInvoice.ScannedToOCRWF == shInvoice.ScannedToOCRWF)
                                                                            if (ODInvoice.SentToSubcontractor == shInvoice.SentToSubcontractor)
                                                                                if (ODInvoice.DeliveryNoteNumber == shInvoice.DeliveryNoteNumber)
                                                                                    if (ODInvoice.ResponsiblePerson == shInvoice.ResponsiblePerson)
                                                                                    {
                                                                                        continue;
                                                                                    }
                        ODInvoice.InvoiceId = shInvoice.InvoiceId;
                        ODInvoice.AVRid = shAVR.AVRId;
                        importInvoices.Add(ODInvoice);


                    }
                    else
                    {
                        ODInvoice.InvoiceId = -1000;
                        ODInvoice.AVRid = shAVR.AVRId;
                        importInvoices.Add(ODInvoice);
                    }


                }
            
            }




            var wb1 = NpoiInteract.GetNewWorkBook();
            NpoiInteract.FillReportData(InvoiceVymManSerResult.ToDataTable(), "avrInvoise", wb1);
            NpoiInteract.FillReportData(POVympelcomManSerResult.ToDataTable(), "avrOrder", wb1);
            NpoiInteract.FillReportData(TOVympelcommanSerResult.ToDataTable(), "TOManSer", wb1);
            NpoiInteract.FillReportData(importInvoices.ToDataTable(), "InvoicePay", wb1);
            NpoiInteract.SaveReport(Path.Combine(TaskParameters.DbTask.ArchiveFolder, string.Format("InvoiceTrack{0}.xls", DateTime.Now.ToString("yyMMddHHmmss"))), wb1);



            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(InvoiceVymManSerResult) });
            //TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(POVympelcomManSerResult) });
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(POVympelcomManSerResult) });
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName4, Objects = new ArrayList(TOVympelcommanSerResult) });
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName5, Objects = new ArrayList(importInvoices) });

            return true;
        }

        public class InvoiceVymManSer
        {
            public string ShAVR { get; set; } //A
            public string PO { get; set; }
            public string Invoice { get; set; } //C
            public string Factura { get; set; }  //D
            public DateTime? DocumentDate { get; set; }//E
            public decimal TotalAmount { get; set; }//F
            public DateTime? ReceivingDate { get; set; }//G
            public string ApprovedByOD { get; set; }//H
            public string Comments { get; set; }//I
            public DateTime? PassedToFinance { get; set; }//J
            public DateTime? ScannedToOCRWF { get; set; }
            public DateTime? SentToSubcontractorInv { get; set; }//L
            public string DeliveryNoteInv { get; set; }//M
            public int? ItemID { get; set; }

        }
        public class POVympelcomManSer
        {
            public string ShAVR { get; set; }
            public string PurchasingDocument { get; set; }
            public string Status { get; set; }
            public DateTime? SentToSourcing { get; set; }
            public DateTime? ApprovedBySourcing { get; set; }
            public DateTime? LegalSignSeal { get; set; }
            public DateTime? SentToSubcontractor { get; set; }
            public string DeliveryNote { get; set; }
            public int? ItemID { get; set; }
            public string SiteID { get; set; }
            public string Vendor { get; set; }
            public DateTime? POCreationDate { get; set; }
            public DateTime? PODateInSAP { get; set; }
            public DateTime? ReceivedFromSubcontractor { get; set; }
            public DateTime? RejectFromSubcontractor { get; set; }
            public DateTime? RejectedBySourcing { get; set; }
            public DateTime? SentToOD { get; set; }
            public DateTime? ApprovedByOD { get; set; }
            public DateTime? RejectedByOD { get; set; }
            public string Comments { get; set; }



        }

        public class TOVympelcomManSer
        {

            public string TO1 { get; set; }
            public string Status { get; set; }
            public DateTime? PO_Creation_Date { get; set; }
            public DateTime? PO_Date_In_SAP { get; set; }
            public DateTime? Legal_Sign_Seal { get; set; }

            public DateTime? Sent_To_Subcontractor { get; set; }
            public DateTime? Received_From_Subcontractor { get; set; }
            public DateTime? Reject_From_Subcontractor { get; set; }
            public DateTime? Sent_To_Sourcing { get; set; }
            public DateTime? Approved_By_Sourcing { get; set; }
            public DateTime? Rejected_By_Sourcing { get; set; }
            public DateTime? Sent_To_OD { get; set; }
            public DateTime? Approved_By_OD { get; set; }
            public DateTime? Rejected_By_OD { get; set; }
            public string Comments { get; set; }

            public string PurchasingDocument { get; set; }



        }



    }
}
