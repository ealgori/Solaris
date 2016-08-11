using EpplusInteract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class NewAVRDistrHandler : ATaskHandler
    {
        public NewAVRDistrHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

        /// <summary>
        /// свежесозданные заявки отправляются на адрес контактных лиц из сх контактс
        /// </summary>
        /// <returns></returns>
        public override bool Handle()
        {
            

            bool test = true;
            var testRecipints = DistributionConstants.EalgoriEmail+";"+DistributionConstants.EgorovEmail;

            var startDate = new DateTime(2016, 6, 23);
            var now = DateTime.Now;
            var importModels = new List<ImportModel>();
            var avrs = TaskParameters.Context.ShAVRs
                .Where(a => a.ObjectCreateDate > startDate)
                .Where(a => !a.SendToSubc.HasValue)
                .Where(a => a.Subregion == "VC MS Siberia Novosibirsk") //тест
                .ToList();
            foreach (var avr in avrs)
            {
                var recipients = TaskParameters.Context.ShContacts.FirstOrDefault(c => c.Contact == avr.Subcontractor);
                if (recipients != null)
                    using (var service = new EpplusService(TaskParameters.DbTask.TemplatePath))
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("AVR", avr.AVRId);
                        dict.Add("Subregion", avr.Subregion);
                        dict.Add("Subcontractor", avr.Subcontractor);
                        dict.Add("TaskSubcontractorNumber", avr.TaskSubcontractorNumber);
                        dict.Add("AddingsInfo", avr.AddingsInfo);
                        dict.Add("Source", avr.Source);
                        dict.Add("SourceNo", avr.SourceNo);
                        dict.Add("CreatedBy", avr.CreatedBy);
                        dict.Add("Total", avr.TotalAmount.Value.ToString("C"));
                        dict.Add("StartDate", avr.WorkStart.Value.ToShortDateString());
                        dict.Add("EndDate", avr.WorkEnd.Value.ToShortDateString());
                        service.ReplaceDataInBook(dict);
                        var path = Path.Combine(TaskParameters.DbTask.EmailSendFolder, $"{avr.AVRId}.xlsx");
                        service.CreateFolderAndSaveBook(path);
                        var recip = recipients.EMailAddress.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        TaskParameters.EmailHandlerParams.Add(recip, null, $"New AVR {avr.AVRId}", false, "Hi", new List<string> { path }, test ? testRecipints : null);
                        importModels.Add(new ImportModel { AVR = avr.AVRId, SendToSbcr = now });



                    }
            }
           // if (!test)
                if (importModels.Count > 0)
                {
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });
                }
            return true;

        }

        class ImportModel
        {
            public string AVR { get; set; }
            public DateTime? SendToSbcr { get; set; }
        }
    }
}
