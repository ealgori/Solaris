using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle.HandleResult;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO
{
    public class FirstPart
    {
        /// <summary>
        ///  если вернул позиции, но не саксесс, то это позици надо пометить как мануал GR, если на них вообще отсутствует GR
        /// </summary>
        /// <param name="toItems"> Позиции от одного ТО уже отфильтрованные по материал коду</param>
        /// <param name="sapItems"> Сап позиции с нужным номером ПО и материал кодом и отсутствием удаленными позициями</param>
        public FirstHandleResult Handle(List<ShItemModel> toItems, List<SAPItemModel> sapItems, LogManager logManager)
        {

            //проверка на присутствие количеств
            if(toItems.Any(i=>!i.Qty.HasValue)||toItems.Any(i=>!i.Price.HasValue))
            {
                logManager.Add(toItems.Select(s => s).ToList(), sapItems, $"Не указано количество либо цена", LogStatus.Error);
                return null;
            }

            if(sapItems==null||sapItems.Count==0)
            {
                logManager.Add(toItems.Select(s => s).ToList(), sapItems, $"В сапе отсутсвтует информация по позициям сх", LogStatus.Error);
                return null;
            }


            // проверка на то, что в сх заказано ровно столько же позиций, сколько и в САП
            var shQty = toItems.Sum(i => i.Qty);
            var sapQty = sapItems.Sum(i => i.QtyOrdered);
            if (shQty != sapQty)
            {
                logManager.Add(toItems.Select(s => s).ToList(), sapItems, $"Количество заказанных в сх позиций и заказанных в сап не совпало sh:{shQty} ; sap {sapQty}", LogStatus.Error);
                return null;
            }



            var shApprItems = toItems.Where(i => i.TOFactDate.HasValue).ToList();
            var shApprQty = shApprItems.Sum(s => s.Qty);
            var sapGRQty = sapItems.Sum(s => s.GRQty);

            // в сх принятых должно быть больше или равно, чем в сап GR
            if (sapGRQty > shApprQty)
            {
                logManager.Add(shApprItems, sapItems, $"В сапе GR больше, чем принято позиций в СХ sh:{shApprQty} ; sap {sapGRQty}", LogStatus.Error);
                return null;
            }


            var hr = new FirstHandleResult();
            hr.ShModels = shApprItems;
            hr.SAPRows = sapItems;

            var fmCount = shApprQty - sapGRQty;

            if (fmCount == 0) //for made
            {
                logManager.Add(shApprItems, sapItems, $"Свежепринятые позиции отсутствуют. Идем на след итер. sh:{shApprQty} ; sap:{sapGRQty}", LogStatus.Debug);

            }
            else
            {
                hr.Succeed = true;
            }



            return hr;

        }
    }
}
