using AutoImport.Rev3.ImportHandlers.Abstract;
using DbModels.DataContext;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    /// <summary>
    /// Хендлер для менеджмента адресов рассылки по неутвержденным авр
    /// </summary>
    public class AVRRecipientsHandler : IAutoImportHandler
    {

        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {

            string templatePath = @"\\RU00112284\Solaris\AutoImportTemplates\AVRRecipientsTemplate.xlsx";
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // экземпляр юнирепорта
            var rows = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 2);
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
                {
                    var model = new ImportModel();
                    model.Name = row.Column1.Trim();
                    model.RukOtdelaEmail = row.Column2;
                    model.RukFillialaEmail = row.Column3;
                    model.POPOREmail = row.Column4;
                    models.Add(model);
                }
            }

            using (Context context = new Context())
            {
                foreach (var model in models)
                {
                    var satsubregion = context.SATSubregions.FirstOrDefault(s => s.Name == model.Name);
                    if (satsubregion == null)
                    {
                        // если такого сабрегиаона нет, надо проверить, сущетсвует ли он вообще.
                        var shAvr = context.ShAVRs.FirstOrDefault(s => s.Subregion == model.Name);
                        if (shAvr == null)
                        {
                            // добавляем в лог, что извини чувак, но мы таких сабрегионов в глаза не видели
                            hr.InfoList.Add(string.Format("Указанный Subregion не используется ни на одном АВР: {0}", model.Name));
                        }
                        else
                        {
                            satsubregion = new DbModels.DomainModels.SAT.SATSubregion();
                            satsubregion.Name = model.Name;
                            satsubregion.RukOtdelaEmail = model.RukOtdelaEmail;
                            satsubregion.RukFillialaEmail = model.RukFillialaEmail;
                            satsubregion.POROREmail = model.POPOREmail;
                            context.SATSubregions.Add(satsubregion);
                            hr.InfoList.Add(string.Format("Добавлен новый Subregion: {0}", model.Name));
                            // добавляем лог что внесен новый сабрегион
                        }
                    }
                    else
                    {

                        bool updated = false;
                        if (satsubregion.RukOtdelaEmail != model.RukOtdelaEmail ||
                        satsubregion.RukFillialaEmail != model.RukFillialaEmail ||
                        satsubregion.POROREmail != model.POPOREmail)
                        {
                            // если такой сабрегион уже есть, надо обновить его адресатов
                            satsubregion.RukOtdelaEmail = model.RukOtdelaEmail;
                            satsubregion.RukFillialaEmail = model.RukFillialaEmail;
                            satsubregion.POROREmail = model.POPOREmail;
                            hr.InfoList.Add(string.Format("Обновлены адресаты в Subregion: {0}", model.Name));
                        }
                    }
                }
                context.SaveChanges();

                // в ответ на письмо будет присылать заполенный шаблон с актуальной информацией.
                if (File.Exists(templatePath))
                {
                    try
                    {
                        var dataTable = context.SATSubregions.Select(s => new {
                        Name=s.Name
                        ,RulOtdela= s.RukOtdelaEmail
                        ,RukFiliala = s.RukFillialaEmail
                        ,PORPO = s.POROREmail

                        }).ToList().ToDataTable();
                        using (EpplusInteract.EpplusService service = new EpplusService(templatePath))
                        {
                            var insertParameters = new EpplusService.InsertTableParams() { CopyFirstRowStyle = true };
                            service.InsertTableToPatternCellInWorkBook("subregions", dataTable, insertParameters);
                            var fileSavePath = Path.Combine(savePath,"ActualAVRRecipientsTemplate.xlsx");
                            service.CreateFolderAndSaveBook(fileSavePath);
                            hr.ResultFiles.Add(fileSavePath);
                            
                        }
                    }
                    catch (Exception)
                    {

                        hr.InfoList.Add("Извините, что-то пошло не так и шаблон не сгенерирован...");
                    }
                }
                else
                {
                    hr.InfoList.Add("Извините, что то пошло не так и шаблон не сгенерирован...");
                }



            }




            hr.Success = true;

            return hr;
        }

        public class ImportModel
        {
            public string Name { get; set; }
            public string RukOtdelaEmail { get; set; }
            public string RukFillialaEmail { get; set; }
            public string POPOREmail { get; set; }
        }

    }
}
