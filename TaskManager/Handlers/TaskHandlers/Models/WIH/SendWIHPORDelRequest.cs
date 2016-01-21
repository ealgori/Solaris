
using DbModels.DomainModels.ShClone;
using DbModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using TaskManager.Service;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.SAT;
using System.IO;
using WIHInteract;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
   /// <summary>
   /// Хендлер на отправку ПОдел
   /// Друг, если ты ищешь, где генерируются обычные поры и торы, то генерация самих файлов
   /// Причина в том, что превьюшки должны генерироваться с сайта,а  достучаться с сайта для хендлера довольно сложно
   /// 
   /// </summary>
   public class SendWIHPORDelRequests:ATaskHandler
   {
       public SendWIHPORDelRequests(TaskParameters taskParameters)
           : base(taskParameters) { }
       public override bool Handle()
       {
           // выбираем эгрименты, которые готовы к отправке
           var shAgreem = TaskParameters.Context.ShAddAgreements.Where(a => a.SendAddAgreement == true);
           AgreementRepository reposit = new AgreementRepository(TaskParameters.Context);
           var agreemImportModels = new List<AgreemImportModel>();
           List<ShWIHRequest> requestList = new List<ShWIHRequest>();
           foreach (var agreem in shAgreem)
           {
               var items = reposit.GetAgreementItems(agreem.AddAgreement);
               if (items.Count == 0)
               {
                   AddImportModel(agreem.AddAgreement, true, string.Format("На эгрименте нет ни одной позиции"), agreemImportModels);
                   continue;
               }

               var now = DateTime.Now;
               //// среди готовых, выбираем те, у которых все ок с реквестами
               //// для этого нам нужен экземпляр ТО
               var randomItem = items.FirstOrDefault();
                
               //у ТО должен быть пор в состоянии комплитед.
               if(!WIHService.TOHasCompletedRequest(randomItem.TOId,WIHInteract.Constants.InternalMailTypeTOPOR,TaskParameters.Context))
               {
                   AddImportModel(agreem.AddAgreement, true, string.Format("На ТО должны быть поры в состоянии комплитед."), agreemImportModels);
                   continue;
               }
                
               if(WIHService.ReadySendTOWIHRequest(randomItem.TOId,WIHInteract.Constants.InternalMailTypeTOPORDel,TaskParameters.Context,agreem.AddAgreement))
               {

                   string error;
                   var porBytes = ExcelParser.EpplusInteract.CreatePorDel.GenerateDelPOR(agreem.AddAgreement,false,out error );
                   if(porBytes==null)
                   {
                       AddImportModel(agreem.AddAgreement, true, error, agreemImportModels);
                       continue;
                   }
                   var torBytes = ExcelParser.EpplusInteract.CreateTORequestDel.Create(agreem.AddAgreement, false, out error);
                   if(torBytes==null)
                   {
                       AddImportModel(agreem.AddAgreement, true, error, agreemImportModels);
                       continue;
                   }

                   var archive = Path.Combine(TaskParameters.DbTask.ArchiveFolder, now.ToString(@"yyyy\\MM\\dd"));
                   if (!Directory.Exists(archive))
                   {
                       try
                       {
                           Directory.CreateDirectory(archive);
                       }
                       catch (Exception exc)
                       {
                           TaskParameters.TaskLogger.LogError(string.Format("Ошибка создания папки  '{0}'; {1}", archive, exc.Message));
                           continue;
                       }
                   }
                    
                   string fileName = GenerateTOPorName(now);
                   if (fileName.Length > 42)
                   {
                       TaskParameters.TaskLogger.LogError(string.Format("Название файла больше 42 символов '{0}'", fileName));
                       continue;
                   }
                   string fileName1 = GeneratedTORequestName(now);

                   var filePath = Path.Combine(archive, fileName);
                   if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, porBytes))
                   {
                       TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath));
                       continue;
                   }
                   var filePath1 = Path.Combine(archive, fileName1);
                   if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath1, torBytes))
                   {
                       TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath1));
                       continue;
                   }


                   string internalMailType = WIHInteract.Constants.InternalMailTypeTOPORDel;
                   WIHMailInformation mailInf = new WIHMailInformation();
                   mailInf.InternalMailType = internalMailType;
                   mailInf.FilePath = filePath;
                   mailInf.FilePath2 = filePath1;
                   mailInf.MailBoxSigmun = "ESOLARIS";
                   mailInf.Project = "MS-SOLARIS";
                   mailInf.Subject = "PO WIH Request";
                   mailInf.Email = "technical.box.for.solaris@ericsson.com";
                   mailInf.ResponsibleTeam = "ROD Sofia";
                   mailInf.SystemComponent = "Purchase Order Request (POR)";
                   mailInf.CertificationCode = "L2302RODSofia_AO";

                   var result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS");
                  // var result = "ok";
                   if (string.IsNullOrEmpty(result) || (string.IsNullOrWhiteSpace(result)))
                   {
                       TaskParameters.TaskLogger.LogError(string.Format("Функция отправки письма не вернула ConversationIndex "));
                   }
                   else
                   {

                       requestList.Add(new ShWIHRequest() {AddAgreementId=agreem.AddAgreement , TOid = randomItem.TOId, WIHrequests = fileName, RequestSentToODdate = now, Type = WIHInteract.Constants.InternalMailTypeTOPOR });
                       AddImportModel(agreem.AddAgreement, false, string.Format("Отправлен {0}", DateTime.Now.ToString("dd-MM-yyyy")), agreemImportModels);
                   }
                    
               
                  
                    

                   // отсылаем запросы в вих
                   // делаем импорт
               }
            
            
            
           }
           if (requestList.Count > 0)
           {


              // TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
           }
           if (agreemImportModels.Count>0)
           {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(agreemImportModels) });
               
           }

           return true;
          
           
       }

       private class AgreemImportModel 
       {
           public string Agreement { get; set; }
           public bool ReadyToSend { get; set; }
           public string Comment { get; set; }
       }

       private void AddImportModel(string agreement, bool readyToSend, string comment, List<AgreemImportModel> agreemImportModels)
       {
           var model = new AgreemImportModel();
           model.Agreement = agreement;
           model.ReadyToSend = readyToSend;
           model.Comment = comment;
           agreemImportModels.Add(model);
       }

       public string GenerateTOPorName(DateTime date)
       {
           return string.Format("DEL-POR-TO-{0}.xlsx", date.ToString("yyyyMMdd_HHmmss_ffff"));
       }
       public string GeneratedTORequestName(DateTime date)
       {
           return string.Format("DEL-TOR-TO-{0}.xlsx", date.ToString("yyyyMMdd_HHmmss_ffff"));
       }
        
   }
}
