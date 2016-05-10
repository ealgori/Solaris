using DbModels.DataContext;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO
{
    public static class Matching
    {

        /// <summary>
        /// позиции уже отфильтрованы по нетворку
        /// </summary>
        /// <param name="sapRows"></param>
        /// <param name="toItems"></param>
        //public void Match(List<SAPRow> sapRows, List<ShTOItem> toItems, Context context)
        //{
        //    var toItemsGroupByCode = toItems.GroupBy(g => g.IDItemFromPL);

        //    foreach (var codeGroup in toItemsGroupByCode)
        //    {
        //        var priceListItem = context.PriceListRevisionItems.FirstOrDefault(i => i.Id == codeGroup.Key);
        //        if(priceListItem!=null)
        //        {
        //            var sapCode = priceListItem.SAPCode.Code;
        //            var sapRowsByCode = sapRows.Where(r => r.SAPCode == sapCode).ToList();
        //            if(sapRowsByCode.Count()>0)
        //            {
        //                throw new Exception($"На одном PO две позиции с одинаковыми сап кодами. Это ненормально. Ожидалось, что позиции должны были быть сгруппированы по SAP коду при отправке пора");
        //            }
        //            if(sapRowsByCode.Count()==0)
        //            {
        //                throw new Exception($"В сапе не нашлось позиций с кодом {sapCode}");
        //            }
        //            var sapRowByCode = sapRowsByCode.FirstOrDefault();

        //            var acceptedItems = codeGroup.Where(i => i.WorkConfirmedByEricsson);
        //            var withGRItems = acceptedItems.Where(i => i.GRSend.HasValue);

        //            // если в сх позиций с GR больше либо равно чем в файле, значит файл старый. 
        //            if (withGRItems.Count()<sapRowByCode.GRQuantity)
        //            {
        //                // берем все принятые без GR
        //                var withoutGRItems = acceptedItems.Where(i => !i.GRSend.HasValue);

        //                // на этом этапе есть покод
        //                // сапкод
        //                // позиции из сх, по которым надо импорт произвести

        //                // надо вернуть набор позиций для импорт
        //                // набор позиций для GR
                        
        //            }

                    

        //        }
        //    }
            

        //}

    }
}
