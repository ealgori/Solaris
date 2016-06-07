
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

            var toTypeBezPodtv = "Регулярный без подтверждения выполнения работ";

            var hr = new SecondHandlerResult();
            var tmrItems = toItems.Where(i =>
                // i.TOFactDate.Max(i.TOPlanDate).TwoMonthRange(date) // 02.06.2016 решили отменить
                //&&
                i.WorkConfirmedByEricsson
                &&!i.ExcludeFromTO
                &&
                (
                    (i.ObichniyReqularniyTO != toTypeBezPodtv
                    && i.ShAct != null
                    && i.ShAct.ActApprovedDate.HasValue)
                    ||
                    (i.ObichniyReqularniyTO == toTypeBezPodtv) // для регулятрных без подтверждения работ нет необходимост вообще обращать внимания на акт.
                )

            ).ToList();
            if(tmrItems.Count==0)
            {
                logManager.Add(toItems, sapItems, $"Нет позиций с подтвержденными работами или принятыми актами", LogStatus.Debug);
                return null; 
            }
            if (tmrItems.Count != toItems.Count)
            {
                logManager.Add(toItems, sapItems, $"После фильтра по дате осталось :{tmrItems.Count} из {toItems.Count}", LogStatus.Debug);
            }

            var grCount = sapItems.Sum(r => r.GRQty);
            var tmrCount = tmrItems.Sum(i => i.Qty);


            // общее количество GR в Сап должно быть меньше.
            // если оно больше, значит надо в сх проставить ручной gr (отказался пока от этой идеи)

            if (grCount >= tmrCount)
            {
                logManager.Add(tmrItems, sapItems, $"В сапе принято больше либо равно , чем в сх осталось после фильтра по дате : sapGr:{grCount} из  shApprF:{tmrCount}", LogStatus.Error);
                hr.ManGRItems = tmrItems.Where(i => string.IsNullOrEmpty(i.GR)).ToList();
                return hr;
            }
            //if (grCount == tmrCount)
            //{
            //    logManager.Add(tmrItems, sapItems, $"Количество GR в СХ и сап совпало. пометим все позиции как ручные, если они не помечены  ({tmrCount})", LogStatus.Debug);
            //    hr.ShModels = 

            //    return hr;
            //}


            // набираем позиции из сх, для которых будем делать GR, чтобы потом проставить им отметки
            var shSapDif = tmrCount - grCount;
            IShItemSelect shItemSelector = new BaseShItemSelect();
            List<ShItemModel> itemsForGr;
            if (!shItemSelector.Select(tmrItems, shSapDif, out itemsForGr))
            {
                logManager.Add(tmrItems, sapItems, $"Среди позиций сх не удалось выделить {shSapDif} позиций для GR", LogStatus.Error);
                return null;
            }
            else
            {
                // помечаем без GR как ручные
                hr.ManGRItems = hr.ManGRItems = tmrItems.Except(itemsForGr).Where(i => string.IsNullOrEmpty(i.GR)).ToList();
            }

            // набираем позиции из сапа, в которые будет допринимать количество позиций

            ISapItemSelect sapItemsSelect = new BaseSapItemsSelect();
            List<GRItemModel> grItemModels = null;
            if (!sapItemsSelect.Select(sapItems, shSapDif, out grItemModels))
            {
                logManager.Add(tmrItems, sapItems, $"Среди позиций САП не удалось выделить {shSapDif} позиций для GR", LogStatus.Error);
                return null;
            }
            /// допишем, с каких позиций ТО этот сап заполнялся
            foreach (var item in grItemModels)
            {
                item.TOItem = string.Join(",", itemsForGr.Select(i => i.Id));
                item.Vendor = itemsForGr.FirstOrDefault().Vendor;
            }

           
            hr.GRModels = grItemModels;
            hr.ShModels = itemsForGr;
            hr.Succeed = true;
            return hr;







        }
    }
}



