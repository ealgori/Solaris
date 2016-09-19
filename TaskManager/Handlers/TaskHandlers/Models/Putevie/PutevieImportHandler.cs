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
        public PutevieImportHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

      

        public override bool Handle()
        {
            var now = DateTime.Now;
            var waylistImport = new List<WaylistImport>();
            var waylistRequiredImport = new List<WaylistRequiredImport>();
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
                // машину определяем по первым трем цифрам
                string carNum = GetCarNum(carPart);
                bool letBool = true;
                bool digbool = true;

              
               

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

                var wayListName = GetWaylistName(carNum,date);
                var shWaylist = TaskParameters.Context.ShWaylists.FirstOrDefault(w=>w.Waylist==wayListName);
                if(shWaylist==null)
                {
                    var import = new WaylistImport();
                    
                    import.Car = shCar.CarId;
                    import.Name = wayListName;
                    import.Date = now;
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
                    
                if(shWaylist.Required=="Yes")
                {
                    var import = new WaylistRequiredImport();
                    import.Name = wayListName;
                    waylistRequiredImport.Add(import);
                    continue;
                }

                
                

                if (mailHandled)
                {
                    redemption.MoveToSuccess(mail.ConversationId);
                    // ответить
                    redemption.SendMail(
                        new AutoMail
                        {
                            Email = DistributionConstants.EgorovEmail,
                            Body = $"Файлы по '{shCar.CarId}' за '{date.ToString("MM.yyyy")}' успешно прогружены.",
                            Subject = "Re. Путевые"


                        }

                        );

                }

            }
            if (waylistImport.Count > 0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(waylistImport) });
            if (waylistRequiredImport.Count > 0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(waylistRequiredImport) });

            return true;
        }

        class WaylistImport
        {
            public string Name { get; set; }
            public string Car { get; set; }
            public DateTime? Date { get; set; }

        }

        class WaylistRequiredImport
        {
            public string Name { get; set; }
            public string Required { get; set; }

        }

        public static string GetWaylistName(string carNum,DateTime date)
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
