using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.IO;
using DbModels.DataContext;

using ExcelParser.Extentions;
using UnidecodeSharpFork;
using WIHInteract;
using MailProcessing;
using DbModels.DomainModels.WIH;
using CommonFunctions;
using EpplusInteract;



namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
   public class SendWIHSAPCodeRequest:ATaskHandler
    {
     public SendWIHSAPCodeRequest(TaskParameters taskParameters) : base(taskParameters) { }


     private static readonly string TemplatePath = @"\\RU00112284\p\OrderTemplates\SolarisTemplates\SapCodeRequest.xlsx";
        public override bool Handle()
        {
            //WIHMailInformation mailInf = new WIHMailInformation();

            //WIHInteractor.SendMailToWIH(mailInf);


            //if (!Directory.Exists(TaskParameters.DbTask.EmailSendFolder))
            //{
            //    try
            //    {
            //        Directory.CreateDirectory(TaskParameters.DbTask.EmailSendFolder);
            //        TaskParameters.TaskLogger.LogError(string.Format("Создана папка для отпавки почты}", TaskParameters.DbTask.EmailSendFolder));
            //    }
            //    catch (System.Exception ex)
            //    {
            //        TaskParameters.TaskLogger.LogError(string.Format("Папка для отпавки почты не существует и создать ее не вышло:{0}, error:{1}", TaskParameters.DbTask.EmailSendFolder, ex.Message));
            //        return false;
            //    }
            //}







           // using (Context context = new Context())
            var context = TaskParameters.Context;
            {
                //получаем неотосланные сап коды
                List<SapCodeViewModel> scvModel = StaticHelpers.GetStoredProcDataFromContext<SapCodeViewModel>(context, "SendSapCodes", null);

                // проверим сап кода для исключения дублей.
                var groups = scvModel.GroupBy(g => g.Code);
                var grMore1 = groups.Where(g => g.Count() > 1);
                if (grMore1.Count()>0)
                {
                    TaskParameters.TaskLogger.LogError($"Ошибка. Дубликаты сап кодов.{string.Join(",", grMore1.Select(g=>g.Key).ToList())}");
                    return false;
                }

                string emailId = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = string.Format("newSapCodes({0}).xlsx", emailId);

                TaskParameters.TaskLogger.LogDebug(string.Format("Новых сап кодов:({0})", scvModel.Count));

                if (scvModel.Count > 0)
                {
                    try
                    {
                        //транслитерируем
                        foreach (var scvm in scvModel)
                        {


                            var transliterated = scvm.Description.Unidecode();
                            scvm.Description = transliterated.Substring(0, transliterated.Length > 40 ? 40 : transliterated.Length);



                        }
                        // сохраняем в архивную папку
                        EpplusService service = new EpplusService(new FileInfo(TemplatePath));
                        service.InsertTableToPatternCellInWorkBook("Table", scvModel.ToDataTable(typeof(SapCodeViewModel)), new EpplusService.InsertTableParams() { PrintHeaders = false });
                        service.CreateFolderAndSaveBook(TaskParameters.DbTask.ArchiveFolder, fileName);
                        TaskParameters.TaskLogger.LogDebug(string.Format("Создан файл:{0}", Path.Combine(TaskParameters.DbTask.ArchiveFolder, fileName)));
                        
                        
                        //Посылаем письмо в WIH
                        string internalMailType = "SapCodeRequest";
                        WIHMailInformation mailInf = new WIHMailInformation();
                        mailInf.InternalMailType = internalMailType;
                        mailInf.FilePath = Path.Combine(TaskParameters.DbTask.ArchiveFolder, fileName);
                        mailInf.MailBoxSigmun="ESOLARIS";
                        mailInf.Project = "MS-SOLARIS";
                        mailInf.Subject = "Sap Code Request";
                        mailInf.Email = "technical.box.for.solaris@ericsson.com";
                        mailInf.ResponsibleTeam = "ROD Sofia";
                        mailInf.SystemComponent = "MasterData";
                        mailInf.CertificationCode = "L2302RODSofia_AO";
                        
                        WIHInteractor.SendMailToWIHRussia(mailInf,"SOLARIS");

                        foreach (var sc in scvModel)
                        {
                            var codes = context.SAPCodes.Where(sco => sco.Code == sc.Code).ToList();
                            if(codes.Count>1)
                            {
                                TaskParameters.TaskLogger.LogError($"Ошибка! Сапкодов {sc.Code} больше одного. ");
                                Console.ReadLine();
                            }

                            var code = codes.FirstOrDefault();
                            if (code != null)
                            {
                                code.ExistedInSAP = true;
                                code.EmailId = fileName;
                                context.SaveChanges();
                            }

                        }
                        // создаем запись о запросе в вих.
                        WIHRequest request = new WIHRequest();
                        request.Filename = fileName;
                        request.SendDate = DateTime.Now;
                        request.Type = internalMailType;
                        context.WIHRequests.Add(request);
                        context.SaveChanges();
                    }
                    catch (System.Exception ex)
                    {
                        TaskParameters.TaskLogger.LogError(string.Format("Ошибка при создании файла новых сап кодов:{0}, error:{1}", Path.Combine(TaskParameters.DbTask.EmailSendFolder, fileName), ex.Message));
                        return false;
                    }

                }


            }
                
            
            return true;

        }
        public class SapCodeViewModel
        {
            public string Code { get; set; }
            public string MaterialType { get; set; }
            public string Plant { get; set; }
            public string Description { get; set; }
            public string PC { get; set; }
            public string MaterialGroup { get; set; }
            public string ItemCategoryGroup { get; set; }
            public string ONEPCode { get; set; }
            public string PurchasingGroup { get; set; }
            public string ValuationClass { get; set; }
            public decimal Price { get; set; }
            public string Vendor { get; set; }

        }
    }
    }

