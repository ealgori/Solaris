using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.FileImportHandlers;
using DbModels.DataContext;
using System.IO;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomFiHandlers
{
    public class TOItemFilesDownloadHandler : IFileImportHandler
    {

        public HandlerResult Handle(global::Models.AutoMail amail)
        {

            //string rootPath = @"\\Ev001b78bfe400\Solaris files from SH";
            string rootPath = @"\\RU00112284\Downloads";

            HandlerResult result = new HandlerResult();
            if (!Directory.Exists(rootPath))
            {
                result.ErrorsList.Add(string.Format("Папка для сохранения файлов не существует. Обратитесь к администратору данного тула"));
                return result;
            }
            var attachments = amail.Attachments.Where(a => Path.GetExtension(a.File) == ".xls");
            using (Context context = new Context())
            {
                foreach (var attach in attachments)
                {
                    UniReport.UniReportBulkCopy<ToItemModel> report = new UniReport.UniReportBulkCopy<ToItemModel>(attach.FilePath);
                    // считали объекты из эксель файла
                    var _obj = report.ReadFile();
                    if (_obj == null || _obj.Count == 0)
                    {
                        result.ErrorsList.Add(string.Format("Из файла не удалось считать ни одного элемента"));
                        return result;
                    }
                    var now = DateTime.Now;
                    var datedDir = Path.Combine(rootPath, string.Format(@"{0}.{1}.{2}-{3}\", now.Year, now.Month, now.Day, now.ToString("HHmm")));
                    result.FormatedResults.Add(string.Format("<a href='{0}'>{0}</a>",datedDir));
                    foreach (var item in _obj)
                    {
                        var shToItem = context.ShTOItems.FirstOrDefault(i => i.TOItem == item.ItemId);
                        if (shToItem != null)
                        {
                            datedDir = Path.Combine(rootPath, string.Format(@"{0}.{1}.{2}-{3}\{4}\", now.Year, now.Month, now.Day, now.ToString("HHmm"), shToItem.TOId));
                            if (!Directory.Exists(datedDir))
                            {
                                try
                                {
                                    Directory.CreateDirectory(datedDir);
                                }
                                catch(Exception exc)
                                {
                                    result.ErrorsList.Add(string.Format("Папка для сохранения файлов не существует. Обратитесь к администратору данного тула"));
                                    break;
                                }
                            }
                            var downloadResult  = SHInteract.Handlers.Solaris.FileDownload.Handle(item.ItemId);
                            result.FormatedResults.Add(downloadResult.Result);
                            foreach (var file in downloadResult.FilesDictionary)
                            {
                                string fileName = string.Format("{0}-{1}-{2}",shToItem.Site,shToItem.TOItem,file.Key);
                                if(!CommonFunctions.StaticHelpers.ByteArrayToFile(Path.Combine(datedDir,fileName),file.Value))
                                {
                                    result.ErrorsList.Add(string.Format("Ошибка сохранения файла. Обратитесь к администратору данного тула"));
                                }
                            }
                            
                        }
                        else
                        {
                            result.ErrorsList.Add(string.Format("Объект не найден в сх: {0}",item.ItemId));
                        }
                    }


                }
            }
            result.Success = true;
            return result;
        }

        private class ToItemModel
        {
            public string ItemId { get; set; }
        }
    }
}
