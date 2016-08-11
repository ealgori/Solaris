using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.TaskModel;
using TaskManager.Handlers.TaskHandlers.ShClone;
//using TaskManager.Handlers.TaskHandlers.Models.PrintOut;
//using TaskManager.Handlers.TaskHandlers.Models.POR;

using TaskManager.Handlers.ImportHandlers;
using TaskManager.Handlers.TaskHandlers.Models.Solaris;
using TaskManager.Handlers.TaskHandlers.Models.WIH;
using TaskManager.Handlers.TaskHandlers.Models.AutoImport;
using TaskManager.Handlers.TaskHandlers.Models.Billing;
using TaskManager.Handlers.EmailHandlers.Models;
using TaskManager.Handlers.TaskHandlers.Models.TOH;
using TaskManager.Handlers.TaskHandlers.Models.SAT;
using TaskManager.Handlers.TaskHandlers.Models.Site;
using TaskManager.Handlers.TaskHandlers.Models.PO;
using TaskManager.Handlers.TaskHandlers.Models;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.Handlers.TaskHandlers.Models.Acts;
using TaskManager.Handlers.TaskHandlers.Models.BackUps;
using TaskManager.Handlers.TaskHandlers.Models.AVR;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionHandlers;
using TaskManager.Handlers.TaskHandlers.Models.Limits;
using TaskManager.Handlers.TaskHandlers.Models.Putevie;
//using TaskManager.Handlers.TaskHandlers.Models.POR;
//using TaskManager.Handlers.TaskHandlers.Models.MUSForms;
//using TaskManager.Handlers.TaskHandlers.Models.PO;
//using TaskManager.Handlers.TaskHandlers.Models;


namespace TaskManager
{
    public class TaskQuantifier
    {
        public bool FillHandlers(ref TaskBase task)
        {
            switch (task.TaskParameters.DbTask.Name)
            {
                case "ShCloneUpdate":
                    {
                        task.TaskHandler = new SHCloneBulkCopyHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "DistributionHandler":
                    {
                        task.TaskHandler = new DistributionHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "DistributionHandler2":
                    {
                        task.TaskHandler = new DistributionHandler2(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "DistributionHandler3":
                    {
                        task.TaskHandler = new DistributionHandler3(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "BackUpHandler":
                    {
                        task.TaskHandler = new BackUpHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "PriceListRefresh":
                    {
                        task.TaskHandler = new PriceListRefreshHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        break;
                    }
                case "SapCodeSend":
                    {
                        task.TaskHandler = new SAPSendHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "AVRSynchronization":
                    {
                        task.TaskHandler = new AVRSynchronizationHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        break;
                    }


                case "SendWIHSAPCodeRequest":
                    {
                        task.TaskHandler = new SendWIHSAPCodeRequest(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "WIHAnalyzer":
                    {
                        task.TaskHandler = new WIHAnalyzer(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        break;
                    }
                case "WIHTrashCleaner":
                    {
                        task.TaskHandler = new WIHTrashCleaner(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "SOLAutoImport":
                    {
                        task.TaskHandler = new SOLAutoImport(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        break;
                    }
                case "DataToSH":
                    {
                        task.TaskHandler = new DataToSH(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        break;
                    }
                case "SOLAutoReport":
                    {
                        task.TaskHandler = new SOLAutoReport(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                //case "SOLAutoReportRev2":
                //    {
                //        task.TaskHandler = new SOLAutoReportRev2(task.TaskParameters);
                //        task.FileIOSubHandler = null;
                //        task.ImportHandler = null;
                //        task.ConvertHandler = null;
                //        task.EmailHandler = new EmailHandlerRev2(task.TaskParameters);
                //        break;
                //    }


                case "TOImport":
                    {
                        task.TaskHandler = new TOImport(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "TORefresh":
                    {
                        task.TaskHandler = new TORefresh(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "SendWIHPORRequests":
                    {
                        task.TaskHandler = new SendWIHPORRequests(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "TOToSHHandler":
                    {
                        task.TaskHandler = new TOToSHHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        // у него свой механизм автоимпорта
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "ActToSHHandler":
                    {
                        task.TaskHandler = new ActToSHHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        // у него свой механизм автоимпорта
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "SiteIndexerHandler":
                    {
                        task.TaskHandler = new SiteIndexerHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "PORecallHandler":
                    {
                        task.TaskHandler = new PORecallHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "ActPrintToSHHandler":
                    {
                        task.TaskHandler = new ActPrintToSHHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "SubcontractorSyncHandler":
                    {
                        task.TaskHandler = new SubcontractorSyncHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "ActsInvoiceLinkingHandler":
                    {
                        task.TaskHandler = new ActsInvoiceLinkingHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "TOTotalAmmountUpdate":
                    {
                        task.TaskHandler = new TOTotalAmmountUpdate(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "TOItemOtchetPredostHandler":
                    {
                        task.TaskHandler = new TOItemOtchetPredostHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "SendWIHPORDelRequests":
                    {
                        task.TaskHandler = new SendWIHPORDelRequests(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "FolderBackUpHandler":
                    {
                        task.TaskHandler = new FolderBackUpHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = null;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                //case "SendNotifyHandler":
                //    {
                //        task.TaskHandler = new SendNotifyHandler(task.TaskParameters);
                //        task.FileIOSubHandler = null;
                //        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                //        task.ConvertHandler = null;
                //        task.EmailHandler = null;
                //        break;
                //    }

                case "SendRequestHandler":
                    {
                        task.TaskHandler = new SendVCRequestHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "SaveMailToAdmin2":
                    {
                        task.TaskHandler = new SaveMailToAdmin2(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "VCRequestAnalyzer":
                    {
                        task.TaskHandler = new VCRequestAnalyzer(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                //case "NeedPrepriceHandler":
                //    {
                //        task.TaskHandler = new NeedPrepriceHandler(task.TaskParameters);
                //        task.FileIOSubHandler = null;
                //        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                //        task.ConvertHandler = null;
                //        task.EmailHandler = null;
                //        break;
                //    }
                //case "ReadyForPORHandler":
                //    {
                //        task.TaskHandler = new ReadyForPORHandler(task.TaskParameters);
                //        task.FileIOSubHandler = null;
                //        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                //        task.ConvertHandler = null;
                //        task.EmailHandler = null;
                //        break;
                //    }
                //case "ReadyForRequestHandler":
                //    {
                //        task.TaskHandler = new ReadyForRequestHandler(task.TaskParameters);
                //        task.FileIOSubHandler = null;
                //        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                //        task.ConvertHandler = null;
                //        task.EmailHandler = null;
                //        break;
                //    }
                case "PutevieImportHandler":
                {
                    task.TaskHandler = new PutevieImportHandler(task.TaskParameters);
                    task.FileIOSubHandler = null;
                    task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                    task.ConvertHandler = null;
                    task.EmailHandler = null;
                    break;
                }

                case "AVRUnfreezeHandler":
                    {
                        task.TaskHandler = new AVRUnfreezeHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "LimitCalcHandler":
                    {
                        task.TaskHandler = new LimitCalcHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null; ;
                        break;

                    }
                case "UploadVCReqToCreateHandler":
                    {
                        task.TaskHandler = new UploadVCReqToCreateHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null; ;
                        break;

                    }
                case "ConditionsHandler":
                    {
                        task.TaskHandler = new ConditionsHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "NotifyHandler":
                    {
                        task.TaskHandler = new NotifyHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "SendWIHGRRequest":
                    {
                        task.TaskHandler = new SendWIHGRRequest(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = null;
                        break;
                    }
                case "TypeCheckHandler":
                    {
                        task.TaskHandler = new TypeCheckHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters); ;
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "SendWIHGRTORequestsHandler":
                    {
                        task.TaskHandler = new SendWIHGRTORequestsHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "LimitsAutoImportHandler":
                    {
                        task.TaskHandler = new LimitsAutoImportHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "SendToSubcontractorHandler":
                    {
                        task.TaskHandler = new SendToSubcontractorHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }

                case "EmptyAVRDistrHandler":
                    {
                        task.TaskHandler = new EmptyAVRDistrHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        //task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                case "NewAVRDistrHandler":
                    {
                        task.TaskHandler = new NewAVRDistrHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                        task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                        task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }

                case "SubcontrActUploadHandler":
                    {
                        task.TaskHandler = new SubcontrActUploadHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                       // task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                       // task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                 case "TestWIHHandler":
                    {
                        task.TaskHandler = new TestWIHHandler(task.TaskParameters);
                        task.FileIOSubHandler = null;
                       // task.ImportHandler = new ImportHandler(task.TaskParameters);
                        task.ConvertHandler = null;
                       // task.EmailHandler = new BaseEmailHandler(task.TaskParameters);
                        break;
                    }
                default:
                    {

                        break;
                    }
            }











            return true;
        }
    }
}