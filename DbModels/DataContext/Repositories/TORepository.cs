using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.DomainModels.ShClone;
using DbModels.Models;
using DbModels.DomainModels.Solaris.Pors;
using DbModels.DomainModels.SAT;
using DbModels.Models.Pors;
using CommonFunctions.Extentions;
using DbModels.Repository.Abstract;

namespace DbModels.DataContext.Repositories
{
    public class TORepository:IRepository
    {


        private Context Context { get; set; }
        public TORepository(Context context)
        {
            if (context == null)
            {
                Context = new Context();
            }
            else
                Context = context;
        }

        public List<ItemWSiteWFOL> GetToItemsExt(string TO)
        {
            var cachedSites = Context.ShSITEs.ToList();
            var cachedFols = Context.ShFOLs.ToList();
            var cachedToes = Context.ShTOes.ToList();
            var plItemsCache = Context.PriceListRevisionItems.ToList();
            var _items = GetTOItems(TO);
            var _itemsWsf = new List<ItemWSiteWFOL>();
            foreach (var item in _items)
            {
                var itemsf = new ItemWSiteWFOL();
                itemsf.ShItem = item;
                if (item.Site != null)
                {
                    itemsf.ShSite = cachedSites.FirstOrDefault(s => s.Site == item.Site);
                }
                if (item.FOL != null)
                {
                    itemsf.ShFOL = cachedFols.FirstOrDefault(f => f.FOL == item.FOL);
                }
                itemsf.ShTO = cachedToes.FirstOrDefault(t => t.TO == item.TOId);

                if (item.IDItemFromPL.HasValue)
                {
                    itemsf.PLRI = plItemsCache.FirstOrDefault(i => i.Id == item.IDItemFromPL.Value);
                }

                if (itemsf.ShSite != null || itemsf.ShFOL != null)
                {
                    _itemsWsf.Add(itemsf);
                }


            }
            return _itemsWsf;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="TO"></param>
        /// <param name="simplify"></param>
        /// <param name="exteded">Добавить в модели объекты из сх, для использования их свойств</param>
        /// <returns></returns>
        public IEnumerable<TOItemViewModel> GetTOItemModels(string TO, bool simplify=false, bool exteded=false)
        {
            var shTo = Context.ShTOes.Find(TO);
            
            if (shTo != null)
            {
                var _itemsWsf = GetToItemsExt(TO);

                var items = _itemsWsf
                    //.Join(Context.ShTOes, i => i.i.TOId, t => t.TO, (i, t) => new { t, i.i, i.s, i.f })
                      //                     .GroupJoin(plItemsCache, t=>t.i.IDItemFromPL, pli=>pli.Id, (t,pli)=>new {t.t, t.i,t.s, t.f ,pli,  })
                      .ToList();
                if(simplify)
                {
                    return items.Where(i => i.ShTO.TOType == shTo.TOType).Where(s=>s.ShSite!=null).Select(i => new TOItemViewModel()
                    {
                        Site = i.ShSite.Site,
                        TO = i.ShTO.TO,
                        TOItem = i.ShItem.TOItem,
                        SiteAddress = i.ShSite.Address,
                        //SiteQuantity = i.s.KolvoAMS,
                        ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                        //Description = i.s.VidTOAMS,
                        SiteRegion = i.ShSite.MacroRegion,
                        SiteBranch = i.ShSite.Branch,
                        TOPlanDate = i.ShItem.TOPlanDate,
                        Price = i.PLRI != null ? i.PLRI.Price : 0,
                        //Total = i.pli.Count() > 0 ? i.pli.FirstOrDefault().Price * i.s.KolvoAMS : 0


                    });
                }

                var itemsViewModels = new List<TOItemViewModel>();

                switch (shTo.TOType)
                {
                    case "АМС":
                        {
                            itemsViewModels = items.Where(i => i.ShTO.TOType == "АМС").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            { 
                                Site = i.ShSite.Site, 
                                TO = i.ShTO.TO, 
                                TOItem = i.ShItem.TOItem, 
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoAMS,
                                ItemId = i.ShItem.IDItemFromPL.HasValue?i.ShItem.IDItemFromPL.Value:0,
                                Description = i.ShSite.VidTOAMS,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI!=null?i.PLRI.Price:0,
                                Total = i.PLRI!=null? i.PLRI.Price*i.ShSite.KolvoAMS : 0
                            
                            
                            }).ToList();
                            break;
                        }
                    case "СКВ":
                        {
                            itemsViewModels= items.Where(i => i.ShTO.TOType == "СКВ").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoSKV,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.VidTOSKV,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * i.ShSite.KolvoSKV,
                               


                            }).ToList();
                            break;
                        }
                    case "пСКВ":
                        {
                            itemsViewModels= items.Where(i => i.ShTO.TOType == "пСКВ").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvopSKV,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TippSKV,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI!=null?i.PLRI.Price:0 * i.ShSite.KolvopSKV 


                            }).ToList();
                            break;
                        }
                    case "Сайт ТО":
                        {
                            itemsViewModels = items.Where(i => i.ShTO.TOType == "Сайт ТО").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = 1,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = "ТО для сайта",
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * 1


                            }).ToList() ;
                            break;
                        }
                    case "АУГПТ":
                        {
                            itemsViewModels = items.Where(i => i.ShTO.TOType == "АУГПТ").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoAUGPT,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TipAUGPT,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * i.ShSite.KolvoAUGPT 

                            }).ToList();
                            break;
                        }
                    case "СТАЦИОНАРНЫХ ГУ":
                        {
                            itemsViewModels= items.Where(i => i.ShTO.TOType == "СТАЦИОНАРНЫХ ГУ").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoStacionarnihGU,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TipStatcionarnoiGU,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * i.ShSite.KolvoStacionarnihGU 

                            }).ToList();
                            break;
                        }
                    case "МОБИЛЬНЫХ ГУ":
                        {
                            return itemsViewModels.Where(i => i.ShTO.TOType == "МОБИЛЬНЫХ ГУ").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoMobilnihGU,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TipMobilnoiGU,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * i.ShSite.KolvoMobilnihGU 

                            }).ToList();
                            break;
                        }

                    case "ТО ВОЛС":
                        {
                            var volsTOItems = items.Where(i => i.ShTO.TOType == "ТО ВОЛС").ToList();
                            //List<TOItemViewModel> volsTOItemsVM = new List<TOItemViewModel>();
                            foreach (var item in volsTOItems)
                            {
                                var volsItem = new TOItemViewModel();
                                if (item.ShFOL!=null)
                                {
                                    volsItem.Site = string.Format("{0}({1})", item.ShFOL.FOL, item.ShItem.TOPlanDate.HasValue ? item.ShItem.TOPlanDate.Value.ToString("ddMMyyyy") : "no date");
                                    volsItem.FOL = string.Format("{0}({1})", item.ShFOL.FOL, item.ShItem.TOPlanDate.HasValue
                                                    ? item.ShItem.TOPlanDate.Value.ToString("dd.MM.yyyy")
                                                    : "no date");
                                    volsItem.SiteAddress = string.Format("{0}-{1}", item.ShFOL.StartPoint, item.ShFOL.DestinationPoint);
                                    volsItem.SiteQuantity = 1;
                                    volsItem.Description = item.ShFOL.FOL;
                                    
                                }
                                else
                                {
                                    if(item.ShSite!=null)
                                    {
                                        volsItem.Site = item.ShSite.Site;
                                        
                                        volsItem.SiteAddress = item.ShSite.Address;
                                        volsItem.SiteQuantity = item.ShSite.KolvoVOLS;
                                        volsItem.Description = item.ShSite.VidTOVOLS;
                                        volsItem.SiteRegion = item.ShSite.MacroRegion;
                                        volsItem.SiteBranch = item.ShSite.Branch;
                                    }
                                }

                                volsItem.TO = item.ShTO.TO;
                                volsItem.TOItem = item.ShItem.TOItem;
                                volsItem.ItemId = item.ShItem.IDItemFromPL.HasValue ? item.ShItem.IDItemFromPL.Value : 0;
                                volsItem.TOPlanDate = item.ShItem.TOPlanDate;
                                volsItem.Price = item.PLRI != null ? item.PLRI.Price : 0;
                                volsItem.Total = item.PLRI != null ? item.PLRI.Price : 0 * 1;

                                itemsViewModels.Add(volsItem);
                            }
                          
                            
                            
                            
                            //.Select(i => new TOItemViewModel()
                            //{
                            //    // Волсы только к фолам
                            //    Site = i.ShSite != null ? i.ShSite.Site : 
                            //    string.Format("{0}({1})", i.ShFOL.FOL, i.ShItem.TOPlanDate.HasValue ? i.ShItem.TOPlanDate.Value.ToString("ddMMyyyy"):"no date"),
                            //    FOL = i.ShFOL!=null
                            //        ?string.Format("{0}({1})", i.ShFOL.FOL, i.ShItem.TOPlanDate.HasValue 
                            //            ? i.ShItem.TOPlanDate.Value.ToString("dd.MM.yyyy") 
                            //            : "no date")
                            //        :"not mounted",

                            //    TO = i.ShTO.TO,
                            //    TOItem = i.ShItem.TOItem,
                            //    SiteAddress =  i.ShSite!=null?i.ShSite.Address:string.Format("{0}-{1}",i.ShFOL.StartPoint, i.ShFOL.DestinationPoint),

                            //    SiteQuantity = 1, // 1 тк. 1 раз в месяц. я так понимаю. ежемесячно
                            //    ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                            //    Description = i.ShFOL!=null?i.ShFOL.FOL+"_":"",
                            //    SiteRegion = i.ShSite != null ? i.ShSite.MacroRegion:null,
                            //    SiteBranch = i.ShSite != null ? i.ShSite.Branch:null,
                                
                            //    TOPlanDate = i.ShItem.TOPlanDate,
                            //    Price = i.PLRI != null ? i.PLRI.Price : 0,
                            //    Total = i.PLRI != null ? i.PLRI.Price : 0 * 1

                            //}).ToList();
                            break;
                        }

                    case "ТО БС":
                        {
                            itemsViewModels= items.Where(i => i.ShTO.TOType == "ТО БС").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = 1,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TipMobilnoiGU,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * 1 

                            }).ToList();
                            break;
                        }

                    //case "Сайт ТО":
                    //    {
                    //        return items.Where(i => i.t.TOType == "Сайт ТО").Select(i => new TOItemViewModel()
                    //        {
                    //            Site = i.s.Site,
                    //            TO = i.t.TO,
                    //            TOItem = i.i.TOItem,
                    //            SiteAddress = i.s.Address,
                    //            SiteQuantity = 1,
                    //            ItemId = i.i.IDItemFromPL.HasValue ? i.i.IDItemFromPL.Value : 0,
                    //            Description = "",
                    //            SiteRegion = i.s.MacroRegion,
                    //            SiteBranch = i.s.Branch,
                    //            TOPlanDate = i.i.TOPlanDate,
                    //            Price = i.pli.Count() > 0 ? i.pli.FirstOrDefault().Price : 0,
                    //            Total = i.pli.Count() > 0 ? i.pli.FirstOrDefault().Price * 1 : 0

                    //        });
                    //    }

                    case "ТО прочее":
                        {
                            itemsViewModels= items.Where(i => i.ShTO.TOType == "ТО прочее").Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = i.ShSite.KolvoProchee,
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.VidTOprochee,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * 1 

                            }).ToList();
                            break;
                        }


                    default:
                        {
                            itemsViewModels= items.Where(s => s.ShSite != null).Select(i => new TOItemViewModel()
                            {
                                Site = i.ShSite.Site,
                                TO = i.ShTO.TO,
                                TOItem = i.ShItem.TOItem,
                                SiteAddress = i.ShSite.Address,
                                SiteQuantity = 1, // 1 тк. 1 раз в месяц. я так понимаю. ежемесячно
                                ItemId = i.ShItem.IDItemFromPL.HasValue ? i.ShItem.IDItemFromPL.Value : 0,
                                Description = i.ShSite.TipMobilnoiGU,
                                SiteRegion = i.ShSite.MacroRegion,
                                SiteBranch = i.ShSite.Branch,
                                TOPlanDate = i.ShItem.TOPlanDate,
                                Price = i.PLRI != null ? i.PLRI.Price : 0,
                                Total = i.PLRI != null ? i.PLRI.Price : 0 * 1 

                            }).ToList();
                            break;
                        }

                }

                if(exteded)
                {
                    foreach (var item in itemsViewModels)
                    {
                        var extModel = items.FirstOrDefault(i=>i.ShItem.TOItem== item.TOItem);
                        item.ShItem = extModel.ShItem;
                        item.ShSite = extModel.ShSite;
                        item.ShFOL = extModel.ShFOL;
                        item.PLRI = extModel.PLRI;
                    }
                }
                return itemsViewModels;

                
            }
            return null;
        }

        public class ItemWSiteWFOL
        {
            public ShTOItem ShItem { get; set; }
            public ShSITE ShSite { get; set; }
            public ShFOL ShFOL { get; set; }
            public ShTO ShTO { get; set; }
            public PriceListRevisionItem PLRI { get; set; }
        }



        public IQueryable<ShMatToItem> GetToMaterialItems(string TO)
        {
            return Context.ShMatTOItems.Where(t => t.TOId == TO);
        }
        public IEnumerable<TOItemViewModel> GetTOEquipmentItemModels(string TO)
        {
            var shTo = Context.ShTOes.Find(TO);

            if (shTo != null)
            {
                var items = GetTOItems(TO).Join(Context.ShSITEs, s => s.Site, t => t.Site, (i, s) => new { i, s })
                                           .Join(Context.ShTOes, i => i.i.TOId, t => t.TO, (i, t) => new { t, i.i, i.s })
                                           .GroupJoin(Context.PriceListRevisionItems, t => t.i.IDItemFromPL, pli => pli.Id, (t, pli) => new { t.t, t.i, t.s, pli }).ToList();

                return items.Select(i => new TOItemViewModel()
                {
                    Site = i.s.Site,
                    TO = i.t.TO,
                    TOItem = i.i.TOItem,
                    SiteAddress = i.s.Address,
                    SiteQuantity = i.i.EquipmentQuantity,
                    ItemId = i.i.IDItemFromPL.HasValue ? i.i.IDItemFromPL.Value : 0,
                    Description = i.i.EquipmentName,
                    SiteRegion = i.s.MacroRegion,
                    SiteBranch = i.s.Branch,
                    TOPlanDate = i.i.TOPlanDate,
                    Price = i.pli.Count() > 0 ? i.pli.FirstOrDefault().Price : 0,
                    Total = i.pli.Count() > 0 ? i.pli.FirstOrDefault().Price * i.s.KolvoAMS : 0


                });
            }
            return null;
                 
        }


        public IEnumerable<MatItemViewModel> GetTOMatItemModels(string TO)
        {
            var matItems = GetToMaterialItems(TO);
            if (matItems.Count() > 0)
            {
                var matGJSite = matItems.GroupJoin(Context.ShSITEs, s => s.SiteId, t => t.Site, (i, s) => new { i, s })
                                           .Join(Context.ShTOes, i => i.i.TOId, t => t.TO, (i, t) => new { t, i.i, i.s })
                                           .GroupJoin(Context.PriceListRevisionItems, t=>t.i.IDItemFromPL, pli=>pli.Id, (t,pli)=>new {t.t, t.i,t.s,pli }).ToList();
                var matItemModels = matGJSite.Select(m => new MatItemViewModel()
                {
                    Description = m.i.Description,
                    ItemId = m.i.IDItemFromPL.HasValue ? m.i.IDItemFromPL.Value : 0,
                    MatItem = m.i.MatTOId,
                    Price = m.i.Price.HasValue ? m.i.Price : 0,
                    Site = m.s.FirstOrDefault() == null ? "" : m.s.FirstOrDefault().Site,
                    SiteAddress = m.s.FirstOrDefault() == null ? "" : m.s.FirstOrDefault().Address,
                    SiteBranch = m.s.FirstOrDefault() == null ? "" : m.s.FirstOrDefault().Branch,
                    SiteRegion = m.s.FirstOrDefault() == null ? "" : m.s.FirstOrDefault().MacroRegion,
                    Quantity = m.i.Quantity,
                    TO = m.t.TO,
                    ECRADD = ((m.i.Price.HasValue&&m.i.Price!=0)&&!m.i.IDItemFromPL.HasValue)?true:false,
                    Total = m.i.Price.HasValue? m.i.Price*m.i.Quantity:0

                });

                return matItemModels;
            }
            return null;
        }

        public IQueryable<ShTOItem> GetTOItems(string TO)
        {
            return Context.ShTOItems.Where(i => i.TOId == TO);
        }

        public IQueryable<ShTOItem> GetTOItems(ShTO TO)
        {
            return GetTOItems(TO.TO);
        }

        public SubContractor GetTOSubContractor(string TO)
        {
            var shTO = Context.ShTOes.Find(TO);
            if (shTO != null)
            {
                return Context.SubContractors.FirstOrDefault(s => s.ShName == shTO.Subcontractor);
                
            }
            return null;
        }

        public IEnumerable<ShTO> GetAcceptedToList()
        {
            var test1 = Context.ShTOes.Where(t => t.TO == "2016_ТЮМЕНЬ_ТО_АУГПТ").ToList();
            return Context.ShTOes.Where(t => 
            !string.IsNullOrEmpty(t.TOapproved)
            &&string.IsNullOrEmpty(t.TOTotalAmmountApproved) // стоимость увтерждаю, выпустить заказ
            && !t.NotForPOR)
            .ToList();
        }
        //больше не используется . теперь активности и нетворки в сайтхендлере
        public PORActivity GetToActivity1(string TO)
        {
            var shTO = Context.ShTOes.Find(TO);
            if (shTO != null)
            {
                var activity = Context.PORActivities.FirstOrDefault(act => act.TOType == shTO.TOType);
                return activity;
            }
            return null;
        }


        private ShSITE GetToFirstItemSite(string TO)
        {
            var serv = GetTOItems(TO);
            if (serv.Count() > 0)
            {
                var firstServ = serv.FirstOrDefault();
                ShSITE shSite = Context.ShSITEs.FirstOrDefault(s => s.Site == firstServ.Site);
                return shSite;

            }
            return null;
        }

        public string GetToRegion(string TO)
        {
            var shSite = GetToFirstItemSite(TO);
            if (shSite != null)
            {
                return shSite.MacroRegion;
            }
            return "Не указан";
        }

        public string GetToBranch(string TO)
        {
            var shSite = GetToFirstItemSite(TO);
            if (shSite != null)
            {
                return shSite.Branch;
            }
            return "Не указан";
        }


        public List<SATTO> GetLastSATTOList()
        {
            var result = Context.SATTOs.GroupBy(s => s.TO).Select(
                   group => group.
                       OrderByDescending(t => t.CreateUserDate)
                       .FirstOrDefault()
                   ).OrderByDescending(s=>s.CreateUserDate).ToList();
            return result;
        }
        // эта модель использоуется так же в реколах и дел запросах. 
        public List<PORTOItem> GetSATTOPORItemModels(int id)
        {
            string ecrAddToSol = "ECR-ADD-TO-SOL";
            var satTo = Context.SATTOs.Find(id);
            if (satTo != null)
            {
                var vendor = Context.SubContractors.FirstOrDefault(s => s.ShName == satTo.SubContractor|| s.SAPName== satTo.SubContractor||s.Name == satTo.SubContractor||s.NameRef==satTo.SubContractor);
                if (vendor == null)
                {
                    return null;
                }
                var itemModels = satTo.SATTOItems.ToList().Select((i, index) => new PORTOItem()
                {
                    No= index+1 ,
                    Cat=i.Type,
                    Code = ecrAddToSol,
                    Plant="2349",
                    NetQty = i.Quantity,
                    ItemCat="N",
                    PRtype="3",
                    POrg="1439",
                    GLacc= "402601",
                    Price = i.PricePerItem,
                    PRUnit="1",
                    Vendor = vendor.SAPNumber,
                    Plandate =  i.PlanDate,
                    //Description = i.PriceListRevisionItem.Name,
                    PriceListRevisionItem = i.PriceListRevisionItem,
                    ItemId = i.TOItemId
                 
                    



                     
                }).ToList();



                var startDate = itemModels.Min(a => a.Plandate);
                var endDate = itemModels.Max(a => a.Plandate);
                if (startDate.Value.Date == endDate.Value.Date)
                    startDate = startDate.Value.AddDays(-1);

                // еще раз пробежимся для заполнения сапкодов
                for (int i=0; i< itemModels.Count();i++)
                {
                    if (itemModels[i].PriceListRevisionItem != null)
                    {

                        itemModels[i].Description = itemModels[i].PriceListRevisionItem.Name;
                        var plr = itemModels[i].PriceListRevisionItem;
                        var name = plr.Name;
                        var sapCode = Context.SAPCodes.FirstOrDefault(s => s.Vendor == vendor.SAPNumber && s.Description == name);
                        if (sapCode != null)
                        {
                            itemModels[i].Code = sapCode.Code;
                        }
                        else
                        {
                            itemModels[i].Plandate = endDate;
                        }

                    }
                    else
                    {
                        itemModels[i].Plandate = endDate;
                    }
                }

                // а теперь сгруппируем то что получилось, но только сервисы... вот бред то...

             
            //    groupped.AddRange(itemModels.Where(t=>t.Cat=="Material"));
            //  for (int i = 0; i < groupped.Count(); i++)
            //{
            //    groupped[i].No=i+1;
            //}


                return itemModels.ToList();
            }
            return null;
        }

          public List<PORTOItem> GetSATTOPORItemModelsGroupped(int id)
        {
            string ecrAddToSol = "ECR-ADD-TO-SOL";
              var itemModels = GetSATTOPORItemModels(id);
            var groupped = itemModels.Where(t => t.Cat == "Service" && t.Code != ecrAddToSol).GroupBy(g => g.Code).Select(
             (i, index) => new PORTOItem()
             {
                 No = index + 1,
                 Cat = i.FirstOrDefault().Cat,
                 Code = i.FirstOrDefault().Code,
                 Plant = "2349",
                 NetQty = i.Sum(it => it.NetQty),
                 ItemCat = "N",
                 PRtype = "3",
                 POrg = "1439",
                 GLacc = "402601",
                 Price = i.FirstOrDefault().Price,
                 PRUnit = "1",
                 Vendor = i.FirstOrDefault().Vendor,
                 Plandate = i.Max(p => p.Plandate),
                 Description = i.FirstOrDefault().PriceListRevisionItem.Name,
                 PriceListRevisionItem = i.FirstOrDefault().PriceListRevisionItem
                 //ItemId = "Нет, потому что у нас группировка"
                  

               }

               ).ToList();
            groupped.AddRange(itemModels.Where(t => t.Code == ecrAddToSol || t.Cat != "Service"));
            // восстановим индексы.
            var no = 1;
            foreach (var item in groupped)
            {
                item.No = no++;
            }
            return groupped;
        }

        public List<TOSostavRabotModel> GetTOSostavRabot(string TO)
        {
            var shTO = Context.ShTOes.Find(TO);
            if (shTO != null)
            {
                var shSR = Context.ShSostavRabotTOs.FirstOrDefault(s => s.SostavRabotTOid == shTO.SostavRabotTOid);
                if (shSR != null)
                {
                    var shSRItems = Context.ShSostavRabotTOItems.Where(i => i.SostavRabotTOid == shSR.SostavRabotTOid).ToList()
                        .Select((i,ind) => new TOSostavRabotModel()
                        {
                             Id = ind+1,
                              Description = i.Description,
                                Price = i.Price.HasValue? i.Price.Value:0,
                                 Quantity = i.Quantity.HasValue? i.Quantity.Value:0,
                             Total = (i.Price.HasValue ? i.Price.Value : 0) * (i.Quantity.HasValue ? i.Quantity.Value : 0)
                                  
                                  
                        });
                    return shSRItems.ToList();
                }
            }
            return null;
        }

        public class TOSostavRabotModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
        }

        public class TOItemViewModel
        {
            public string TO { get; set; }
            public string TOItem { get; set; }
            public string Site { get; set; }
            public string SiteAddress { get; set; }
            public decimal? SiteQuantity { get; set; }
            /// <summary>
            /// Место для всплывающего меню
            /// </summary>
            public int ItemId { get; set; }
            public string SiteRegion { get; set; }
            public string SiteBranch { get;set; }
            public string Description { get; set; }
            public string FIX { get; set; }
            public string FOL { get; set; }
            public DateTime? TOPlanDate { get; set; }
            public decimal? Price { get; set; }
            public decimal? Total { get; set; }

            public ShTOItem ShItem { get; set; }
            public ShSITE ShSite { get; set; }
            public ShFOL ShFOL { get; set; }
            public ShTO ShTO { get; set; }
            public PriceListRevisionItem PLRI { get; set; }




        }

        public class MatItemViewModel
        {
            public string TO { get; set; }
            public string MatItem { get; set; }
            public string Site { get; set; }
            public string SiteAddress { get; set; }
            public decimal? Quantity { get; set; }
            /// <summary>
            /// Место для всплывающего меню
            /// </summary>
            public int ItemId { get; set; }
            public string SiteRegion { get; set; }
            public string SiteBranch { get;set; }
            public string Description { get; set; }
            public string FIX { get; set; }
            public string FOL { get; set; }
            public DateTime? TOPlanDate { get; set; }
            public decimal? Price { get; set; }
            public decimal? Total { get; set; }
            public bool ECRADD { get; set; }
        }



        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }

   
}
