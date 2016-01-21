using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.ImportHandlers.Abstract;
using DbModels.DataContext;
using System.IO;
using AutoImport.Rev3.DomainModels;
using EpplusInteract;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    public class TOImportHandler : IAutoImportHandler
    {
        public HandlerResult Handle(Attachment attachment)
        {
            HandlerResult hr = new HandlerResult();
            using (Context context = new Context())
            {
                // string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.FFFFFFF");
                HandlerResult result = new HandlerResult();
                string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));

                List<TOImportModel> importModels = new List<TOImportModel>();
                var _objs = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 2);
                // 11 = SubcPlanDate
                // 12  = subcFactDate
                // 13 = Approved
                // 7 =  ItemId 
                if (_objs.Count == 0)
                {
                    hr.ErrorsList.Add(string.Format("Не удалось считать ни одной записи их файла "));
                    return result;
                }
                // plandate
                var objs = _objs.Select(s => new TOImportViewModel()
                {
                    Approved = s.Column13,
                    SubcFactDate = s.Column12,
                    ItemId = s.Column7,
                    SubcPlanDate = s.Column11
                });
                var planObjs = objs.Where(o => (!string.IsNullOrEmpty(o.SubcPlanDate)) && (!string.IsNullOrEmpty(o.ItemId))).ToList();

                foreach (var obj in planObjs)
                {
                    DateTime plannedDate;
                    if (!DateTime.TryParse(obj.SubcPlanDate, out plannedDate))
                    {
                        hr.ErrorsList.Add(string.Format("Не распознана дата:{0}",obj.SubcPlanDate));
                        continue;
                    }
                    var shItem = context.ShTOItems.Find(obj.ItemId);
                    if (shItem == null)
                    {
                        hr.ErrorsList.Add(string.Format("Не найден айтем по ИД:{0}", obj.ItemId));
                        continue;
                    }
                    else
                    {
                        var subcs = context.ShContacts.Where(c => c.EMailAddress.Contains(attachment.Mail.Sender)).ToList();
                        var owners = subcs.Join(context.ShTOes.Where(t => t.TO == shItem.TOId), s => s.Contact, t => t.Subcontractor, (t, s) => new { t, s });
                        if (owners.Count() == 0 )
                        {
                            if (attachment.Mail.Sender != "aleksey.chekalin@ericsson.com"
                             && attachment.Mail.Sender != "aleksey.borshchev@ericsson.com")
                            {
                                hr.ErrorsList.Add(string.Format("Адресат не является владельцем айтема :{0}", obj.ItemId));
                                continue;
                            }
                        }

                    }
                    if (!shItem.TOPlanDateSubcontractor.HasValue)
                    {
                        hr.InfoList.Add(string.Format("Import: ItemId:{0}, SubcPlanDate:{1};",obj.ItemId, obj.SubcPlanDate));
                        importModels.Add(new TOImportModel()
                        {
                            ItemId = obj.ItemId,
                            SubcPlanDate = plannedDate
                        });
                    }
                    else
                    {
                        if (!shItem.TOFactDate.HasValue && (!shItem.WorkConfirmedByEricsson))
                        {

                            if (shItem.TOPlanDateSubcontractor != plannedDate)
                            {
                                hr.InfoList.Add(string.Format("Import: ItemId:{0}, SubcPlanDate:{1};", obj.ItemId, obj.SubcPlanDate));
                                importModels.Add(new TOImportModel()
                                {
                                    ItemId = obj.ItemId,
                                    SubcPlanDate = plannedDate
                                });

                            }
                            else
                            {
                                hr.InfoList.Add(string.Format("AlreadyInSH: ItemId:{0}, SubcPlanDate:{1};", obj.ItemId, obj.SubcPlanDate));
                            }
                        }
                        else
                        {
                            hr.ErrorsList.Add(string.Format("Непустая фактДата, либо подтвержден факт выполения работ ItemId:{0}", obj.ItemId));
                        }

                    }

                }


                var factObjs = objs.Where(o => !string.IsNullOrEmpty(o.SubcFactDate) && (!string.IsNullOrEmpty(o.ItemId))).ToList();
                foreach (var obj in factObjs)
                {
                    DateTime factDate;
                    if (!DateTime.TryParse(obj.SubcFactDate, out factDate))
                    {
                        hr.ErrorsList.Add(string.Format("Не распознана дата:{0}", obj.SubcFactDate));
                        continue;
                    }
                    var shItem = context.ShTOItems.Find(obj.ItemId);
                    if (shItem == null)
                    {
                        hr.ErrorsList.Add(string.Format("Не найден айтем по ИД:{0}", obj.ItemId));
                        continue;
                    }
                    else
                    {
                        var subcs = context.ShContacts.Where(c => c.EMailAddress.Contains(attachment.Mail.Sender)).ToList();
                        var owners = subcs.Join(context.ShTOes.Where(t => t.TO == shItem.TOId), s => s.Contact, t => t.Subcontractor, (t, s) => new { t, s });
                        if (owners.Count() == 0)
                        {
                            hr.ErrorsList.Add(string.Format("Адресат не является владельцем айтема :{0}", obj.ItemId));
                            continue;
                        }
                    }
                    if (!shItem.TOFactDate.HasValue)
                    {
                        hr.InfoList.Add(string.Format("Import: ItemId:{0}, SubcFactDate:{1};", obj.ItemId, obj.SubcFactDate));
                        importModels.Add(new TOImportModel()
                        {
                            ItemId = obj.ItemId,
                            SubcFactDate = factDate
                        });

                    }
                    else
                    {
                        if (!shItem.WorkConfirmedByEricsson)
                        {
                            if (shItem.TOFactDate != factDate)
                            {
                                importModels.Add(new TOImportModel()
                                {
                                    ItemId = obj.ItemId,
                                    SubcFactDate = factDate
                                });
                            }
                            else
                            {
                                hr.InfoList.Add(string.Format("AlreadyInSH: ItemId:{0}, SubcFactDate:{1};", obj.ItemId, obj.SubcFactDate));
                            }
                        }
                        else
                        {
                            hr.ErrorsList.Add(string.Format("Выполнение работ подтверждено ItemId:{0}", obj.ItemId));
                        }
                    }

                }
                // конвертируем их в дататэйбл, чтобы воспользоваться существующим функционалом
                var dataTable = importModels.ToDataTable();
                var WB = NpoiInteract.GetNewWorkBook();
                // встваляем в нее данные и создаем в ней новый шит
                NpoiInteract.FillReportData(dataTable, "sheet1", WB);
                // сохраняем это все по пути назначения
                string path = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("TOSubcImport", attachment.Id, ".xls"));
                NpoiInteract.SaveReport(path, WB);
                result.FilesPaths.Add(path);
                result.Success = true;
                return result;
                //var objs = EpplusSimpleUniReport.ReadFile(attachment.FilePath, Constants.DefaultSheetName, 2);
            }

         
        }


    }

    public class TOImportModel
    {
        public string ItemId { get; set; }
        public DateTime? SubcPlanDate { get; set; }
        public DateTime? SubcFactDate { get; set; }
    }

    public class TOImportViewModel
    {
        public string ItemId { get; set; }
        public string SubcPlanDate { get; set; }
        public string SubcFactDate { get; set; }
        public string Approved { get; set; }
    }
}
