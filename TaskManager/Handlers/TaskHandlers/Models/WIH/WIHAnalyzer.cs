using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using MailProcessing;
using WIHInteract;
using DbModels.DomainModels.ShClone;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class WIHAnalyzer : ATaskHandler
    {

        public const string WIHMailTypeCreated = "CREATED";
        public const string WIHMailTypeCompleted = "COMPLETED";
        public const string WIHMailTypeRejected = "REJECTED";
        public WIHAnalyzer(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            RedemptionMailProcessor redemtion = new RedemptionMailProcessor("SOLARIS");
            ProcessCreated(redemtion);
            ProcessCompleted(redemtion);
            ProcessRejected(redemtion);

            return true;

        }
        //private string GetInternalMailTypeByWIHId(string WIHId)
        //{
        //    var shPOR = TaskParameters.Context.ShPORs.FirstOrDefault(p => p.WIHIdPOR == WIHId);
        //    if (shPOR == null)
        //    {
        //        var shMUS = TaskParameters.Context.ShMUSs.FirstOrDefault(m => m.WIHIdMUS == WIHId);
        //        if (shMUS == null)
        //        {
        //            return string.Empty;
        //        }
        //        return WIHInteract.Constants.InternalMailTypeMUS;
        //    }

        //    return WIHInteract.Constants.InternalMailTypePOR;
        //}
        private void ProcessCompleted(RedemptionMailProcessor redemtion)
        {
            var CompletedMails = redemtion.GetMails(new List<string> { WIHMailTypeCompleted });
            TaskParameters.TaskLogger.LogInfo("Количество писем Completed - " + CompletedMails.Count);
            List<ShWIHRequest> PORResult = new List<ShWIHRequest>();
            List<PorCompletedImportModel> PORResult2 = new List<PorCompletedImportModel>();
            List<PorCompletedImportModel> PORResult3 = new List<PorCompletedImportModel>();
           

            //List<UpdateWIHDataMUS> MUSResult = new List<UpdateWIHDataMUS>();
            //List<UpdateWBS> WBSResult = new List<UpdateWBS>();
            foreach (var mail in CompletedMails)
            {
                string WIHId = WIHInteractor.GetWIHId(mail.Subject);
                if (string.IsNullOrEmpty(WIHId))
                {
                    TaskParameters.TaskLogger.LogError("Не удалось распарсить WIH Id.");
                    continue;
                }


                string internalMailType = string.Empty;
                var request = TaskParameters.Context.WIHRequests.FirstOrDefault(wr => wr.WIHNumber == WIHId);
                var shRequest = TaskParameters.Context.ShWIHRequests.FirstOrDefault(wr => wr.WIHnumber == WIHId);
                if (request != null)
                {
                    internalMailType = WIHInteract.Constants.InternalMailTypeSAPCODE;
                }
                else
                {
                    if (shRequest != null)
                        
                        internalMailType = shRequest.Type;
                }

                switch (internalMailType)
                {
                    case WIHInteract.Constants.InternalMailTypeTOPOR:
                        {

                            string poNumber = WIHInteractor.GetSingleLineResultByRegex(mail.Body, "#PO#:");
                            if (string.IsNullOrEmpty(poNumber))
                            {
                                TaskParameters.TaskLogger.LogError(string.Format("WIH ID - {0}; Не удалось распарсить номер ПО.", WIHId));
                                continue;
                            }
                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                 CompletedByOD = DateTime.Now
                              
                            });
                            PORResult2.Add(new PorCompletedImportModel()
                            {
                                 TO = shRequest.TOid,
                                 PO = poNumber
                                 

                                  

                            });

                            break;
                        }

                    case WIHInteract.Constants.InternalMailTypeTORecall:
                        {

                            
                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                CompletedByOD = DateTime.Now

                            });
                            PORResult3.Add(new PorCompletedImportModel()
                            {
                                TO = shRequest.TOid
                               

                            });

                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeSAPCODE:
                        {
                              request.ReceivedWIHResultDate = DateTime.Now;
                               request.Approved = true;
                   
                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeTOPORDel: 
                        {
                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                CompletedByOD = DateTime.Now

                            }); 
                            break;
                        }
                    default:
                        {
                            continue;
                        }

                }

             
                    TaskParameters.Context.SaveChanges();
                    //Только если все хорошо, перекидываем письмо в папку Completed
                    TaskParameters.TaskLogger.LogInfo("Письмо обработано. Перемещаем в папку Completed");
                    redemtion.MoveToCompleted(mail.ConversationId);
                
            }
            #region Загружаем данные в СХ

            if (PORResult.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(PORResult) });
            }
             if (PORResult2.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(PORResult2) });
            }
             if (PORResult3.Count > 0)
             {
                 TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(PORResult3) });
             }
            //if (MUSResult.Count > 0)
            //{
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(MUSResult) });
            //}
            //if (WBSResult.Count > 0)
            //{
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(WBSResult) });
            //}
            #endregion
        }
        private void ProcessRejected(RedemptionMailProcessor redemtion)
        {
            var RejectedMails = redemtion.GetMails(new List<string> { WIHMailTypeRejected });
            TaskParameters.TaskLogger.LogInfo("Количество писем Rejected - " + RejectedMails.Count);
            List<ShWIHRequest> PORResult = new List<ShWIHRequest>();
            List<ShAddAgreement> AGRResult = new List<ShAddAgreement>();
          //  List<UpdateWIHDataMUS> MUSResult = new List<UpdateWIHDataMUS>();
            foreach (var mail in RejectedMails)
            {
                string WIHId = WIHInteractor.GetWIHId(mail.Subject);
                if (string.IsNullOrEmpty(WIHId))
                {
                    TaskParameters.TaskLogger.LogError("Не удалось распарсить WIH Id.");
                    continue;
                }
                string RejectReason = WIHInteractor.GetMultiLineResultByRegex(mail.Body, "#RejectReason#");
                string internalMailType = string.Empty;
                var request = TaskParameters.Context.WIHRequests.FirstOrDefault(wr => wr.WIHNumber == WIHId);
                var shRequest = TaskParameters.Context.ShWIHRequests.FirstOrDefault(wr => wr.WIHnumber == WIHId);
                if (request != null)
                {
                    internalMailType = WIHInteract.Constants.InternalMailTypeSAPCODE;
                }
                else
                {
                    if (shRequest != null)
                        internalMailType = shRequest.Type;
                }
               

                
                
                
                
                //if (internalMailType == null)
                //{
                //    TaskParameters.TaskLogger.LogError("Не удалось найти обьект по его WIH Id");
                //    continue;
                //}
                //TaskParameters.TaskLogger.LogInfo("Internal Mail Type - " + internalMailType);

                switch (internalMailType)
                {
                    case WIHInteract.Constants.InternalMailTypeTOPOR:
                        {

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                RejectedByOD = DateTime.Now,
                                RejectedComment = RejectReason
                            });
                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeTOPORDel:
                        {

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                RejectedByOD = DateTime.Now,
                                RejectedComment = RejectReason
                            });
                            AGRResult.Add(new ShAddAgreement
                            {
                                AddAgreement=shRequest.AddAgreementId,
                                SendAddAgreement = false, 
                                ErrorCreationComment = RejectReason
                                 
                            });

                            break;
                        }

                    case WIHInteract.Constants.InternalMailTypeTORecall:
                        {

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHrequests = shRequest.WIHrequests,
                                RejectedByOD = DateTime.Now,
                                RejectedComment = RejectReason
                            });
                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeSAPCODE:
                        {
                            request.RejectReason = RejectReason;
                            request.ReceivedWIHResultDate = DateTime.Now;
                            request.Approved = false;
                            break;
                        }
                    default:
                        {
                            continue;
                        }

                }
              
                    
                    //Только если все хорошо, перекидываем письмо в папку Rejected
                    TaskParameters.TaskLogger.LogInfo("Письмо обработано. Перемещаем в папку Rejected");
                    redemtion.MoveToRejected(mail.ConversationId);
                
            }
            #region Загружаем данные в СХ

            if (PORResult.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(PORResult) });
            }
            //if (MUSResult.Count > 0)
            //{
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(MUSResult) });
            //}
            if(AGRResult.Count>0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName5, Objects = new ArrayList(AGRResult) });
            }

            #endregion
        }
        private void ProcessCreated(RedemptionMailProcessor redemtion)
        {
            var CreatedMails = redemtion.GetMails(new List<string> { WIHMailTypeCreated });
            TaskParameters.TaskLogger.LogInfo("Количество писем Сreated - " + CreatedMails.Count);
            List<ShWIHRequest> PORResult = new List<ShWIHRequest>();
            //List<UpdateWIHDataMUS> MUSResult = new List<UpdateWIHDataMUS>();
            foreach (var mail in CreatedMails)
            {
                #region Парсим данные из письма

                string WIHId = WIHInteractor.GetWIHId(mail.Subject);
                if (string.IsNullOrEmpty(WIHId))
                {
                    TaskParameters.TaskLogger.LogError("Не удалось распарсить WIH Id.");
                    continue;
                }
                TaskParameters.TaskLogger.LogInfo("WIH Id - " + WIHId);
                string fileName = WIHInteractor.GetSingleLineResultByRegex(mail.Body, "ShortDescription :");
                if (string.IsNullOrEmpty(fileName))
                {
                    TaskParameters.TaskLogger.LogError("Не удалось распарсить имя файла.");
                    continue;
                }
                TaskParameters.TaskLogger.LogInfo("Имя файла - " + fileName);
                string internalMailType = WIHInteractor.GetSingleLineResultByRegex(mail.Body, "Description :");
                if (string.IsNullOrEmpty(internalMailType))
                {
                    TaskParameters.TaskLogger.LogError("Не удалось распарсить тип письма.");
                    continue;
                }
                TaskParameters.TaskLogger.LogInfo("Тип письма - " + internalMailType);
                #endregion
                #region На основе типа письма заносим WIH Id в ту или иную таблицу

                switch (internalMailType)
                {
                    case WIHInteract.Constants.InternalMailTypeTOPOR:
                        {
                            string porName = fileName;

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHnumber = WIHId,
                                WIHrequests = fileName,
                                Type = WIHInteract.Constants.InternalMailTypeTOPOR

                            });
                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeTOPORDel:
                        {
                            string porName = fileName;

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHnumber = WIHId,
                                WIHrequests = fileName,
                                Type = WIHInteract.Constants.InternalMailTypeTOPORDel

                            });
                            break;
                        }


                    case WIHInteract.Constants.InternalMailTypeTORecall:
                        {
                            string porName = fileName;

                            PORResult.Add(new ShWIHRequest
                            {
                                WIHnumber = WIHId,
                                WIHrequests = fileName,
                                Type = WIHInteract.Constants.InternalMailTypeTORecall
                            });
                            break;
                        }
                    case WIHInteract.Constants.InternalMailTypeSAPCODE:
                        {
                            var request = TaskParameters.Context.WIHRequests.FirstOrDefault(wr => wr.Filename == fileName);
                            if (request != null)
                            {
                                request.ReceivedWIHNumberDate = DateTime.Now;
                                request.WIHNumber = WIHId;


                            }
                            else
                            {
                                continue;
                            }
                            break;
                        }
                    default:
                        {
                            continue;
                        }
                }
                #endregion
                //Только если все хорошо, перекидываем письмо в папку Created
                TaskParameters.Context.SaveChanges();
                TaskParameters.TaskLogger.LogInfo("Письмо обработано. Перемещаем в папку Created");
                redemtion.MoveToCreated(mail.ConversationId);   
            }


            #region Загружаем данные в СХ

            if (PORResult.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(PORResult) });
            }
            //if (MUSResult.Count > 0)
            //{
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(MUSResult) });
            //}
            #endregion
        }

    

        private class UpdateWIHDataPOR
        {
            public string ShPOR { get; set; }
            public string WiHIdPOR { get; set; }
            public string PONumber { get; set; }
            public DateTime? ApprovedDate { get; set; }
            public string RejectReason { get; set; }
            public DateTime? RejectedDate { get; set; }
        }
        private class UpdateWIHDataMUS
        {
            public string ShMUS { get; set; }
            public string WiHIdMUS { get; set; }
            public DateTime? ApprovedDate { get; set; }
            public DateTime? RejectedDate { get; set; }
            public string RejectReason { get; set; }
        }
        private class UpdateWBS
        {
            public string ShWBS { get; set; }
            public string Network { get; set; }
            public string SO { get; set; }
            public string WBS { get; set; }
        }

        private class PorCompletedImportModel
        {
            public string TO { get; set; }
            public string PO {get;set;}
            public bool Recall { get; set; }
            public string RecallComment { get; set; }
        }

        

    }
}