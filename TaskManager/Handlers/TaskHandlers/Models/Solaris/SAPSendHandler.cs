using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.IO;
using DbModels.DataContext;

using ExcelParser.Extentions;
using UnidecodeSharpFork;
using CommonFunctions;
using EpplusInteract;

namespace TaskManager.Handlers.TaskHandlers.Models.Solaris
{
    public class SAPSendHandler : ATaskHandler
    {
        public SAPSendHandler(TaskParameters taskParameters) : base(taskParameters) { }
        private static readonly string TemplatePath = @"\\E7643150A11C3F\SolarisTemplates$\SapCodeRequest.xlsx";
        public override bool Handle()
        {
            if (!Directory.Exists(TaskParameters.DbTask.EmailSendFolder))
            {
                try
                {
                    Directory.CreateDirectory(TaskParameters.DbTask.EmailSendFolder);
                    TaskParameters.TaskLogger.LogError(string.Format("Создана папка для отпавки почты}", TaskParameters.DbTask.EmailSendFolder));
                }
                catch (System.Exception ex)
                {
                    TaskParameters.TaskLogger.LogError(string.Format("Папка для отпавки почты не существует и создать ее не вышло:{0}, error:{1}",TaskParameters.DbTask.EmailSendFolder,ex.Message));
                    return false;
                }
            }





           
            
            using (Context context = new Context())
                {
                 List<SapCodeViewModel> scvModel = StaticHelpers.GetStoredProcDataFromContext<SapCodeViewModel>(context,"SendSapCodes",null);
                string emailId = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string  fileName = string.Format("newSapCodes({0}).xlsx",emailId);

                    TaskParameters.TaskLogger.LogDebug(string.Format("Новых сап кодов:({0})", scvModel.Count));

               

                    if (scvModel.Count > 0)
                    {
                        try
                        {
                            foreach (var scvm in scvModel)
                            {
                               
                               
                                    var transliterated = scvm.Description.Unidecode();
                                    scvm.Description = transliterated.Substring(0, transliterated.Length>40?40:transliterated.Length);
                                
                                
                               
                            }

                            EpplusService service = new EpplusService(new FileInfo(TemplatePath));

                            service.InsertTableToPatternCellInWorkBook("Table",scvModel.ToDataTable(typeof(SapCodeViewModel)), new EpplusService.InsertTableParams() { PrintHeaders = false});
                            service.CreateFolderAndSaveBook(TaskParameters.DbTask.EmailSendFolder, fileName);
                            TaskParameters.TaskLogger.LogDebug(string.Format("Создан файл:{0}", Path.Combine(TaskParameters.DbTask.EmailSendFolder, fileName)));
                            foreach (var sc in scvModel)
                            {
                                var code = context.SAPCodes.FirstOrDefault(sco => sco.Code == sc.Code);
                                code.ExistedInSAP = true;
                                code.EmailId = emailId;

                            }
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
