using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Repository.Abstract;
using DbModels.DomainModels.ShClone;
using DbModels.DomainModels.SAT;
using DbModels.SharedModels;

namespace DbModels.DataContext.Repositories
{
    public class ActRepository : IRepository
    {

        public Context Context { get; set; }
        private List<ShTOItem> _cachedShToItems;
        private List<ShMatToItem> _cachedShMatToItems;
        private List<ShSITE> _cachedShSites;
        private List<ShAct> _cachedActs;
       

        public ActRepository(Context context = null)
        {
            if (context == null)
                Context = new Context();
            else
            Context = context;
            _cachedShToItems = context.ShTOItems.ToList();
            _cachedShMatToItems = context.ShMatTOItems.ToList();
            _cachedShSites = context.ShSITEs.ToList();
            _cachedActs = context.ShActs.ToList();
            
        }





        public List<SATActService> GetSATActServices(SATAct act)
        {
            if (act.SATActItems != null)
            {
                return act.SATActItems.Where(i => i is SATActService).Cast<SATActService>().ToList();
            }
            return null;
        }

        public List<SATActMaterial> GetSATActMaterials(SATAct act)
        {
            if (act.SATActItems != null)
            {
                return act.SATActItems.Where(i => i is SATActMaterial).Cast<SATActMaterial>().ToList();
            }
            return null;
        }

        public List<SATAct> GetReadyToPrintActs()
        {
            return Context.SATActs.Where(a => a.UploadedToSH).ToList();
        }





        private List<ShMatToItem> GetToMatItems(string TO)
        {

            return _cachedShMatToItems.Where(t => t.TOId == TO).ToList();
        }
        /// <summary>
        /// оптимизации не требует
        /// </summary>
        /// <param name="TO"></param>
        /// <returns></returns>
        private List<ShTOItem> GetTOServItems(string TO,bool filter)
        {
            
                
                var result = _cachedShToItems.Where(i =>
                i.WorkConfirmedByEricsson &&
              
                i.TOId == TO
                // количества берутся с сайтов
                ////10.08.2015. обработаем нулевые количества
                //&& i.Quantity.HasValue
                //&& i.Quantity>0
                ).ToList();
               
                
                if(filter)
                {
                    result = result.Where(i=>string.IsNullOrEmpty(i.ActId)).ToList();
                }
                else
                {
                    // доп фильтр на позиции по принятым актам
                    var services = new List<ShTOItem>();
                    foreach (var item in result)
                    {
                        if (!string.IsNullOrEmpty(item.ActId))
                        {
                            var shAct = _cachedActs.FirstOrDefault(a => a.Act == item.ActId);
                            if(shAct!=null)
                            {
                                if(!shAct.GetActLink)
                                {
                                    services.Add(item);
                                }
                            }
                        }
                        else
                        {
                            services.Add(item);
                        }
                    }
                    result = services;
                }
            return result;
        }
        /// <summary>
        /// TO по которым можно выпустить акт хоть на что нибудь. В дропдауне, а так же будем проверять можно ли выпускать акт через эту функцию
        /// </summary>
        /// <returns></returns>
        public List<ShTO> GetReadyTOForAct(string year = null,bool filter=false)
        {
            //var toes = GetTOWithPONumber(year).ToList().Where((t =>
            //    //(t.Year=="2015")&&
            //    t.ObichniyRegulyarniyTO != "Регулярный без подтверждения выполнения работ" &&
            //    (GetToItems(t.TO).Materials.Count() > 0 || GetToItems(t.TO).Services.Count() > 0))).ToList();
            //return toes;
            var toes = GetTOWithPONumber(year).ToList().Where(t =>
               
             t.ObichniyRegulyarniyTO != "Регулярный без подтверждения выполнения работ").ToList();
           
            var debugTO = toes.Select(t => t.TO).ToList();
            var readyToes = new List<ShTO>();
             foreach (var to in toes)
	        {
		        var items = GetToItems(to.TO,filter);
                if(items.Materials.Count()>0||items.Services.Count>0)
                    readyToes.Add(to);

	        }
             
            return readyToes;
        }

        /// <summary>
        /// оптимизации не требует
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private IQueryable<ShTO> GetTOWithPONumber(string year)
        {
            var toes = Context.ShTOes.Where(t => !string.IsNullOrEmpty(t.PONumber));
            if (!string.IsNullOrEmpty(year))
                toes = toes.Where(t => t.Year == year);
            return toes;
        }
        /// <summary>
        /// Серсивы и подходящие к ним материалы для ТО
        /// </summary>
        /// <param name="TO"></param>
        /// <returns></returns>
        public ActItemModels GetToItems(string TO, bool filter =true)
        {
            var serv = GetTOServItems(TO,filter);
            var materials = GetToMatItems(TO);
            var servSites = serv.Select(s => s.Site);
            var mats = new List<ShMatToItem>();
            foreach (var mat in materials)
            {
                
                if(string.IsNullOrEmpty(mat.SiteId) || servSites.Contains(mat.SiteId))
                {
                    mats.Add(mat);
                }
            }
           // var materialsForServices = servSites.Join(materials, s => s, m => m.SiteId, (s, m) => m).ToList();
           // var emptySiteMaterials = materials.Where(m => string.IsNullOrEmpty(m.SiteId)).ToList();
            return new ActItemModels()
            {
                Services = serv.ToList(),
                Materials = mats //materialsForServices.Union(emptySiteMaterials).ToList()
            };
        }






        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        #region Act create
        /// <summary>
        /// Функции вынесены из интернета, для последующего использования
        /// запрещаю акты только на материалы
        /// </summary>
        /// <returns></returns>
        public SATAct GetSatAct(string to, string user, DateTime startDate, DateTime endDate, bool filter)
        {
            string actName = string.Format("{0}", DateTime.Now.ToString("MMdd-HHmmss"));



            ShTO shTO = GetReadyTOForAct( filter:filter).FirstOrDefault(t => t.TO == to);
            if (shTO == null)
            {
                throw new Exception(string.Format("TO не существует, либо не готово к созданию акта : {0}", to));
            }

            SATAct act = new SATAct();
            act.ActName = actName;
            act.CreateDate = DateTime.Now;
            act.CreateName = user;
            act.TO = shTO.TO;
            act.StartDate = startDate;
            act.EndDate = endDate;


            var subcontractor = Context.SubContractors.FirstOrDefault(s => s.ShName == shTO.Subcontractor);
            if (subcontractor != null)
            {
                act.SubContractor = subcontractor.Name;
                act.SubContractorSapNumber = subcontractor.SAPNumber;
                act.SubContractorAddress = subcontractor.Address;
            }

            //act.ToType = shTO.TOType;


            act.NomerDogovora = shTO.NomerDogovora;
            act.DataDogovora = shTO.DataDogovora;
            // проверка на ВАТ
            var shContact = Context.ShContacts.FirstOrDefault(c => c.Contact == shTO.Subcontractor);
            if (shContact == null)
            {
                throw new Exception("Ошибка.. в СХ отсутсвтует контакт с именем " + shTO.Subcontractor);

            }
            act.WOVAT = shContact.WithOutVAT;
            act.PONumber = shTO.PONumber;
            act.PODate = shTO.POIssueDate;

            // надо где то его достать      
            var items = GetToItems(to);

            foreach (var item in items.Services)
            {
                var shSite = _cachedShSites.FirstOrDefault(s=>s.Site==item.Site);
                if (shSite != null)
                {
                    if (string.IsNullOrEmpty(act.Region))
                    {
                        act.Region = shSite.MacroRegion;
                    }
                    if (string.IsNullOrEmpty(act.Branch))
                    {
                        act.Branch = shSite.Branch;
                    }
                    if (string.IsNullOrEmpty(act.Network))
                    {
                        var network = shTO.Network;//context.PORNetworks.FirstOrDefault(n => n.SiteBranch == act.Branch);
                        if (network != null)
                        {
                            act.Network = network;
                        }
                    }
                }
            }
            // на тот случай если акт только для материалов


            return act;
        }
        /// <summary>
        /// Получаем позиции из данных пришедших с формы
        /// </summary>
        /// <param name="to"></param>
        /// <param name="servicesModels"></param>
        /// <param name="all">Обрабатывать не выбранные позиции</param>
        /// <returns></returns>
        public List<SATActService> GetSATServices(string to, List<ActItemModel> servicesModels,bool filter, bool all=false )
        {
            List<SATActService> services = new List<SATActService>();
            var toItems = GetToItems(to,filter);
            if (servicesModels == null)
                servicesModels = new List<ActItemModel>();
            if (!servicesModels.Any(m => m.Checked))
                all = true;
            if (!all)
                servicesModels = servicesModels.Where(s => s.Checked).ToList();
            var selectedServices = servicesModels.GroupJoin(toItems.Services, s => s.Id, ss => ss.TOItem, (mi, si) => new { modelItem = mi, shItem = si.FirstOrDefault() });
            if (selectedServices.Any(s => s.shItem == null))
            {
                var unlinked = selectedServices.Where(s => s.shItem == null).Select(s=>s.modelItem.Id);
                throw new Exception(string.Format("Некоторые из отмеченых позиций не пренадлежат данному ТО, либо по ним уже выпущен акт:ТО - {0}, items:{1}", to,string.Join(", ",unlinked)));
            }




            foreach (var serv in selectedServices)
            {
                SATActService item = new SATActService();
                item.Description = serv.shItem.DescriptionFromPL;
                item.FactDate = serv.shItem.TOFactDate.Value;

                item.Price = serv.shItem.PriceFromPL.HasValue ? serv.shItem.PriceFromPL.Value : 0;//!!!!!!!!!!!!!!
                item.Quantity = !string.IsNullOrEmpty(serv.shItem.ReasonForPartialClosure) ?
                    (serv.modelItem.Quantity.HasValue ? serv.modelItem.Quantity.Value : 0) :
                    (serv.shItem.Quantity.HasValue ? serv.shItem.Quantity.Value : 0);
                //item.SATAct = act;
                item.ShId = serv.shItem.TOItem;
                item.Site = serv.shItem.Site;
                item.FOL = serv.shItem.FOL;
                var shSite = _cachedShSites.FirstOrDefault(s=>s.Site==item.Site);
                if (shSite != null)
                {
                    item.SiteAddress = shSite.Address;
                }
                else
                {
                    var shFOL = Context.ShFOLs.FirstOrDefault(f => f.FOL == item.FOL);
                    if (shFOL != null)
                    {
                        item.SiteAddress = string.Format("{0}-{1}", shFOL.StartPoint, shFOL.DestinationPoint);
                    }
                }


                //context.SATActItems.Add(item);
                services.Add(item);
            }
            return services;
        }
        public List<SATActMaterial> GetSATMaterials(string to, List<ActItemModel> materialModels)
        {
            List<SATActMaterial> materials = new List<SATActMaterial>();
            var toItems = GetToItems(to);
            if (toItems.Materials.Count() > 0 && materialModels.Where(m => m.Quantity > 0).Count() > 0)
            {
                var selectedMaterials = materialModels.Where(m => m.Quantity.HasValue && m.Quantity.Value > 0)
                    .GroupJoin(toItems.Materials, m => m.Id, s => s.MatTOId, (mi, si) => new { modItem = mi, shItem = si.FirstOrDefault() });
                //if (selectedServices.Any(s => s.shItem == null))
                //{
                //    result.Success = false;
                //    result.Message = string.Format("Некоторые из выбранных материалов не пренадлежат данному ТО: {0}", model.TO);
                //    return Json(result);
                //}

                foreach (var mat in selectedMaterials)
                {
                    SATActMaterial item = new SATActMaterial();
                    item.Description = mat.shItem.Description;

                    item.Price = mat.shItem.Price.Value;//!!!!!!!!!!!!
                    item.Quantity = mat.modItem.Quantity.Value; 
                    //item.SATAct = act;
                    item.ShId = mat.shItem.MatTOId;
                    item.Site = mat.shItem.SiteId;

                    item.Unit = mat.shItem.Unit;
                    // if (string.IsNullOrEmpty(act.Region))
                    // {

                    var shSite = _cachedShSites.FirstOrDefault(s=>s.Site==item.Site);
                    if (shSite != null)
                    {
                        item.SiteAddress = shSite.Address;
                        //        if (!string.IsNullOrEmpty(item.Site))
                        //        {
                        //            act.Region = shSite.MacroRegion;
                        //            act.Branch = shSite.Branch;

                        //            var network = shTO.Network;//context.PORNetworks.FirstOrDefault(n => n.SiteBranch == act.Branch);
                        //            if (network != null)
                        //            {
                        //                act.Network = network;
                        //            }
                        //        }
                    }
                    //}
                    materials.Add(item);

                }
            }
            return materials;
        }

        #endregion

        public class ActItemModels
        {
            public List<ShTOItem> Services { get; set; }
            public List<ShMatToItem> Materials { get; set; }
        }
        /// <summary>
        /// Метод саздает (сохраняет) акт вы базе данных. 
        /// </summary>
        /// <param name="satAct"></param>
        /// <param name="satServices"></param>
        /// <param name="satMaterials"></param>
        public void CreateAct(SATAct satAct, List<SATActService> satServices, List<SATActMaterial> satMaterials)
        {
            Context.SATActs.Add(satAct);
            if (satServices != null)
                foreach (var service in satServices)
                {
                    service.SATAct = satAct;
                    Context.SATActItems.Add(service);
                }
            if (satMaterials != null)
                foreach (var material in satMaterials)
                {
                    material.SATAct = satAct;
                    Context.SATActItems.Add(material);
                }
            try
            {
                SaveChanges();
                // воизбежании... пришлось поработать с бд.
                foreach (var item in satServices)
                {
                    var toItem = Context.ShTOItems.FirstOrDefault(i=>i.TOItem==item.ShId);
                    if (toItem != null)
                        toItem.ActId = satAct.ActName;
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception("Ошибка при создании акта");
            }

        }
    }
}
