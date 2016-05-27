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

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class SendWIHGRTORequestsHandler : ATaskHandler
    {
        public SendWIHGRTORequestsHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        public override bool Handle()
        {

            string manualGR = "Manual GR";
            var itemGRNameModels = new List<TOItemGRNameImport>();
            var GRModels = new List<SecondHandlerResult>();
            bool test = false;

            var toItems = TaskParameters.Context.ShTOes.Where(t => t.Year == "2016" && !string.IsNullOrEmpty(t.PONumber)).
                Join(TaskParameters.Context.SubContractors, t => t.Subcontractor, i => i.ShName, (t, v) => new { TO = t, Vendor = v }).// джойним с подрядчиками
                Join(TaskParameters.Context.ShTOItems, t => t.TO.TO, i => i.TOId, (t, i) => new { TO = t.TO, Item = i, Vendor = t.Vendor }). // джойним ТО с позициями
                Join(TaskParameters.Context.PriceListRevisionItems, t => t.Item.IDItemFromPL, p => p.Id, (t, p) =>
                    new ShItemModel
                    {
                        TO = t.TO.TO,
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
                        ActId = t.Item.ActId



                    })// джойним позиции с Прайс позициями
                .GroupBy(g => g.PO)
                .ToList();

            if (test)
            {
                toItems = toItems.Where(t =>
                //t.Key == "4512498621"
                //||t.Key == "4512678010"
                //||t.Key == "4512718056"
                //||t.Key == "4512718449"
                //||t.Key == "4512718661"
                t.Key == "4512489038"


                ).ToList();
            }

            string path = @"C:\Temp\Logs\19.05.2016\zzpomon.xlsx";
            ISapReader reader = new XlsxSapReader(path);
            reader.Read();
            if (!reader.Succeed)
            {
                TaskParameters.TaskLogger.LogError($"Ошибка чтения файла {path}");
                return false;
            }

            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var secondPart = new SecondPart();
            DateTime processDate = DateTime.Now;
            foreach (var items in toItems)
            {
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
                var itemsGroupBySapCode = items.GroupBy(i => new { i.MaterialCode, i.Price });
                foreach (var itemsByCode in itemsGroupBySapCode)
                {
                    var sapRowsByCode = sapRows.Where(r =>
                    r.MaterialCode == itemsByCode.Key.MaterialCode
                    && r.Price == itemsByCode.Key.Price

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
                        // значит все совпадает. надо пометить позиции без GR как мануал
                       

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


           // if (!test)
            {
                

                if (GRModels.Count > 0)
                {
                    GRModels.ForEach(r =>
                    {
                        r.GRModels
                            .ForEach(i => {
                                i.Act = string.Join(",", r.ShModels.Select(m => m.ActId));
                                i.TOItem = string.Join(",", r.ShModels.Select(m => m.Id));
                                i.FactDate = string.Join(",", r.ShModels.Select(m => m.TOFactDate.Value.ToString("dd.MM.yyyy"))); /// потенцияальная возможность ошибки, если факт дэйт не заполнен вдруг
                            });


                    });

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


                        var bytes = ExcelParser.ExcelParser.CreateTOGR.CreateGRFile(grItems, group.Key, TaskParameters.DbTask.TemplatePath);
                        var GRName = $"GR_{group.Key}_{DateTime.Now.ToString("yyMMddHHmmssfff")}.xlsx";

                        CommonFunctions.StaticHelpers.ByteArrayToFile($@"C:\Temp\123\{GRName}", bytes);

                        // отправка
                        // если успешно, добавление на импорт позиций
                        //itemGRNameModels.AddRange(
                        // group.SelectMany(g=>g.ShModels)
                        // .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = "GRName" }));


                        // добавление на импорт запросов




                    }

                    // группировка по по

                }
                if (itemGRNameModels.Count > 0)
                {
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(itemGRNameModels) });
                }
            }
            var logBytes = NpoiInteract.DataTableToExcel(logManager.AsTable().ToDataTable());
            CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Temp\123\logs.xls", logBytes);


            // импорт




            return true;
        }



        public class TOItemGRNameImport
        {
            public string Id { get; set; }
            public string GRName { get; set; }
        }
    }
}
