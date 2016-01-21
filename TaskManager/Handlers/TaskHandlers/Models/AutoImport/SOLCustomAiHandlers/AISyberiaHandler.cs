using AutoImport.Rev3.ImportHandlers.Abstract;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;
using DbModels.DataContext;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    public class AISyberiaHandler : IAutoImportHandler
    {
        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // экземпляр юнирепорта
            var rows = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 6);
            // считали объекты из эксель файла

            if (rows == null || rows.Count == 0)
            {
                hr.ErrorsList.Add("Ошибка работы с файлом. Проверьте его формат и содержимое.");
            }
            List<ImportModel> models = new List<ImportModel>();

           // using (Context context = new Context())
           // {
             



                foreach (var row in rows)
                {
                    if(row.Column6.ToUpper()=="TRUE")
                    {
                        var model = new ImportModel();
                        model.TOItemId = row.Column1;
                        model.TOId = row.Column2;
                        model.SiteId = row.Column3;
                        model.DateTOPlan = row.Column4;
                        model.DateTOFact = row.Column5;
                        model.FactVipolnRabot = row.Column6;
                        model.LinkToReportInEridoc = row.Column7;
                        model.DateTOBaseline = row.Column8;
                        model.FactVipolnRabotUtvEricBy = attachment.Mail.Author;

                    }
                }
           
            var dataTable = models.ToDataTable();
            // создаем новую рабочую книгу
            var wb = NpoiInteract.GetNewWorkBook();
            // встваляем в нее данные и создаем в ней новый шит
            NpoiInteract.FillReportData(dataTable, "sheet1", wb);
            // сохраняем это все по пути назначения
            var fileSavePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AISyberia", attachment.Id, ".xls"));
            NpoiInteract.SaveReport(fileSavePath, wb);





            hr.Success = true;
            // возвращаем коллекцию файлов для обработки
            hr.FilesPaths.Add(fileSavePath);
            hr.FilesPaths.Add(fileSavePath);




            return hr;
        }
        public class ImportModel
        {
            public string TOItemId { get; set; }
            public string TOId { get; set; }
            public string SiteId { get; set; }
            public string DateTOPlan { get; set; }
            public string DateTOFact { get; set; }
            public string FactVipolnRabot { get; set; }
            public string LinkToReportInEridoc { get; set; }
            public string DateTOBaseline { get; set; }
            public string FactVipolnRabotUtvEricBy { get; set; }
        }
    }
   
}
