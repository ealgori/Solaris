
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using CommonFunctions.Extentions;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapItemSelect;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle.HandleResult;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle
{

    public class SecondPart
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toItems">утвержденные позиции, с одним сап кодом</param>
        /// <param name="sapItems"> позиции из сап, отфильтр по по и сапкоду</param>
        /// <param name="logManager"></param>
        public SecondHandlerResult Handle(List<ShItemModel> toItems, List<SAPItemModel> sapItems, DateTime date, LogManager logManager)
        {

            var hr = new SecondHandlerResult();
            var tmrItems = toItems.Where(i => i.TOFactDate.TwoMonthRange(date)).ToList();
            if (tmrItems.Count != toItems.Count)
            {
                logManager.Add(toItems, null, $"После фильтра по дате осталось :{tmrItems.Count} из {toItems.Count}", LogStatus.Debug);
            }

            var grCount = sapItems.Sum(r => r.GRQty);
            var tmrCount = tmrItems.Sum(i => i.Qty);


            // общее количество GR в Сап должно быть меньше.
            // если оно больше, значит надо в сх проставить ручной gr

            if (grCount > tmrCount)
            {
                logManager.Add(tmrItems, sapItems, $"В сапе принято больше , чем в сх осталось после фильтра по дате : sapGr:{grCount} из  shApprF:{tmrCount}", LogStatus.Error);
                return null ;
            }
            if (grCount == tmrCount)
            {
                logManager.Add(tmrItems, sapItems, $"Количество GR в СХ и сап совпало. пометим все позиции как ручные, если они не помечены  ({tmrCount})", LogStatus.Debug);
                hr.ShModels = tmrItems;

                return hr;
            }


            // набираем позиции из сх, для которых будем делать GR, чтобы потом проставить им отметки
            var shSapDif = tmrCount - grCount;
            IShItemSelect shItemSelector = new BaseShItemSelect();
            List<ShItemModel> itemsForGr;
            if (!shItemSelector.Select(tmrItems, shSapDif, out itemsForGr))
            {
                logManager.Add(tmrItems, sapItems, $"Среди позиций сх не удалось выделить {shSapDif} позиций для GR", LogStatus.Error);
                return null;
            }

            // набираем позиции из сапа, в которые будет допринимать количество позиций

            ISapItemSelect sapItemsSelect = new BaseSapItemsSelect();
            List<GRItemModel> grItemModels = null;
            if (!sapItemsSelect.Select(sapItems, shSapDif, out grItemModels))
            {
                logManager.Add(tmrItems, sapItems, $"Среди позиций САП не удалось выделить {shSapDif} позиций для GR", LogStatus.Error);
                return null;
            }

           
            hr.GRModels = grItemModels;
            hr.ShModels = itemsForGr;
            hr.Succeed = true;
            return hr;







        }
    }
}



