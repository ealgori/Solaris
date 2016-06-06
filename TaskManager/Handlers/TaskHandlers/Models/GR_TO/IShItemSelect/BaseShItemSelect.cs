using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using CommonFunctions.Extentions;
using System.Diagnostics;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO
{
    /// <summary>
    /// мы уже знаем,  сколько GR нам не хватает, судя по сап.
    /// среди всех позиций сх, надо выбрать позицию(и) с необходимым количеством, у которой нет GR
    /// </summary>
    public class BaseShItemSelect : IShItemSelect
    {
       

        public bool Select(List<ShItemModel> shItems, decimal? qty, out List<ShItemModel> selected)
        {
            selected = new List<ShItemModel>();

            // нас интересуют только позиции без GR
            shItems = shItems.Where(s => string.IsNullOrEmpty(s.GR)).OrderByDescending(i=>i.TOFactDate ).ToList();

            var totalQty = shItems.Sum(s => s.Qty);
            if(totalQty<qty)
            {
                /// заданного количества в данных позицях не наберется
                return false;
            }

            var sameQtyItem = shItems.FirstOrDefault(i => i.Qty == qty);
            if (sameQtyItem!=null)
            {
                // попалась позиция именно такого количества
                selected.Add(sameQtyItem);
                return true;
            }

            if (shItems.Sum(i => i.Qty) == qty)
            {
                selected = shItems;
                return true;
            }

            var _tempList = new List<ShItemModel>();
            foreach (var item in shItems)
            {
                _tempList.Add(item);
                if(_tempList.Sum(i=>i.Qty)==qty)
                {
                    selected = _tempList;
                    return true;
                }
            }


            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            for (int i = 2; i <= shItems.Count; i++)
            {
               
                watch.Start();
                var combs = shItems.GetKCombs(i);
                var suitable = combs.FirstOrDefault(it => it.Sum(b => b.Qty)==qty);
                if(suitable!=null)
                {
                    selected = suitable.ToList();
                    return true;
                }
                watch.Stop();
                Console.WriteLine("{0}:{1} - {2}", i, shItems.Count, watch.Elapsed.ToString());
                Debug.WriteLine("{0}:{1} - {2}", i, shItems.Count, watch.Elapsed.ToString());
                watch.Reset();


            }
            return false;



        }
    }
}
