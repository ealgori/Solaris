using MailProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Limits
{
    public class LimitsAutoImportHandler : ATaskHandler
    {
        public LimitsAutoImportHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        public override bool Handle()
        {
            var models = new List<LimitImport>();
            RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
            var mails = processor.GetMails(new List<string> { "#Limits#" }).OrderByDescending(m => m.Date).ToList();
            foreach (var mail in mails)
            {
                var attachments = mail.Attachments.Where(a => Path.GetExtension(a.FilePath).ToLower() == ".xlsx");
                foreach (var attach in attachments)
                {
                   // var filePath = @"C:\Temp\Logs\31.05.2016\lim.xlsx";
                    var rows = EpplusInteract.EpplusSimpleUniReport.ReadFile(attach.FilePath, "Итого", 3);
                    // бежим по первой строчке, пока есть значен
                    var header = rows[0];
                    var props = typeof(EpplusInteract.SimpleUniReportRow).GetProperties();
                    var limitRows = new List<LimitRow>();
                    foreach (var prop in props)
                    {
                        var value = CommonFunctions.StaticHelpers.GetValueExt(prop, header);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var row = new LimitRow();
                            row.Limit = value;
                            row.Column = prop.Name;
                            limitRows.Add(row);
                        }

                    }

                    var executed = rows[1];
                    var limitVal = rows[2];
                    foreach (var row in limitRows)
                    {

                        string val = null;
                        if (CommonFunctions.StaticHelpers.GetPropStringyfyValue(executed, row.Column, out val))
                        {
                            decimal dec;
                            if (decimal.TryParse(val, out dec))
                            {
                                row.Executed = dec;

                            }
                        }
                        if (CommonFunctions.StaticHelpers.GetPropStringyfyValue(limitVal, row.Column, out val))
                        {
                            decimal dec;
                            if (decimal.TryParse(val, out dec))
                            {
                                row.LimitVal = dec;

                            }
                        }
                        var lim = TaskParameters.Context.ShLimits.FirstOrDefault(l => l.Alias == row.Limit);
                        if (lim != null)
                        {
                            models.Add(new LimitImport { LimitName = lim.LimitCode, Limit = row.LimitVal, Executed = row.Executed });
                        }


                    }









                }
                if (models.Count() > 0)
                {
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(models) });
                }

                var emailParam = new EmailParams(new List<string>() { mail.Email }, "LimitImport");
                emailParam.AllowWithoutAttachments = true;
                emailParam.HtmlBody = "Приветствую. Файл прогружен";
                TaskParameters.EmailHandlerParams.EmailParams.Add(emailParam);
                processor.MoveToSuccess(mail.ConversationId);

            }
            return true;

        }


        public class LimitRow
        {
            public string Limit { get; set; }
            public decimal? Executed { get; set; }
            public decimal? LimitVal { get; set; }

            public string Column { get; set; }


        }

        class LimitImport
        {
            public string LimitName { get; set; }
            public decimal? Executed { get; set; }
            public decimal? Limit { get; set; }
        }
    }
}
