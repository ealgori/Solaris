using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Record;
using SHInteract;
using SHInteract.Handlers;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Putevie
{
    public class PutevieNotifierHandler : ATaskHandler
    {
        public PutevieNotifierHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

        public override bool Handle()
        {

            var shCars = TaskParameters.Context.ShCars.ToList();
            DateTime startDate = new DateTime(2016, 7, 1);
            DateTime endDate = DateTime.Now;
            var dateRange = CommonFunctions.Dates.GetMonthsRange(startDate, endDate);

            var fDownloadParams = new FileDownloadParams()
            {
                objectListObjectId = "524622",
                objectListObjectType = "1253",
                formName = "Путевые листы admin",
                c_dyntype = "1",
                currentMainObjType = "1253",
                currentSectionObjType = "1253",
                project = "SOLARIS",
                sectionNum = "4"

            };

            FileDownloader fileDownloader = new FileDownloader(fDownloadParams);
            var requiredPutevie = TaskParameters.Context.ShWaylists.Where(w => w.Required == "Yes").ToList();
            //  requiredPutevie = requiredPutevie.Where(p => p.Car.Contains("371")|| p.Car.Contains("352")).ToList();
            //   requiredPutevie = TaskParameters.Context.ShWaylists.Where(p => p.Waylist == "976 072016").ToList();
            foreach (var waylist in requiredPutevie)
            {
                // рассылка
                var shCar = shCars.FirstOrDefault(c => c.CarId == waylist.Car);
                if (shCar != null)
                {
                    var date = ExtractDateFromWaylistName(waylist.Waylist);

                    if (date.HasValue)
                        AddToDelivery(
                            shCar.Responsible.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            shCar.Manager.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            shCar.CarId, date.Value, waylist, fileDownloader);
                }
            }

            return true;
        }

        void AddToDelivery(List<string> recipients, List<string> ccRecipients,   string car, DateTime date, ShWaylist waylist,
            FileDownloader fileDownloader)
        {
            EmailParams param = new EmailParams(recipients, "#путевой#");
            {
                param.Recipients = recipients;
                param.CCRecipients = ccRecipients; // new List<string> { DistributionConstants.EgorovEmail };
            }
            //param.TestRecipients = $"{DistributionConstants.EgorovEmail}";
            param.AllowWithoutAttachments = true;

            string commentText = string.Empty;
            if (waylist != null && !string.IsNullOrEmpty(waylist.Comment))
                commentText = string.Format("Комментарий:{0}", waylist.Comment);
            var carNum = PutevieImportHandler.GetCarNum(car);

            // брали файлы из папки и загружали их в сх.
            //var files = Directory.GetFiles(TaskParameters.DbTask.EmailSendFolder, $"*{carNum}*_{date.ToString("MMyyyy")}*",SearchOption.TopDirectoryOnly);
            //foreach (var file in files)
            //{
            //    var newPath = CommonFunctions.StaticHelpers.GetDatedPath(TaskParameters.DbTask.EmailSendFolder);

            //    if (!Directory.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(newPath);
            //    }
            //    var newFilePath = Path.Combine(newPath, Path.GetFileName(file));
            //    var result = SHInteract.Handlers.Solaris.WayListUpload.Upload(waylist.Waylist, file, "Исходный Файл");


            //    File.Move(file,newFilePath);
            //    param.FilePaths.Add(newFilePath);
            //}


            // теперь задача обратная. взять файл из сх и добавить его в рассылку
            var newPath = CommonFunctions.StaticHelpers.GetDatedPath(TaskParameters.DbTask.EmailSendFolder);
            try
            {
                var files = fileDownloader.Download(waylist.Waylist, new List<string> { "Исходный Файл" }, "", true);
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        var newFilePath = Path.Combine(newPath, file.Key);
                        CommonFunctions.StaticHelpers.ByteArrayToFile(newFilePath, file.Value);
                        param.FilePaths.Add(newFilePath);
                    }
                }
            }
            catch (Exception)
            {

               return;
            }

            param.HtmlBody += string.Format(@"
<style>
pre {{
    font-size: 16px;
    font-family: 'Times New Roman', Times, serif;
}}
</style>
<pre>
Добрый день!

{2}

Прошу ответным письмом предоставить путевой лист за период {1} по автомобилю {0}, для этого необходимо:

1.       Заполнить путевой лист(включая страницу СЗ о расходе топлива)  в формате excel, название файла БЦЦЦББ_ММГГГГ.xls (где БЦЦЦББ – номер автомобиля, ММГГГГ – отчетные месяц и год)
2.       Распечатать путевой лист, добавить страницу с чеками ГСМ, на первой странице рядом с подписью отвественного написать фразу от руки «Путевой лист отправлен в бухгалтерию Эрикссон *указать число, месяц, год отправки и номер накладной*»
3.       Отсканировать путевой лист и лист с чеками ГСМ в формате pdf, название файла БЦЦЦББ_ММГГГГ.pdf (где БЦЦЦББ – номер автомобиля, ММГГГГ – отчетные месяц и год)
4.       Отправить на адрес technical.box.for.solaris@ericsson.com письмо с темой #путевой# вложив в него два файла: БЦЦЦББ_ММГГГГ.xls и БЦЦЦББ_ММГГГГ .pdf
5.       Отправить оригинал путевого листа в бумажном виде в бухгалтерию



Спасибо!
</pre>
"
, car
, date.ToString("MM.yyyy")

, commentText);
            param.HtmlBody += @"<br>";
            param.TestRecipients = DistributionConstants.EalgoriEmail;
           
            TaskParameters.EmailHandlerParams.EmailParams.Add(param);
        }

        DateTime? ExtractDateFromWaylistName(string waylistName)
        {
            var datePart = waylistName.Substring(waylistName.Length - 6);
            DateTime date;
            if (!DateTime.TryParseExact(datePart, "MMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
            {

                return null;
            }
            return date;
        }



    }
}
