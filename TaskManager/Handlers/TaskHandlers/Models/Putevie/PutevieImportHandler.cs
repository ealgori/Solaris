using MailProcessing;

using Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Putevie
{
    public class PutevieImportHandler : ATaskHandler
    {
        RedemptionMailProcessor Processor = new RedemptionMailProcessor("SOLARIS");
        public PutevieImportHandler(TaskParameters taskParams)
            : base(taskParams)

        {
           
        }



        public override bool Handle()
        {
            var now = DateTime.Now;
            var waylistImport = new List<WaylistImport>();
            var waylistRequiredImport = new List<WaylistRequiredImport>();
            var mails = Processor.GetMails(new List<string> { "#путевой#" });
            TaskParameters.TaskLogger.LogDebug($"Писем : {mails.Count}");

            foreach (var mail in mails)
            {

                try
                {


                    // берем первый файл из письма. условились что на одну машину и одну дату одно письмо
                    // из этого первого файла узнаем машину и дату и пытаемся их обработать
                    bool mailHandled = true;
                    var testAttachments = mail.Attachments.Where(a => a.File.Contains("_")&& Path.GetExtension(a.File).ToUpper()==".XLSX");
                    if (!testAttachments.Any())
                    {
                        AddErrorEmailMessage(mail);
                        continue;
                    }
                    TaskParameters.TaskLogger.LogDebug($"{mail.Author} - {Path.GetFileName(testAttachments.FirstOrDefault().File)}");

                    var fileName = Path.GetFileNameWithoutExtension(testAttachments.FirstOrDefault().File);
                    var parts = fileName.Split(new char[] { '_' });
                    if (parts.Count() != 2)
                    {
                        TaskParameters.TaskLogger.LogError($"неверный формат письма");
                        Processor.MoveToUnhandled(mail.ConversationId);
                        continue;
                    }
                    var carPart = parts[0];
                    var datePart = new string(parts[1].Where(c => Char.IsDigit(c)).Take(7).ToArray());
                    // машину определяем по первым трем цифрам
                    string carNum = GetCarNum(carPart);
                    bool letBool = true;
                    bool digbool = true;




                    var shCar = TaskParameters.Context.ShCars.FirstOrDefault(c => c.CarId.Contains(carNum));
                    if (shCar == null)
                    {
                        TaskParameters.TaskLogger.LogError($"Авто '{carNum}' не найдено");
                        continue;

                    }
                    DateTime date;
                    if (
                        !DateTime.TryParseExact(
                            datePart,
                            "MMyyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out date))
                    {
                        continue;
                    }

                    var wayListName = GetWaylistName(carNum, date);
                    if (!string.IsNullOrEmpty(wayListName))
                    {
                        PutevoiContent waylistContent = new PutevoiContent();
                        bool found = false;
                        foreach (var file in testAttachments)
                        {
                            if (TryReadPutevoi(file.FilePath, out waylistContent))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            // шлем ошибку
                            // Перемещаем письмо
                            AddErrorEmailMessage(mail);
                            continue;
                        }

                        var shWaylist = TaskParameters.Context.ShWaylists.FirstOrDefault(w => w.Waylist == wayListName);
                        if (shWaylist == null)
                        {
                            // Изначально импорт был только на Create
                            var import = new WaylistImport();

                            import.Car = shCar.CarId;
                            import.Name = wayListName;
                            import.Date = now;
                            import.MeterStart = waylistContent.MeterStart;
                            import.MeterEnd = waylistContent.MeterEnd;
                            import.Refill = waylistContent.Refill;
                            waylistImport.Add(import);
                            continue;
                        }
                        else
                        {
                            //было бы неплохо для существующих прогрузить этот пробег. но для этого пришлось сделать импорт еще и на апдэйт
                            var importMet = new WaylistImport();
                            importMet.Name = wayListName;
                            importMet.MeterStart = waylistContent.MeterStart;
                            importMet.MeterEnd = waylistContent.MeterEnd;
                            importMet.Refill = waylistContent.Refill;
                            waylistImport.Add(importMet);
                        }





                        List<string> fileFields = new List<string> { "Файл1", "Файл2", "Файл3", "Файл4" };
                        int index = 0;
                        StringBuilder results = new StringBuilder();
                        foreach (var attach in mail.Attachments)
                        {
                            var field = fileFields[index];
                            var result = SHInteract.Handlers.Solaris.WayListUpload.Upload(
                                wayListName,
                                attach.FilePath,
                                field);
                            results.AppendLine(result);
                            // прогружаем файл
                            index++;
                        }

                        if (shWaylist.Required == "Yes")
                        {
                            var import = new WaylistRequiredImport();
                            import.Name = wayListName;
                            waylistRequiredImport.Add(import);
                            continue;
                        }




                        if (mailHandled)
                        {
                            Processor.MoveToSuccess(mail.ConversationId);
                            // ответить
                            Processor.SendMail(
                                new AutoMail
                                {
                                    Email = DistributionConstants.EgorovEmail,
                                    Body =
                                            $"Файлы по '{shCar.CarId}' за '{date.ToString("MM.yyyy")}' успешно прогружены.",
                                    Subject = "Re. Путевые"
                                });
                        }
                    }
                }
                catch (Exception exc)
                {
                    TaskParameters.TaskLogger.LogError($"{exc.Message} - {exc.StackTrace}");
                }

            }
            if (waylistImport.Count > 0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(
                    new ImportParams
                    {
                        ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1,
                        Objects = new ArrayList(waylistImport)
                    });
            if (waylistRequiredImport.Count > 0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(
                    new ImportParams
                    {
                        ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2,
                        Objects = new ArrayList(waylistRequiredImport)
                    });

            return true;
        }

        public static bool TryReadPutevoi(string filePath, out PutevoiContent content)
        {
            content = new PutevoiContent();
            try
            {
                using (EpplusInteract.EpplusService service = new EpplusInteract.EpplusService(filePath))
                {
                    var ws = service.GetSheet("Расход бензина");
                    content.MeterStart = int.Parse(ws.Cells["BU30"].Text);
                    content.MeterEnd = int.Parse(ws.Cells["BU31"].Text);
                    content.Refill = decimal.Parse(ws.Cells["CU33"].Text);

                    return true;
                }
            }
            catch (Exception exc)
            {


            }
            return false;

        }

        private void AddErrorEmailMessage(AutoMail mail)
        {
            Processor.MoveToUnhandled(mail.ConversationId);

            #region ErrorMessage

            string message = @"
<style>
pre {{
    font-size: 16px;
    font-family: 'Times New Roman', Times, serif;
}}
</style>
<pre>
Добрый день!



Прошу ответным письмом предоставить путевой лист, для этого необходимо:

1.       Заполнить путевой лист(включая страницу СЗ о расходе топлива)  в формате excel файла приложенного к письму, название файла БЦЦЦББ_ММГГГГ.xls (где БЦЦЦББ – номер автомобиля, ММГГГГ – отчетные месяц и год)
2.       Распечатать путевой лист, добавить страницу с чеками ГСМ, на первой странице рядом с подписью отвественного написать фразу от руки «Путевой лист отправлен в бухгалтерию Эрикссон *указать число, месяц, год отправки и номер накладной*»
3.       Отсканировать путевой лист и лист с чеками ГСМ в формате pdf, название файла БЦЦЦББ_ММГГГГ.pdf (где БЦЦЦББ – номер автомобиля, ММГГГГ – отчетные месяц и год)
4.       Отправить на адрес technical.box.for.solaris@ericsson.com письмо с темой #путевой# вложив в него два файла: БЦЦЦББ_ММГГГГ.xls и БЦЦЦББ_ММГГГГ .pdf
5.       Отправить оригинал путевого листа в бумажном виде в бухгалтерию


Документ регламентирующий предоставление отчетных документов за топливо приложен к письму.

Спасибо!                            
</pre>
";
            #endregion

     Processor.SendMail(
                            new AutoMail
                            {
                                Email = mail.Email,
                                CCEmail = DistributionConstants.EgorovEmail,
                                Attachments = new List<Attachment>
                                                  {
                                                      new Attachment() { FilePath=TaskParameters.DbTask.TemplatePath},
                                                        new Attachment() { FilePath=TaskParameters.DbTask.TemplatePath2}
                                                  },
                                Body =  message,
                                Subject = "Re: "+mail.Subject
                            });
        }

        public class PutevoiContent
        {
            public int MeterStart { get; set; }

            public int MeterEnd { get; set; }

            public decimal Refill { get; set; }

        }

        public class WaylistImport
        {
            public string Name { get; set; }
            public string Car { get; set; }
            public DateTime? Date { get; set; }

            public int? MeterStart { get; set; }

            public int? MeterEnd { get; set; }

            public decimal? Refill { get; set; }

        }

        class WaylistRequiredImport
        {
            public string Name { get; set; }
            public string Required { get; set; }

        }

        public static string GetWaylistName(string carNum, DateTime date)
        {

            return $"{carNum} {date.ToString("MMyyyy")}";
        }

        public static string GetCarNum(string carName)
        {
            string carNum = string.Empty;
            foreach (var ch in carName.Where(c => c != ' '))
            {
                if (char.IsDigit(ch))
                {
                    carNum += ch;
                }
                if (carNum.Length > 2)
                    break;
            }
            return carNum;
        }
    }
}
