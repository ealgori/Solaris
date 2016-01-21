using DbModels.DomainModels.ShClone;
using DbModels.Models;
using DbModels.Models.Pors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DataContext.Repositories
{
    public class AgreementRepository
    {
         private Context Context { get; set; }

       

        public AgreementRepository (Context context)
        {
            Context = context;
        }

        public AgreementRepository()
        {
            Context = new Context();
        }

        public List<ShTOItem> GetAgreementItems(string agreement)
        {
            if (string.IsNullOrEmpty(agreement))
                return new List<ShTOItem>();
            return Context.ShTOItems.Where(i=>i.AddAgreementId == agreement).ToList();
        }


        public List<PORTOItem> GetSATTOPORItemModels(List<ShTOItem> items, SubContractor subcontractor)
        {
          
          
            //    if (subcontractor == null)
            //    {
            //        return null;
            //    }
            //    var itemModels = items.Select((i, index) => new PORTOItem()
            //    {
            //        No = index + 1,
            //        Cat = "Service",
            //        Code = "ECR-ADD-TO-SOL",
            //        Plant = "2349",
            //        NetQty = i.,
            //        ItemCat = "N",
            //        PRtype = "3",
            //        POrg = "1439",
            //        GLacc = "402601",
            //        Price = i.PricePerItem,
            //        PRUnit = "1",
            //        Vendor = vendor.SAPNumber,
            //        Plandate = i.PlanDate,
            //        //Description = i.PriceListRevisionItem.Name,
            //        PriceListRevisionItem = i.PriceListRevisionItem,






            //    }).ToList();



            //    var startDate = itemModels.Min(a => a.Plandate);
            //    var endDate = itemModels.Max(a => a.Plandate);
            //    if (startDate.Value.Date == endDate.Value.Date)
            //        startDate = startDate.Value.AddDays(-1);

            //    // еще раз пробежимся для заполнения сапкодов
            //    for (int i = 0; i < itemModels.Count(); i++)
            //    {
            //        if (itemModels[i].PriceListRevisionItem != null)
            //        {

            //            itemModels[i].Description = itemModels[i].PriceListRevisionItem.Name;
            //            var plr = itemModels[i].PriceListRevisionItem;
            //            var name = plr.Name;
            //            var sapCode = Context.SAPCodes.FirstOrDefault(s => s.Vendor == vendor.SAPNumber && s.Description == name);
            //            if (sapCode != null)
            //            {
            //                itemModels[i].Code = sapCode.Code;
            //            }
            //            else
            //            {
            //                itemModels[i].Plandate = endDate;
            //            }

            //        }
            //        else
            //        {
            //            itemModels[i].Plandate = endDate;
            //        }
            //    }

            //    // а теперь сгруппируем то что получилось, но только сервисы... вот бред то...

            //    //    var groupped = itemModels.Where(t => t.Cat == "Service").GroupBy(g => g.Code).Select(
            //    //        (i, index) => new PORTOItem()
            //    //    {
            //    //        No = index + 1,
            //    //        Cat = i.FirstOrDefault().Cat,
            //    //        Code = i.FirstOrDefault().Code,
            //    //        Plant = "2349",
            //    //        NetQty = i.Sum(it => it.NetQty),
            //    //        ItemCat = "N",
            //    //        PRtype = "3",
            //    //        POrg = "1439",
            //    //        GLacc = "402601",
            //    //        Price = i.FirstOrDefault().Price,
            //    //        PRUnit = "1",
            //    //        Vendor = vendor.SAPNumber,
            //    //        Plandate = i.Max(p => p.Plandate),
            //    //        Description = i.FirstOrDefault().PriceListRevisionItem.Name,
            //    //        PriceListRevisionItem = i.FirstOrDefault().PriceListRevisionItem

            //    //    }

            //    //        ).ToList();

            //    //    groupped.AddRange(itemModels.Where(t=>t.Cat=="Material"));
            //    //  for (int i = 0; i < groupped.Count(); i++)
            //    //{
            //    //    groupped[i].No=i+1;
            //    //}


            //    return itemModels.ToList();
            //}
            return null;
        }
    }
}
