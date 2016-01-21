using AutoImport.Rev3.ImportHandlers.Abstract;
using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    public class ActCreateAutoimportHandler : IAutoImportHandler
    {
        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // экземпляр юнирепорта
            var rows = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 7);
            // считали объекты из эксель файла
            var actItems = new List<DbModels.SharedModels.ActItemModel>();
            string to = string.Empty;
            DateTime startDate;
            DateTime endDate;

            if (rows == null || rows.Count == 0)
            {
                hr.ErrorsList.Add("Ошибка работы с файлом. Проверьте его формат и содержимое.");
            }
            foreach (var row in rows)
            {
                var actModel = new DbModels.SharedModels.ActItemModel();
                decimal quantity = 0m;
                if (!decimal.TryParse(row.Column3, out quantity))
                {
                    continue;
                }
                actModel.Id = row.Column1;
                actModel.Quantity = quantity;
                actModel.Checked = true;
                actItems.Add(actModel);
            }
            // теперь надо считать отдельные ячейки из файла автоимпорта
            using(EpplusService service = new EpplusService(attachment.FilePath))
            {
                var sheet = service.GetSheet("Template");
                to = sheet.Cells[2, 2].Text;
                var startDateText = sheet.Cells[3, 2].Text;
                var endDateText = sheet.Cells[3, 5].Text;
                if(!DateTime.TryParse(startDateText, out startDate)||!DateTime.TryParse(endDateText, out endDate))
                {
                    hr.ErrorsList.Add(string.Format("Не удалось распознать дату начала или окончания работ:{0}-{1}", startDateText, endDateText));
                    return hr;
                }

            }


            using(Context context = new Context())
            {
            ActRepository repository = new ActRepository(context);
            try
            {
                var satAct = repository.GetSatAct(to, attachment.Mail.Sender, startDate, endDate,false);
                var satServices = repository.GetSATServices(to, actItems,false);
                repository.CreateAct(satAct, satServices, null);
                hr.Success=true;
                string.Format(@"Файл для автоимпорта создан.", satAct.ActName);
            }
            catch (Exception ex)
            {

                hr.ErrorsList.Add(ex.Message); 
            }
            }
            return hr;

        }
    }
}
