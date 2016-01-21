//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using DbModels.DataContext;
//using DbModels.DomainModels.ShClone;
//using DbModels.DomainModels.Base;

//namespace TaskManager.Service
//{
//    public static class SHWOService
//    {
//        public const string WoTypeSupplement = "Supplement";
//        public const string SowConfirmedYes = "Yes";
//        public const string EcrAddS = "ECR_ADD_S";
//        public const string EcrAddM = "ECR_ADD_M";
//        public const string EcrAmend = "ECR_Amend";
//        public const string Materials = "Materials";
//        public const decimal Vat = 0.18m;
//        public const string ServicesRus = "Услуги";
//        public const string MaterialsRus = "Материалы";
//        public const int PriceRoundQuantity = 2;

//        #region Основные функции
//        public static ShNode GetSiteNode(ShSite site, Context context)
//        {
//            return context.ShNodes.FirstOrDefault(n => n.Site == site.Site);
//        }
//        /// <summary>
//        /// Проверка заказа на тип Саплемент
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <returns></returns>
//        public static bool IsSupplement(this ShWO wo)
//        {
//            return wo.WoType == WoTypeSupplement;
//        }
//        /// <summary>
//        /// Проверка заказа на наличие в БД
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static bool WoExists(string WO, Context context)
//        {
//            if (context.ShWOs.FirstOrDefault(w => w.WO == WO) == null)
//            {
//                return false;
//            }
//            return true;
//        }
//        /// <summary>
//        /// Получение экземпляра заказа
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static ShWO GetWO(string WO, Context context)
//        {
//            return context.ShWOs.FirstOrDefault(w => w.WO == WO);
//        }
//        /// <summary>
//        /// Получение списка позиций
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static List<ShWOItem> GetWoItems(this ShWO wo, Context context)
//        {
//            return context.ShWOItems.Where(w => w.WO == wo.WO).ToList();
//        }
//        /// <summary>
//        /// Получения экземпляра основного заказа
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static ShWO GetParentWo(ShWO wo, Context context)
//        {
//            var child = GetWO(wo.WO, context);
//            if (child != null)
//            {
//                return context.ShWOs.FirstOrDefault(w => w.WO == child.ParentWO);
//            }
//            return null;

//        }
//        public static ShWO GetFreezedParent(this ShWO wo, Context context)
//        {
//            ShWO parent = GetParentWo(wo, context);
//            if (parent != null && parent.SowConfirmed == SowConfirmedYes)
//            {
//                return parent;
//            }
//            return null;
//        }
//        /// <summary>
//        /// Получение списка всех связанных заказов. 
//        /// С учетом заказа, относительно которого осуществляется поиск
//        /// В рекурсии нет необходимости, так как у нас нет иерархически связанных заказов
//        /// </summary>
//        /// <param name="WO">Неосновной заказ</param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static List<ShWO> GetParentWos(ShWO wo, Context context, bool appendMainWoToResultList = true)
//        {
//            List<ShWO> WoList = new List<ShWO>();
//            ShWO parent = GetFreezedParent(wo, context);
//            if (parent != null)
//            {
//                //Выбираем только те заказы, у которых WO SOW Confirmed = Yes
//                var ChildWoList = context.ShWOs.Where(w => w.ParentWO == parent.WO && w.SowConfirmed == SowConfirmedYes).ToList();
//                foreach (var ParentWo in ChildWoList)
//                {
//                    WoList.Add(ParentWo);
//                }
//            }
//            if (appendMainWoToResultList)
//            {
//                WoList.Add(wo);
//            }
//            return WoList;
//        }
//        /// <summary>
//        /// Получение обьекта сайт для обычного заказа
//        /// Выбирает первый WBS, потом первый элемент и только после сайт.
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static ShSite GetWoSite(ShWO wo, Context context)
//        {
//            ShWBS wbs = context.ShWBSs.FirstOrDefault(w => w.WBS == wo.WBS);
//            if (wbs != null)
//            {
//                ShElement element = context.ShElements.FirstOrDefault(e => e.Element == wbs.Element);
//                if (element != null)
//                {
//                    return context.ShSites.FirstOrDefault(s => s.Element == element.Element);
//                }
//            }
//            return null;
//        }
//        /// <summary>
//        /// Получение обьекта элемент для обычного заказа
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static ShElement GetWoElement(ShWO wo, Context context)
//        {
//            ShWBS wbs = context.ShWBSs.FirstOrDefault(w => w.WBS == wo.WBS);
//            if (wbs != null)
//            {
//                return context.ShElements.FirstOrDefault(e => e.Element == wbs.Element);
//            }
//            return null;
//        }


//        #endregion

//        #region Все, что касается позиций
//        /// <summary>
//        /// Проверка наличия позиций у заказа, у которых поле ExcludeFromTimeplan false
//        /// Это условие перекрывает следующее условие:
//        /// У заказа должны быть позиции , чтобы его напечатать
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static bool CheckWOPositions(ShWO wo, Context context)
//        {
//            List<ShWOItem> woItemList = context.ShWOItems
//                        .Where(wi => wi.WO == wo.WO && wi.ExcludeFromTimePlan == false).ToList();

//            if (woItemList.Count > 0)
//            {
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Проверка позиций Ecr Add
//        /// </summary>
//        /// <param name="WO"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static bool CheckEcrAddPositions(ShWO wo, Context context)
//        {
//            List<ShWOItem> woItemList = context.ShWOItems
//                        .Where(wi => wi.WO == wo.WO)
//                        .Where(wi => wi.Code == EcrAddS || wi.Code == EcrAddM || wi.Code == EcrAmend)
//                        .Where(wi => wi.Price == null || wi.Unit == null || wi.Description == null)
//                        .ToList();
//            if (woItemList.Count > 0)
//            {
//                return false;
//            }
//            return true;
//        }
//        /// <summary>
//        /// Расширяющий метод, позволяющий понять, является ли позиция Ecr
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public static bool IsEcrAddS(this ShWOItem item)
//        {
//            return item.Code == EcrAddS || item.Code == EcrAddM || item.Code == EcrAmend ? true : false;
//        }
//        public static bool IsEcrAddS(this ShSOWItem item)
//        {
//            return item.Code == EcrAddS || item.Code == EcrAddM || item.Code == EcrAmend ? true : false;
//        }
//        public static bool IsMultiple(this ShPORItem item)
//        {
//            return item.SOWItemCode == EcrAddS || item.SOWItemCode == EcrAddM || item.SOWItemCode == EcrAmend ? true : false;
//        }

//        public static bool IsMaterial(this ShWOItem item)
//        {
//            return item.Service == Materials;
//        }
//        public static bool IsMaterial(this ShSOWItem item)
//        {
//            return item.Service == Materials;
//        }

//        public static decimal? RegionPrice(this ShWOItem item, string region, Context context)
//        {
//            var regionPrice = context.RegionPrices.FirstOrDefault(rp => rp.Region.NameRus == region && rp.MasterData.Code == item.Code);
//            if (regionPrice != null)
//            {
//                return regionPrice.Price;
//            }
//            else
//            {
//                return null;
//            }
//        }
//        /// <summary>
//        /// Получение цены позиции
//        /// </summary>
//        /// <param name="item">Обьект SHWoItem</param>
//        /// <param name="regionName"> регион для заказа</param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static decimal GetItemPrice(this ShWOItem item, string regionName, Context context)
//        {
//            //Проверяем , есть ли у позиции запись в таблице RegionPrices
//            //Если да, то возвращаем тупо эту цену(там сразу должны быть учтены всякие коэффициенты)
//            decimal? regionPrice = item.RegionPrice(regionName, context);
//            if (regionPrice != null)
//            {
//                return (decimal)regionPrice;
//            }
//            //Если позиция ECr_Add
//            if (item.IsEcrAddS())
//            {
//                //Если это ECR_ADD_M
//                if (item.IsMaterial())
//                {
//                    Region region = context.Regions.FirstOrDefault(r => r.NameRus == regionName);
//                    if (region == null)
//                    {
//                        throw new NullReferenceException(string.Format("Регион {0} для позиции материалов не найден в таблице Regions!", regionName));
//                    }
//                    if (!region.MaterialKoeff.HasValue)
//                    {
//                        throw new NullReferenceException(string.Format("В таблице Regions для региона {0} не указан коэффициент для материалов", regionName));
//                    }
//                    return Math.Round(item.Price.Value * region.MaterialKoeff.Value, PriceRoundQuantity);
//                }
//                else
//                {
//                    return Math.Round(item.Price.Value, PriceRoundQuantity);
//                }
//            }
//            else
//            {
//                //Для обычных позиций цены берутся из таблицы MastarData
//                MasterDataRow masterData = context.MasterData.FirstOrDefault(m => m.Code == item.Code);
//                if (masterData == null)
//                {
//                    throw new NullReferenceException(string.Format("Код позиции {0} не найден в таблице MasterData", item.Code));
//                }
//                //Если это материалы
//                if (item.IsMaterial())
//                {
//                    Region region = context.Regions.FirstOrDefault(r => r.NameRus == regionName);
//                    if (region == null)
//                    {
//                        throw new NullReferenceException(string.Format("Регион {0} для позиции материалов не найден в таблице Regions!", regionName));
//                    }
//                    if (!region.MaterialKoeff.HasValue)
//                    {
//                        throw new NullReferenceException(string.Format("В таблице Regions для региона {0} не указан коэффициент для материалов", regionName));
//                    }
//                    return Math.Round(masterData.Price * region.MaterialKoeff.Value, PriceRoundQuantity);
//                }
//                else
//                {
//                    return Math.Round(masterData.Price, PriceRoundQuantity);
//                }
//            }
//        }
//        /// <summary>
//        /// Произведение цены позиции(с учетом коэф-тов для материалов) и количества
//        /// </summary>
//        /// <param name="item"></param>
//        /// <param name="regionName"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static decimal GetItemTotalPrice(this ShWOItem item, string regionName, Context context)
//        {
//            if (!item.Quantity.HasValue)
//            {
//                throw new NullReferenceException(string.Format("Для позиции {0} не указано количество", item.WOItem));
//            }
//            return Math.Round(item.GetItemPrice(regionName, context) * item.Quantity.Value, PriceRoundQuantity);
//        }
//        /// <summary>
//        /// Получение Ват для конкретного заказа
//        /// </summary>
//        /// <param name="price"></param>
//        /// <returns></returns>
//        public static decimal GetWOTotalPriceVat(ShWO wo, Context context)
//        {
//            return Math.Round(GetSoWList(wo, context).Sum(w => w.TotalPrice) * Vat, PriceRoundQuantity);
//        }
//        /// <summary>
//        /// Полная сумма конкретного заказа(без учета других заказов и с учетом Ват)
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static decimal GetWOTotalPriceWithVat(ShWO wo, Context context)
//        {
//            return Math.Round(GetSoWList(wo, context).Sum(w => w.TotalPrice) * (1 + Vat), PriceRoundQuantity);
//        }
//        /// <summary>
//        /// Сумма конкретного заказа(без учета других заказов и без учета Ват)
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static decimal GetWOTotalPrice(ShWO wo, Context context)
//        {
//            return Math.Round(GetSoWList(wo, context).Sum(w => w.TotalPrice), PriceRoundQuantity);
//        }
//        /// <summary>
//        /// Полная сумма конкретного заказа(с учетом других заказов и с учетом Ват)
//        /// </summary>
//        public static decimal GetAllWOTotalPriceWithVat(ShWO wo, decimal price, Context context)
//        {
//            var woList = GetParentWos(wo, context, false);
//            decimal SubPrice = Math.Round(woList.Sum(w => w.WoTotal).Value * (1 + Vat), PriceRoundQuantity);
//            return Math.Round(SubPrice + price, PriceRoundQuantity);
//        }
//        /// <summary>
//        /// Получение Ват для всех связанных заказов
//        /// </summary>
//        public static decimal GetAllWOTotalPriceVat(ShWO wo, decimal vat, Context context)
//        {
//            var woList = GetParentWos(wo, context, false);
//            decimal SubPrice = woList.Sum(w => w.WoTotal).Value;
//            return Math.Round((SubPrice / Vat) + vat, PriceRoundQuantity);
//        }

//        /// <summary>
//        /// Формирование списка позиции для заполнения таблицы
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static List<DefaultWorkItem> GetSoWList(ShWO wo, Context context)
//        {
//            List<DefaultWorkItem> SoWList = new List<DefaultWorkItem>();
//            List<ShWOItem> ItemList = context.ShWOItems.Where(i => i.WO == wo.WO).ToList();
//            ShElement element = GetWoElement(wo, context);
//            if (element != null)
//            {

//                foreach (var item in ItemList)
//                {
//                    if (!item.Quantity.HasValue)
//                    {
//                        throw new NullReferenceException(string.Format("Заказ - {0};Для позиции {1} не указано количество", wo.WO, item.WOItem));
//                    }
//                    if (item.IsEcrAddS())
//                    {
//                        SoWList.Add(new DefaultWorkItem
//                        {
//                            Code = item.Code,
//                            Description = item.Description,
//                            Price = item.GetItemPrice(element.RegionRus, context),
//                            Unit = item.Unit,
//                            Quantity = item.Quantity.Value,
//                            TotalPrice = item.GetItemTotalPrice(element.RegionRus, context),
//                            Type = item.IsMaterial() ? MaterialsRus : ServicesRus
//                        });
//                    }
//                    else
//                    {
//                        MasterDataRow masterData = context.MasterData.FirstOrDefault(d => d.Code == item.Code);
//                        if (masterData == null)
//                        {
//                            throw new NullReferenceException(string.Format("Заказ - {0};Код позиции {1} не найден в таблице MasterData", wo.WO, item.Code));
//                        }
//                        SoWList.Add(new DefaultWorkItem
//                        {
//                            Code = item.Code,
//                            Description = masterData.Description,
//                            Price = item.GetItemPrice(element.RegionRus, context),
//                            Unit = masterData.Unit,
//                            Quantity = item.Quantity.Value,
//                            TotalPrice = item.GetItemTotalPrice(element.RegionRus, context),
//                            Type = masterData.SowData.Type
//                        });
//                    }

//                }
//            }
//            return SoWList;
//        }
//        /// <summary>
//        /// Получение списка групп позициий для печати обычного заказа
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static List<SoWGroup> GetSoWGroupList(ShWO wo, Context context)
//        {
//            List<SoWGroup> ResultList = new List<SoWGroup>();
//            //Набираем список всех позиций в заказе(с заполненными необходимыми полями
//            var SoWList = GetSoWList(wo, context);
//            //Бежим по системной таблице PrintOutSowDatа, содержащей типы групп позиций
//            int i = 1;
//            foreach (var row in context.PrintOutSowData)
//            {
//                int j = 1;
//                var groupSoWList = SoWList.Where(s => s.Type == row.Type).ToList();
//                foreach (var item in groupSoWList)
//                {
//                    item.Position = string.Format("{0}.{1}", i, j);
//                    j++;
//                }
//                //Если в полном списке есть позиции определенной группы, добавляем их в итоговой набор
//                if (groupSoWList.Count() > 0)
//                {
//                    ResultList.Add(new SoWGroup
//                    {
//                        Position = i,
//                        Name = row.Name,
//                        Total = row.Total,
//                        SoWList = groupSoWList,
//                        TotalPrice = Math.Round(groupSoWList.Sum(s => s.TotalPrice), 2)
//                    });
//                    i++;
//                }
//            }
//            return ResultList;

//        }

//        #endregion

//        #region Формирование план-графика

//        /// <summary>
//        /// Проверка позиции на присутсвие необходимых дат начала и окончания работ
//        /// </summary>
//        /// <param name="woItem"></param>
//        /// <returns></returns>
//        public static bool CheckItemDiruation(ShWOItem woItem)
//        {
//            if (woItem.WorkStart.HasValue || woItem.WorkEnd.HasValue)
//            {
//                return true;
//            }
//            else
//            {
//                //Ошибка - не заполнены даты начала и окончания работ
//                return false;
//            }
//        }
//        /// <summary>
//        /// Получение длительности конкретной позиции
//        /// </summary>
//        /// <param name="woItem"></param>
//        /// <returns></returns>
//        public static int GetItemDiruation(ShWOItem woItem)
//        {
//            if (CheckItemDiruation(woItem))
//            {
//                //Минимальная продолжительность - 1 день
//                int result = (int)(woItem.WorkEnd.Value - woItem.WorkStart.Value).TotalDays;
//                if (result < 1)
//                {
//                    return 1;
//                }
//                return result;

//            }
//            else
//            {
//                return 0;
//            }
//        }
//        /// <summary>
//        /// Определение начальной даты для план графика
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static DateTime GetWOStartDate(ShWO wo, Context context)
//        {

//            ShWO Parent = GetFreezedParent(wo, context);
//            if (Parent != null)
//            {
//                wo = Parent;
//                //Если основной заказ подписан, то датой начала тайм плана является дата подписания основного заказа.
//                if (wo.SignedByMTS.HasValue)
//                {
//                    return wo.SignedByMTS.Value;
//                }
//            }

//            return wo.GetWoItems(context).Min(i => i.WorkStart).Value;
//        }
//        public static TimePlan GetTimePlan(ShWO wo, Context context)
//        {
//            TimePlan timePlan = new TimePlan();
//            //Выбираем связанные заказы с WO SOW Confirmed  = Yes
//            var WoList = GetParentWos(wo, context);
//            timePlan.StartDate = GetWOStartDate(wo, context);
//            foreach (var parentWo in WoList)
//            {
//                //Начальные данные
//                //Дата начала выбирается, как дата начала всего графика
//                DateTime TempStartDate = timePlan.StartDate;
//                //Дата окончания выбирается равной дате начала минус один день
//                //Это сделано для того, чтобы корректно выбрать первую дату начала в цикле
//                DateTime TempEndDate = TempStartDate.AddDays(-1);
//                //Выбираются только те позиции, где не стоит галочка Exclude from TimePlan
//                foreach (var item in parentWo.GetWoItems(context).Where(i => i.ExcludeFromTimePlan == false))
//                {
//                    int itemDuriation = GetItemDiruation(item);
//                    TempStartDate = TempEndDate.AddDays(1);
//                    TempEndDate = TempStartDate.AddDays(itemDuriation);
//                    TimePlanRow row = new TimePlanRow
//                    {
//                        Code = item.Code,
//                        Diruation = itemDuriation,
//                        StartDate = TempStartDate,
//                        EndDate = TempEndDate
//                    };
//                    timePlan.Rows.Add(row);
//                }
//            }
//            timePlan.EndDate = timePlan.StartDate.AddDays(timePlan.Rows.Sum(r => r.Diruation));
//            return timePlan;
//        }

//        #endregion

//    }

//    /// <summary>
//    /// Класс для заполнения Графика
//    /// </summary>
//    public class TimePlan
//    {
//        public TimePlan()
//        {
//            Rows = new List<TimePlanRow>();
//        }
//        public DateTime StartDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public List<TimePlanRow> Rows { get; set; }
//    }
//    /// <summary>
//    /// Класс, представляющий одну строчку в тайм плане
//    /// </summary>
//    public class TimePlanRow
//    {
//        public string Position { get; set; }
//        public string Code { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public int Diruation { get; set; }
//    }
//    /// <summary>
//    /// Представление позиции для обычных заказов(TS,SC,Meterials)
//    /// </summary>
//    public class DefaultWorkItem
//    {
//        public string Position { get; set; }
//        public string Code { get; set; }
//        public decimal Price { get; set; }
//        public decimal TotalPrice { get; set; }
//        public string Unit { get; set; }
//        public decimal Quantity { get; set; }
//        public string Description { get; set; }
//        public string Type { get; set; }
//    }
//    /// <summary>
//    /// Класс, представляющий собой одну группу позиций в табличке заказа
//    /// </summary>
//    public class SoWGroup
//    {
//        public string Name { get; set; }
//        public string Total { get; set; }
//        public decimal TotalPrice { get; set; }
//        public int Position { get; set; }
//        public List<DefaultWorkItem> SoWList { get; set; }
//    }
//}
