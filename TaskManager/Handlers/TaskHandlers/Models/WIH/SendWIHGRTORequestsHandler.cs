using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapReader;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle.HandleResult;
using DbModels.Models.Pors;
using WIHInteract;
using System.IO;
using DbModels.DomainModels.ShClone;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class SendWIHGRTORequestsHandler : ATaskHandler
    {

        public string zzpomonPath { get; set; }
        public SendWIHGRTORequestsHandler(TaskParameters taskParams) : base(taskParams)
        {
            if(taskParams.TestTaskParams!=null)
            {
                if (!string.IsNullOrEmpty(taskParams.TestTaskParams.PathParam))
                {
                    zzpomonPath = taskParams.TestTaskParams.PathParam;
                }
                   
            }
        }

        public override bool Handle()
        {

            string manualGR = "Manual GR";
            var itemGRNameModels = new List<TOItemGRNameImport>();
            var GRModels = new List<SecondHandlerResult>();
            List<ShWIHRequest> requestList = new List<ShWIHRequest>();
            var logGrModels = new List<LogGRModel>();

            // смотрим конкретные ПО
            bool test = false;
            // ничего не отправляем
            bool jugging = false;

            var toItems = TaskParameters.Context.ShTOes.Where(t => (t.Year == "2016"||t.Year=="2017") && !string.IsNullOrEmpty(t.PONumber)).
                Join(TaskParameters.Context.SubContractors, t => t.Subcontractor, i => i.ShName, (t, v) => new { TO = t, Vendor = v }).// джойним с подрядчиками
                Join(
                    TaskParameters.Context.ShTOItems.Where(i => (!i.ExcludeWork.HasValue) || (!i.ExcludeWork.Value))
                    // 06.02.2017. почему то в выборку попадаются эти позиции. попробуем от них избавиться
                    , t => t.TO.TO, i => i.TOId, (t, i) => new { TO = t.TO, Item = i, Vendor = t.Vendor }). // джойним ТО с позициями
                Join(TaskParameters.Context.PriceListRevisionItems, t => t.Item.IDItemFromPL, p => p.Id, (t, p) =>
                    new ShItemModel
                    {
                        TO = t.TO.TO,
                        ObichniyReqularniyTO = t.TO.ObichniyRegulyarniyTO.Trim(),
                        PO = t.TO.PONumber,
                        Id = t.Item.TOItem,
                        Price = t.Item.PriceFromPL,
                        Qty = t.Item.Quantity,
                        TOFactDate = t.Item.TOFactDate,
                        TOPlanDate = t.Item.TOPlanDate,
                        MaterialCode = p.SAPCode.Code,
                        GR = t.Item.GRName,
                        Vendor = t.Vendor.SAPNumber,
                        WorkConfirmedByEricsson = t.Item.WorkConfirmedByEricsson,
                        ActId = t.Item.ActId,
                        ExcludeFromTO = t.Item.ExcludeWork.HasValue ? t.Item.ExcludeWork.Value : false



                    })// джойним позиции с Прайс позициями
                .GroupBy(g => g.PO)
                .ToList();

            if (test)
            {
                toItems = toItems.Where(t =>


               t.Key == "4513118046"
                //|| t.Key == "4512630069"
                //|| t.Key == "4512826552"
                //|| t.Key == "4512826651"
                //|| t.Key == "4512826721"
                //|| t.Key == "4512826771"
                //|| t.Key == "4512920101"
                //|| t.Key == "4512920140"
                //|| t.Key == "4512920427"
                //|| t.Key == "4512974646"
                //|| t.Key == "4512974674"
                //|| t.Key == "4512975149"






                ).ToList();
            }

            // чтение сап файла
            var reportFolder = string.IsNullOrEmpty(zzpomonPath)?TaskParameters.DbTask.EmailSendFolder:zzpomonPath;
            if (!Directory.Exists(reportFolder))
            {
                TaskParameters.TaskLogger.LogError($"Папка недоступна:{reportFolder}");
                return false;
            }
            var reportFile = Directory.GetFiles(reportFolder, "*zzpomon*.xlsx", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (!File.Exists(reportFile))
                return false;

            var reportDatedPath = CommonFunctions.StaticHelpers.GetDatedPath(reportFolder);
            ISapReader reader = new XlsxSapReader(reportFile);
            reader.Read();
            if (!reader.Succeed)
            {
                TaskParameters.TaskLogger.LogError($"Ошибка чтения файла {reportFile}");
                return false;
            }
            else
            {



            }
            // переместить папку в лог
            if (!test)
            {
                if (!Directory.Exists(reportDatedPath))
                {
                    Directory.CreateDirectory(reportDatedPath);
                }
                File.Move(reportFile, Path.Combine(reportDatedPath, Path.GetFileName(reportFile)));
            }




            // перебор позиций по номеру ПО
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var secondPart = new SecondPart();
            DateTime processDate = DateTime.Now;
            var icount = toItems.Count();
            var counter = 0;

            foreach (var items in toItems)
            {
                counter++;
                TaskParameters.TaskLogger.LogDebug($"{items.Key} - {counter}:{icount}");
                var sapRows = reader.Rows
                    .Where(r => string.IsNullOrEmpty(r.PODeletionIndicator))
                    .Where(r => r.PO == items.Key)
                    .Select(r => new SAPItemModel
                    {
                        POItemId = r.POItem,
                        GRQty = r.GRQty,
                        MaterialCode = r.MaterialCode,
                        QtyOrdered = r.QtyOrdered,
                        Price = r.Price

                    })
                    .ToList();
                var itemsGroupBySapCode = items.GroupBy(i => new
                {
                    //i.MaterialCode,
                    i.Price
                });
                foreach (var itemsByCode in itemsGroupBySapCode)    // 07.06.2016 - пробуем отказаться от группировке по материал коду
                {                                                   // 
                    var sapRowsByCode = sapRows.Where(r =>
                    //r.MaterialCode == itemsByCode.Key.MaterialCode&&
                    r.Price == itemsByCode.Key.Price

                    ).ToList();



                    logManager.Key = items.Key;
                    var fResult = firstPart.Handle(itemsByCode.ToList(), sapRowsByCode, logManager);
                    if (fResult == null)
                    {
                        continue;
                    }
                    if (!fResult.Succeed)
                    {
                        // значит все совпадает. надо пометить позиции без GR как мануал
                        itemGRNameModels.AddRange(
                            fResult.ManGRItems
                            .Where(m => string.IsNullOrEmpty(m.GR))
                            .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = manualGR }));

                        continue;
                    }

                    var sShModels = fResult.ShModels.GroupJoin(
                        TaskParameters.Context.ShActs,
                        i => i.ActId,
                        a => a.Act,
                        (x, y) => new { item = x, act = y }
                        )
                        .SelectMany(
                        x => x.act.DefaultIfEmpty(),
                        (x, y) => { x.item.ShAct = y; return x.item; }).ToList();

                    var sResult = secondPart.Handle(sShModels, fResult.SAPRows, processDate, logManager);
                    if (sResult == null)
                    {

                        continue;
                    }


                    itemGRNameModels.AddRange(
                           sResult.ManGRItems
                           .Where(m => string.IsNullOrEmpty(m.GR))
                           .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = manualGR }));

                    if (!sResult.Succeed)
                    {
                        // значит все совпадает.


                        continue;
                    }

                    sResult.PO = items.Key;



                    GRModels.Add(sResult);
                    //просто логи
                    var grQty = sResult.GRModels.Sum(s => s.Qty);
                    var grItemsText = string.Join(";", sResult.GRModels.Select(s => $"{s.POItemId} - {s.Qty}").ToList());
                    logManager.Add(sResult.ShModels, fResult.SAPRows, $@"Будет GR на {grQty}. Позиции: {grItemsText}", LogStatus.Info);
                    //



                }
            }

            // компановка для отправки

            if (GRModels.Count > 0)
            {
                GRModels.ForEach(r =>
                {
                    r.GRModels
                        .ForEach(i =>
                        {
                            i.Act = string.Join(",", r.ShModels.Select(m => m.ActId).Distinct().ToList());
                            i.TOItem = string.Join(",", r.ShModels.Select(m => m.Id));
                            i.FactDate = string.Join(",", r.ShModels.Select(m => m.TOFactDate.Value.ToString("MM")).Distinct().ToList()); /// потенцияальная возможность ошибки, если факт дэйт не заполнен вдруг
                        });


                });

                var now = DateTime.Now;
                int count = 0;
                var groupByPO = GRModels.GroupBy(g => g.PO);
                foreach (var group in groupByPO)
                {
                    count++;
                    var grItems = group
                        .SelectMany(g => g.GRModels)
                        .Select(i => new PORTOItem
                        {
                            No = int.Parse(i.POItemId),
                            Code = i.MaterialCode,
                            Cat = "Service",
                            Plant = "2349",
                            NetQty = i.Qty,
                            ItemCat = "N",
                            PRtype = "3",
                            POrg = "1439",
                            GLacc = "402601",
                            Price = i.Price,
                            Curr = "RUB",
                            Vendor = i.Vendor,
                            Act = i.Act,
                            ItemId = i.TOItem,
                            Description = i.FactDate


                        }).ToList();

                    /// компановка отправка
                    var grBytes = ExcelParser.ExcelParser.CreateTOGR.CreateGRFile(grItems, group.Key, TaskParameters.DbTask.TemplatePath);
                    var grFileName = $"GR_{group.Key}_{DateTime.Now.ToString("yyMMddHHmmssfff")}{(jugging ? "-N" : "")}.xlsx";
                    var archive = Path.Combine(TaskParameters.DbTask.ArchiveFolder, now.ToString(@"yyyy\\MM\\dd"));
                    if (!Directory.Exists(archive))
                    {
                        try
                        {
                            Directory.CreateDirectory(archive);
                        }
                        catch (Exception exc)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка создания папки  '{0}'; {1}", archive, exc.Message));
                            continue;
                        }
                    }
                    var filePath = Path.Combine(archive, grFileName);
                    if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, grBytes))
                    {
                        TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath));
                        continue;
                    }

                    string internalMailType = WIHInteract.Constants.InternalMailTypeTOGR;
                    var mailInf = MailInfoFactory.GetGRInfo(internalMailType, filePath);

                    string result = "jugging";
                    if (!jugging)
                    {
                        result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS");
                    }
                    if (string.IsNullOrEmpty(result) || (string.IsNullOrWhiteSpace(result)))
                    {
                        TaskParameters.TaskLogger.LogError(string.Format("Функция отправки письма не вернула ConversationIndex "));
                    }
                    else
                    {
                        // еще все позиции из гр пометить и добавить на импорт
                        var shItems = group
                           .SelectMany(g => g.ShModels)
                           .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = grFileName })
                           .ToList();
                        itemGRNameModels.AddRange(shItems);

                        // еще в лог для Егорова

                        logGrModels.AddRange(
                            grItems.Select(i => new LogGRModel
                            {
                                PO = group.Key,
                                Act = i.Act,
                                Code = i.Code,
                                Description = i.Description,
                                ItemId = i.ItemId,
                                GRQty = i.NetQty,
                                POItem = i.No.ToString(),
                                Price = i.Price,
                                Vendor = i.Vendor
                            })
                            );




                        string toName = group.SelectMany(g => g.ShModels).FirstOrDefault().TO;
                        requestList.Add(new ShWIHRequest() { TOid = toName, WIHrequests = grFileName, RequestSentToODdate = now, Type = WIHInteract.Constants.InternalMailTypeTOGR });
                    }
                }
            }

            if (!jugging)
            {
                if (itemGRNameModels.Count > 0)
                {
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(itemGRNameModels) });
                }
                if (requestList.Count > 0)
                {
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(requestList) });
                }
            }


            try
            {
                var logBytes = NpoiInteract.DataTableToExcel(logManager.AsTable().ToDataTable());
                CommonFunctions.StaticHelpers.ByteArrayToFile(Path.Combine(
                    reportDatedPath
                    , $"{DateTime.Now.ToString("yyyyMMddHHmmss")}logs.xls"), logBytes);
            }
            catch (Exception ex)
            {

                TaskParameters.TaskLogger.LogError($"Ошибка сохранения файла лога по пути {reportDatedPath}");
            }
            if (logGrModels.Count > 0)
            {
                var emailParams = new EmailParams(new List<string> {
                "dmitriy.b.egorov@ericsson.com",
                "aleksey.gorin@ericsson.com" }
                , jugging?"GR TO to sent":"SentTOGR");
                emailParams.DataTables.Add("Log.xls", logGrModels.ToDataTable());
                emailParams.HtmlBody += TaskParameters.DbTask.ArchiveFolder;
                TaskParameters.EmailHandlerParams.EmailParams.Add(emailParams);
            }



            // импорт




            return true;
        }



        public class TOItemGRNameImport
        {
            public string Id { get; set; }
            public string GRName { get; set; }
        }


        public class LogGRModel
        {

            public string PO { get; set; }
            public string POItem { get; set; }
            public string Code { get; set; }

            public decimal GRQty { get; set; }

            public decimal Price { get; set; }

            public string Vendor { get; set; }

            public string Description { get; set; }

            public string ItemId { get; set; }

            public string Act { get; set; }
        }
    }
}
