using MailProcessing;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Putevie
{
    public class PutevieImportHandler : ATaskHandler
    {
        public PutevieImportHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

      

        public override bool Handle()
        {
            var waylistImport = new List<WaylistImport>();
            var redemption = new RedemptionMailProcessor("SOLARIS");
            var mails = redemption.GetMails(new List<string> { "#путевой#" });
            foreach (var mail in mails)
            {

                // берем первый файл из письма. условились что на одну машину и одну дату одно письмо
                // из этого первого файла узнаем машину и дату и пытаемся их обработать
                bool mailHandled = true;
                var testAttachment = mail.Attachments.FirstOrDefault(a=>a.File.Contains("_"));
                if (testAttachment == null)
                    continue;
                var fileName = Path.GetFileNameWithoutExtension(testAttachment.File);
                var parts = fileName.Split(new char[] {'_' });
                if (parts.Count() != 2)
                {
                    continue;
                }
                var carPart = parts[0];
                var datePart = new string(parts[1].Where(c=>Char.IsDigit(c)).Take(7).ToArray());

                string carNum = string.Empty;
                bool letBool = true;
                bool digbool = true;

                // машину определяем по первым трем цифрам
                foreach (var ch in carPart.Where(c=>c!=' '))
                {
                    if(char.IsDigit(ch))
                    {
                        carNum += ch;
                    }
                    if (carNum.Length > 2)
                        break;
                }

                var shCar = TaskParameters.Context.ShCars.FirstOrDefault(c => c.CarId.Contains(carNum));
                if (shCar == null)
                {
                    continue;

                }
                DateTime date;
                if (!DateTime.TryParseExact(datePart, "MMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                  continue;
                }

                var wayListName = $"{carNum} {date.ToString("yyyy.MM")}";
                var shWaylist = TaskParameters.Context.ShWaylists.FirstOrDefault(w=>w.Waylist==wayListName);
                if(shWaylist==null)
                {
                    var import = new WaylistImport();
                    import.Car = shCar.CarId;
                    import.Name = fileName;
                    import.Date = date;
                    waylistImport.Add(import);
                    continue;
                }

                List<string> fileFields = new List<string> {"Файл1","Файл2","Файл3","Файл4" };
                int index = 0;
                StringBuilder results = new StringBuilder();
                foreach (var attach in mail.Attachments)
                {
                    var field = fileFields[index];
                    var result = SHInteract.Handlers.Solaris.WayListUpload.Upload(wayListName, attach.FilePath, field);
                    results.AppendLine(result);
                    // прогружаем файл
                    index++;
                }    
                    


                
                

                if (mailHandled)
                {
                    redemption.MoveToSuccess(mail.ConversationId);
                    // ответить
                    redemption.SendMail(
                        new AutoMail
                        {
                            Email = mail.Email,
                            Body = $"Файлы по '{shCar.CarId}' за '{date.ToString("MM.yyyy")}' успешно прогружены.",
                            Subject = "Re. Путевые"


                        }

                        );

                }

            }
            if (waylistImport.Count > 0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(waylistImport) });

            return true;
        }

        class WaylistImport
        {
            public string Name { get; set; }
            public string Car { get; set; }
            public DateTime? Date { get; set; }

        }
    }
}
