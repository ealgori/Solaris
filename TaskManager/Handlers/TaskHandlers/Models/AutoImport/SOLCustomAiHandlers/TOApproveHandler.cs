using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoImport.Rev3.ImportHandlers.Abstract;
using CommonFunctions.Extentions;
namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
   // 100% нельзя пускать без обновления
    public class TOApproveHandler:IAutoImportHandler
    {
        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // экземпляр юнирепорта
            UniReport.UniReportBulkCopy<ApproveModel> report = new UniReport.UniReportBulkCopy<ApproveModel>(attachment.FilePath);
            // считали объекты из эксель файла
            var _obj = report.ReadFile();
            if(_obj==null)
            {
                hr.ErrorsList.Add("Ошибка работы с файлом. Проверьте его формат и содержимое. Заголовки являются обязатльными.");
            }
            // конвертируем их в дататэйбл, чтобы воспользоваться существующим функционалом
            var obj = _obj.Select(s => new ApproveExtModel() { TOItemId = s.TOItemId, TRUE = "TRUE" }).ToList();
          
            var dataTable = obj.ToDataTable();
            // создаем новую рабочую книгу
            var wb = NpoiInteract.GetNewWorkBook();
            // встваляем в нее данные и создаем в ней новый шит
            NpoiInteract.FillReportData(dataTable, "sheet1", wb);
            // сохраняем это все по пути назначения
            var fileSavePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AutoTOItemAppove", attachment.Id,".xls"));
            NpoiInteract.SaveReport(fileSavePath, wb);

            var extObj = obj.Select(o => new ApproveExtModel() { TOItemId = o.TOItemId, Date = DateTime.Now, FIO = attachment.Mail.Sender });
            var extwb = NpoiInteract.GetNewWorkBook();
            // встваляем в нее данные и создаем в ней новый шит
            NpoiInteract.FillReportData(extObj.ToList().ToDataTable(), "sheet1", extwb);
            // сохраняем это все по пути назначения
            var extSavePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AutoTOItemAppove(2)", attachment.Id, ".xls"));
            NpoiInteract.SaveReport(extSavePath, extwb);



            hr.Success = true;
            // возвращаем коллекцию файлов для обработки
            hr.FilesPaths.Add(fileSavePath);
            hr.FilesPaths.Add(extSavePath);



            return hr;
        }

        private class ApproveModel
        {
            public string TOItemId { get; set; }
         //   public string TRUE { get; set; }
        }

        private class ApproveExtModel
        {
            public string TOItemId { get; set; }
            public string TRUE { get; set; }
            public string FIO { get; set; }
            public DateTime? Date { get; set; }
        }
    }
}
