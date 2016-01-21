using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using DbModels.Repository.Abstract;
using DbModels.DataContext;
using DbModels.Models;
using CommonFunctions.Extentions;
using DbModels.Models.ImportFilesModels;

namespace DbModels.Repository
{
    public class PriceListRepository:IRepository
    {
        private Context Context { get; set; }

        public List<SubContractor> GetSubcontractorsList()
        {
            return Context.SubContractors.ToList();
        }

        public PriceListRepository (Context context)
        {
            Context = context;
        }

        public PriceListRepository()
        {
            Context = new Context();
        }


        public IQueryable<PriceList> GetWorkablePriceLists(int SubcId)
        {
            return Context.PriceLists.Where(p => !p.Comparable&& p.SubContractor.Id==SubcId);
        }
        /// <summary>
        /// Прайс листы для сравнения. В обыжной жизнедеятельности не используется
        /// </summary>
        /// <param name="SubcId"></param>
        /// <returns></returns>
        public IQueryable<PriceList> GetComparablePriceLists(int SubcId)
        {
            return Context.PriceLists.Where(p => p.Comparable && p.SubContractor.Id == SubcId);
        }

        public List<PriceListRevisionItem> GetActivePriceListsRevisionItemsnew(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd)
        {
            List<PriceListRevisionItem> items = new List<PriceListRevisionItem>();
            // нужный подрядчик
            var subc = Context.SubContractors.FirstOrDefault(subcontr => subcontr.Id == SubcId);
            if (subc != null)
            {
                // все его прайс листы
                var priceLists = GetWorkablePriceLists(subc.Id).Where(p=>!p.Comparable).GroupJoin(
                    Context.PriceListRevisions.Where(r=>(WorkStart==null||r.SignDate<=WorkStart)
                                                        &&(WorkEnd==null||r.ExpiryDate==null||r.ExpiryDate>=WorkEnd)
                                                     )
                    ,
                    p => p.Id,
                    r => r.PriceList.Id,
                    (p, r) => new { PriceList = p, Revision = r.OrderByDescending(pl => pl.CreationDate).FirstOrDefault() }
                    );
                foreach (var el in priceLists)
                {
                    if (el.Revision != null)
                    {
                        foreach (var item in el.Revision.PriceListRevisionItems)
                        {
                            items.Add(item);
                        }
                    }
                }


            }
            return items;
        }

        public List<PriceListRevisionItem> GetActivePriceListsRevisionItems(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd)
        {
            List<PriceListRevisionItem> items = new List<PriceListRevisionItem>();
            // нужный подрядчик
            var subc = Context.SubContractors.FirstOrDefault(subcontr => subcontr.Id == SubcId);
            if (subc != null)
            {
                // все его прайс листы
                var priceLists = GetWorkablePriceLists(subc.Id).ToList();
                // если не даты пустые
                if ((WorkStart != null) && (WorkEnd != null))
                {
                    priceLists = priceLists.Where(pls => !pls.Comparable && pls.PriceListRevisions.OrderByDescending(pl => pl.CreationDate).FirstOrDefault().SignDate <= WorkStart).ToList();

                    priceLists = priceLists.Where(prls => prls.PriceListRevisions.OrderByDescending(pl => pl.CreationDate).FirstOrDefault().ExpiryDate == null || (prls.PriceListRevisions.OrderByDescending(pl => pl.CreationDate).FirstOrDefault().ExpiryDate >= WorkStart && prls.PriceListRevisions.OrderByDescending(pl => pl.CreationDate).FirstOrDefault().ExpiryDate >= WorkEnd)).ToList();
                    // priceLists = priceLists.Where(prls =>(prls.ExpiryDate >= WorkStart && prls.ExpiryDate <= WorkEnd));
                }
                foreach (PriceList priceList in priceLists)
                {
                    // последняя ревизия которая меньше указанной даты
                    PriceListRevision revision = null;
                    //DateTime now = DateTime.Now;
                    //if(WorkStart.HasValue)
                    //    revision = priceList.PriceListRevisions.OrderByDescending(pl => pl.Uploaded).FirstOrDefault(plr => plr.Uploaded.Date <=now);
                    //else
                    revision = priceList.PriceListRevisions.OrderByDescending(pl => pl.Uploaded).FirstOrDefault();
                    if (revision != null)
                    {
                        foreach (var item in revision.PriceListRevisionItems)
                        {
                            items.Add(item);
                        }
                    }
                }


            }
            return items;
        }

        public List<PriceListRevisionItem> GetActivePriceListsRevisionItems()
        {
            
            var activeItems = new List<PriceListRevisionItem>();
            foreach (var subc in Context.SubContractors)
            {
                var items = GetActivePriceListsRevisionItems(subc.Id, 4,null,null);
                activeItems.AddRange(items);
            }
            return activeItems;
        }

        public List<SAPCode> GetActivePriceListsItemsSAPCodes(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd)
        {
            var items = GetActivePriceListsRevisionItems( SubcId,  ProjectId,  WorkStart,  WorkEnd);

            var sapCodes = items.Select(it => it.SAPCode);
            return sapCodes.ToList();
        }

        public decimal GetActivePrice(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd, int SapCodeId, decimal Quantity, decimal coeff)
        {
            var item = GetActivePriceListsRevisionItems(SubcId, ProjectId, WorkStart, WorkEnd).FirstOrDefault(it => it.SAPCode.Id== SapCodeId);
            if (item != null)
                return Extentions.FinanceRound(item.Price * Quantity * coeff, 2);
            else
                return 0;
        }


        #region ComparePriceLists
        /// <summary>
        /// возвращает либо последний из опорных, либо первый из обычных
        /// </summary>
        /// <param name="subcId"></param>
        /// <returns></returns>
        //public PriceList GetComparablePriceList(int subcId)
        //{
        //    var compPls = GetComparablePriceLists(subcId);
        //   // var workPls = GetWorkablePriceLists(subcId);
        //    if (compPls.Count() > 0)
        //    {
        //        var compPl = compPls.SelectMany(p=>p.PriceListRevisions).OrderByDescending(c => c.CreationDate).FirstOrDefault().PriceList;
        //        return compPl;

        //    }
        //    else
        //    {
        //        //if (workPls.Count() > 1)
        //        //{
        //        //    var workPl = workPls.SelectMany(p => p.PriceListRevisions).OrderByDescending(r => r.SignDate).FirstOrDefault().PriceList;
        //        //    return workPl;
        //        //}
        //    }
        //    return null;

        //}
        
        public PriceListRevision GetLastPriceListRevision(int priceListId)
        {
            var workPlr =  Context.PriceLists.Find(priceListId);
            if (workPlr != null)
                return workPlr.PriceListRevisions.OrderByDescending(r => r.CreationDate).FirstOrDefault();
            else 
                return null;

        }
        
        /// <summary>
        /// если есть опорный прайс, то возвращает все неопорные. Если нет, то возвращает все, кроме первого.
        /// </summary>
        /// <param name="subcId"></param>
        /// <returns></returns>
        public IQueryable<PriceList> GetPriceListsForCompare(int subcId)
        {
            var compPls = GetComparablePriceLists(subcId);
            var workPls = GetWorkablePriceLists(subcId);
            if (compPls.Count() > 0)
            {

                return workPls;

            }
            else
            {
                if (workPls.Count() > 1)
                {
                    var workPlr = workPls.SelectMany(p => p.PriceListRevisions).OrderByDescending(r => r.SignDate).FirstOrDefault().PriceList;


                    return workPls.Where(pl=>pl.Id!=workPlr.Id);
                }
            }
            return null;

        }

        public IQueryable<PriceList> GetSubcontractorPriceLists(int subcId)
        {
            return Context.PriceLists.Where(p => p.SubContractor.Id == subcId);
        }

        public ImportFile GetPLRevisionImportFile(int revId)
        {
            var rev = Context.PriceListRevisions.Find(revId);
            if (rev != null)
            {
                if(rev.ImportFile!=null)
 
                return rev.ImportFile;
            }
            return null;
        }

        #endregion

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
        /// <summary>
        /// Проверяем активен ли данный айтем в данной ситуайции
        /// </summary>
        /// <returns></returns>
        public bool CheckIfPriceListItemActive(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd, int PriceListRevisionItemId )
        {
            var item = GetActivePriceListsRevisionItems(SubcId, ProjectId, WorkStart, WorkEnd).FirstOrDefault(it=>it.Id==PriceListRevisionItemId);
            return (item != null);
        }

        
        private bool ApprovableUser(string user)
        {
            List<string> users = new List<string>() { @"ERICSSON\ealgori", @"ERICSSON\echeale", @"ERICSSON\ealeigi" };
            return users.Contains(user);
        }
        public bool ApprovePriceListRevision(int revId,string userName)
        {
            var revision = Context.PriceListRevisions.Find(revId);
            if (revision != null)
            {
                if (revision.PriceList.Comparable)
                {
                    if(ApprovableUser(userName))
                    {
                    revision.Approved = true;
                    revision.ApprovedBy = userName;
                    revision.ApprovedDate = DateTime.Now;
                    return true;
                    }
                }
            }
            return false;
        }

        public bool DeleteComparablePriceList(int revId,string userName)
        {
            if (ApprovableUser(userName))
            {
                var revision = Context.PriceListRevisions.Find(revId);
                if (revision != null)
                {

                    var pList = revision.PriceList;
                    if (pList.Comparable)
                    {
                        var maps = Context.PriceListMaps.Where(p => p.ComparablePriceList.Id == pList.Id);
                        foreach (var map in maps)
                        {
                            Context.PriceListMaps.Remove(map);
                        }
                        foreach (var rev in pList.PriceListRevisions.ToList())
                        {
                            foreach (var item in rev.PriceListRevisionItems.ToList())
                            {
                                Context.PriceListRevisionItems.Remove(item);
                            }
                            Context.PriceListRevisions.Remove(rev);
                        }
                        Context.PriceLists.Remove(pList);
                        Context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DisApprovePriceListRevision(int revId,string userName)
        {
            var revision = Context.PriceListRevisions.Find(revId);
            if (revision != null)
            {
                if (revision.PriceList.Comparable)
                {
                    if (ApprovableUser(userName))
                    {
                        revision.Approved = false;
                        revision.ApprovedBy = userName;
                        revision.ApprovedDate = DateTime.Now;
                        return true;
                    }
                }
            }
            return false;
        }

        public List<int> GetCrossedItemsPLists(int pListId, int destSubcId)
        {
            List<int> crossedPls = new List<int>();
            var pList = Context.PriceLists.Find(pListId);
            if (pList != null)
            {
                var pListRev = GetLastPriceListRevision(pList.Id);
                if (pListRev != null)
                {
                    var subcPls = GetSubcontractorPriceLists(destSubcId);
                    foreach (var sPl in subcPls)
                    {
                        var sPlRev = GetLastPriceListRevision(sPl.Id);
                        var joinedItems = pListRev.PriceListRevisionItems.Join(sPlRev.PriceListRevisionItems, pli=>pli.Name, spli=>spli.Name,(pli,spli)=>new{pli,spli});
                        if (joinedItems.Count() > 0)
                        {
                            crossedPls.Add(sPl.Id);
                        }
                    }
                }
            }
            return crossedPls;
        }

    }
}
