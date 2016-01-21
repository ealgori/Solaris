using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using MailProcessing;
using System.IO;
using EpplusInteract;
using CommonFunctions.Extentions;
using System.Collections;
using DbModels.DomainModels.ShClone;
using Models;


namespace TaskManager.Handlers.TaskHandlers.Models.TOH
{
    public class TOImport : ATaskHandler
    {
        public TOImport(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            RedemptionMailProcessor interact = new RedemptionMailProcessor("SOLARIS");
            var mails = interact.GetMails(new List<string>() { "TO_Import" });

            foreach (var mail in mails)
            {

            StringBuilder logs = new StringBuilder();
                var emailParam = new EmailParams(new List<string>() { mail.Email }, "TOImport");
                foreach (var attach in mail.Attachments)
                {
           // var attach = new Attachment() { FilePath = @"C:\TEMP\TO_2014_list_1424848839453.xlsm", File = "TO_2014_list_1424848839453.xlsm" };
            List<TOAttachModel> model = new List<TOAttachModel>();
            List<TOEquipmentModel> eqModel = new List<TOEquipmentModel>();
            var itemsToDelete = new List<ShTOItem>();
            try
            {
                if (Path.GetExtension(attach.File).ToUpper() == ".XLSM")
                {
                    var objs = EpplusSimpleUniReport.ReadFile(attach.FilePath, "Template", 2);
                    // if (objs == null)
                    // continue;
                    int eqName = 5;
                    int eqQuant = 6;
                    var objgroups = objs.Where(o => !string.IsNullOrEmpty(o.Column1)).GroupBy(g => g.Column1).ToList();
                    foreach (var obj in objgroups)
                    {
                        var shTO = TaskParameters.Context.ShTOes.Find(obj.Key);
                        if (shTO != null)
                        {
                            if (!string.IsNullOrEmpty(shTO.TOapproved))
                            {
                                LogMessage(string.Format("ТО '{0}' заморожено. Айтемы не могут быть добавлены.", obj.Key), logs);
                            }
                            else
                            {
                                var items = TaskParameters.Context.ShTOes.Where(t => t.TO == obj.Key).Join(TaskParameters.Context.ShTOItems, to => to.TO, it => it.TOId, (to, it) => it)
                                    //.Where(it => it.Site == obj.Column3)
                                    .ToList();
                                if (items.Count() != 0)
                                {
                                    itemsToDelete.AddRange(items);

                                    //   LogWarn(string.Format("Сайт {0} присутствуюет в списке для ТО {1}", obj.Column3, obj.Column1), logs);
                                }


                                foreach (var _obj in obj)
                                {
                                    DateTime plannedDate;
                                    var tom = new TOAttachModel();
                                    tom.ItemId = "test";
                                    tom.TOId = _obj.Column1;
                                    tom.SiteId = _obj.Column3;
                                    if (DateTime.TryParse(_obj.Column2, out plannedDate))
                                    {
                                        tom.TOPlannedDate = plannedDate.ToString("dd-MM-yyyy");
                                    }
                                    model.Add(tom);
                                    var matItems = TaskParameters.Context.ShMatTOItems.Where(i => i.TOId == tom.TOId);
                                    if(matItems.Count()>0)
                                        eqModel.Add(new TOEquipmentModel() { TOId = tom.TOId, Equipment = true });
                                    LogMessage(string.Format("Сайт '{0}' отправлен на добавление в спискок для ТО '{1}'", _obj.Column3, _obj.Column1), logs);


                                }



                            }

                        }
                        else
                        {
                            LogMessage(string.Format("ТО '{0}' не найден в СХ", obj.Key), logs);
                        }

                    }


                }

            }
            catch (System.Exception ex)
            {
                LogError(string.Format("Ошибка при работе с файлом:{0}", ex.Message), logs);
            }
            emailParam.FilePaths.Add(attach.FilePath);
            emailParam.HtmlBody = logs.ToString();
            if (itemsToDelete.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(itemsToDelete) });
            }
            if (model.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(model) });


            }
            if (eqModel.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(eqModel) });


            }
                }

                TaskParameters.EmailHandlerParams.EmailParams.Add(emailParam);
                interact.MoveToSuccess(mail.ConversationId);
            }
            return true;

        }

        public void LogError(string message, StringBuilder builder)
        {
            builder.AppendLine(string.Format("<li>{0}{1}</li>", message, System.Environment.NewLine));
        }

        public void LogMessage(string message, StringBuilder builder)
        {
            builder.AppendLine(string.Format("<li>{0}{1}</li>", message, System.Environment.NewLine));
        }

        public void LogWarn(string message, StringBuilder builder)
        {
            builder.AppendLine(string.Format("<li>{0}{1}</li>", message, System.Environment.NewLine));
        }

        public class TOAttachModel
        {
            public string ItemId { get; set; }
            public string TOId { get; set; }
            public string SiteId { get; set; }
            public string TOPlannedDate { get; set; }
            public string EquipmentName { get; set; }
            public string EquipmentQuantity { get; set; }


        }

        public class TOEquipmentModel
        {
            public string TOId { get; set; }
            public bool Equipment { get; set; }
        }
    }


}
