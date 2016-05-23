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

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class SendWIHGRTORequestsHandler:ATaskHandler
    {
        public SendWIHGRTORequestsHandler(TaskParameters taskParams):base(taskParams)
        {

        }

        public override bool Handle()
        {

            string manualGR = "Manual GR";
            var itemGRNameModels = new List<TOItemGRNameImport>();

            var toItems = TaskParameters.Context.ShTOes.Where(t => t.Year == "2016" && !string.IsNullOrEmpty(t.PONumber)).
                Join(TaskParameters.Context.ShTOItems, t => t.TO, i => i.TOId, (t, i) => new { TO = t, Item = i }). // джойним ТО с позициями
                Join(TaskParameters.Context.PriceListRevisionItems, t => t.Item.IDItemFromPL, p => p.Id, (t, p) =>
                    new ShItemModel {
                        TO = t.TO.TO,
                        PO = t.TO.PONumber,
                        Id = t.Item.TOItem,
                        Price = t.Item.PriceFromPL,
                        Qty = t.Item.Quantity,
                        TOFactDate = t.Item.TOFactDate,
                        MaterialCode = p.SAPCode.Code,
                        GR = t.Item.GRName
                        
                    } )// джойним позиции с Прайс позициями
                .GroupBy(g=>g.PO)
                .ToList();
               
            string path = @"C:\Temp\Logs\19.05.2016\zzpomon.xlsx";
            ISapReader reader = new XlsxSapReader(path);
            reader.Read();
            if(!reader.Succeed)
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
                var sapRows = reader.Rows.Where(r => r.PO == items.Key)
                    .Select(r => new SAPItemModel
                    {
                        POItemId = r.POItem,
                        GRQty = r.GRQty,
                        MaterialCode = r.MaterialCode,
                        QtyOrdered = r.QtyOrdered,
                        Price = r.Price

                    })
                    .ToList();
                var itemsGroupBySapCode = items.GroupBy(i => i.MaterialCode);
                foreach (var itemsByCode in itemsGroupBySapCode)
                {
                    var sapRowsByCode = sapRows.Where(r => r.MaterialCode == itemsByCode.Key).ToList();



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
                            fResult.ShModels
                            .Where(m => string.IsNullOrEmpty(m.GR))
                            .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = manualGR }));

                        continue;
                    }

                    var sResult = secondPart.Handle(fResult.ShModels, fResult.SAPRows, processDate, logManager);
                    if (sResult == null)
                    {
                        continue;
                    }

                    if (!sResult.Succeed)
                    {
                        // значит все совпадает. надо пометить позиции без GR как мануал
                        itemGRNameModels.AddRange(
                            sResult.ShModels
                            .Where(m => string.IsNullOrEmpty(m.GR))
                            .Select(s => new TOItemGRNameImport { Id = s.Id, GRName = manualGR }));

                        continue;
                    }
                    var grQty = sResult.GRModels.Sum(s => s.Qty);
                    logManager.Add(null, null, $"Будет GR на {grQty} ", LogStatus.Info);

                }
            }

            var logBytes = NpoiInteract.DataTableToExcel(logManager.AsTable().ToDataTable());
            CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Temp\123\logs.xls", logBytes);





            return true;
        }



        public class TOItemGRNameImport
        {
            public string Id { get; set; }
            public string GRName { get; set; }
        }
    }
}
