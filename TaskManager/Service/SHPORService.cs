using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext;

//using TaskManager.Handlers.TaskHandlers.Models.POR;

namespace TaskManager.Service
{
    public class StoredProcedureResultObject
    {
        public int ObjectId { get; set; }
    }
    public class PriceProc
    {
        public double Price { get; set; }
    }
    /// <summary>
    /// Класс служит для двух типов хендлеров: загрузки ПОРов в СХ и отправки в ОД
    /// </summary>
    //public static class SHPORService
    //{
    //    #region Загрузка в СХ

    //    /// <summary>
    //    /// Расширяющий метод, показывающий, является ли дата праздником или воскресением
    //    /// </summary>
    //    /// <param name="date"></param>
    //    /// <param name="context"></param>
    //    /// <returns></returns>
    //    public static bool IsHolidayOrSunday(this DateTime date, Context context)
    //    {
    //        if (date.DayOfWeek == DayOfWeek.Sunday || context.Holidays.FirstOrDefault(d => d.Date == date) != null)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //    public static DateTime GetPriceDate(DateTime TestDate, Context context)
    //    {
    //        //Отнимаем от мин дата начала работ по ПОРу 7 дней
    //        DateTime PriceDate = TestDate.AddDays(-7);
    //        //Если этот день выходной или воскресение, отнимаем один день и опять проверям.
    //        while (PriceDate.IsHolidayOrSunday(context))
    //        {
    //            PriceDate = PriceDate.AddDays(-1);
    //        }
    //        //В конце концов получаем день, не являющийся ни воскрсением, ни праздником
    //        //Если он в будущем, возвращаем сегодняшний день
    //        if (PriceDate > DateTime.Now)
    //        {
    //            return DateTime.Now;
    //        }
    //        return PriceDate;
    //    }
    //    public static string GetVendorNumber(string VendorName)
    //    {
    //        Dictionary<string, object> vendorDict = new Dictionary<string, object>();
    //        vendorDict.Add("@Vendor", VendorName);
    //        return CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcScalarValueFromServer("GetVendorNumber", vendorDict);
    //    }
    //    /// <summary>
    //    /// Получение цены за позицию с учетом прайса
    //    /// </summary>
    //    /// <param name="SAPCode"></param>
    //    /// <param name="context"></param>
    //    /// <param name="Subregion"></param>
    //    /// <param name="MacroRegion"></param>
    //    /// <param name="date"></param>
    //    /// <param name="ProjectId"></param>
    //    /// <param name="VendorId"></param>
    //    /// <returns></returns>
    //    public static decimal GetPrice(string Code, string Subregion, int MacroRegionId, DateTime PriceDate, int ProjectId, int SubcId)
    //    {
    //        Dictionary<string, object> dict = new Dictionary<string, object>();
    //        dict.Add("@VendorID", SubcId);
    //        dict.Add("@PositionCode", Code);
    //        dict.Add("@MacroRegion", MacroRegionId);
    //        dict.Add("@SubRegion", Subregion);
    //        dict.Add("@PriceDate", PriceDate);
    //        dict.Add("@ProjectId", ProjectId);
    //        decimal price = 0;
    //        decimal.TryParse(CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcScalarValueFromServer("GetActivePriceCoeff", dict), out price);
    //        return price;
    //    }
    //    public static int GetObjectIdByName(string ProcName, string ParamName, string ParamValue)
    //    {
    //        Dictionary<string, object> dict = new Dictionary<string, object>();
    //        dict.Add(ParamName, ParamValue);
    //        int id = 0;
    //        int.TryParse(CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcScalarValueFromServer(ProcName, dict), out id);
    //        return id;

    //    }
    //    /// <summary>
    //    /// Расширяющий метод пытается получить из обьекта SHSowItem соответствующий ему обьект SOWCode
    //    /// </summary>
    //    /// <param name="SOWItem"></param>
    //    /// <param name="context"></param>
    //    /// <returns></returns>
    //    public static SOWCode GetSOWCode(this ShSOWItem SOWItem, Context context)
    //    {
    //        return context.SOWCodes.FirstOrDefault(s => s.Code == SOWItem.Code);
    //    }
    //    /// <summary>
    //    /// Расширяющий метод инкапсулирует логику получения набора сап кодов из SOW кода
    //    /// </summary>
    //    /// <param name="code"></param>
    //    /// <param name="context"></param>
    //    /// <returns>Список сап кодов</returns>
    //    public static List<SOWMappingRow> GetSAPCodes(this SOWCode code, Context context)
    //    {
    //        return context.SOWMapping.Where(s => s.SOWCode.Code == code.Code).ToList();
    //    }
    //    /// <summary>
    //    /// Метод для проверки SOW позиций при загрузке в СХ
    //    /// Включает в себя:
    //    /// 1) Наличие позиции в таблице SOWCodes
    //    /// 2) Пристутствие данной позиции в маппинге
    //    /// </summary>
    //    /// <param name="SOW"></param>
    //    /// <param name="context"></param>
    //    /// <returns>Списки позиций с ошибками</returns>
    //    public static string CheckSOWItems(ShSOW SOW, Context context)
    //    {
    //        //Итоговая строка
    //        string result = string.Empty;
    //        //Подстрока с позициями, отсутствующими в SOWCodes
    //        string NotInSOWTableResult = string.Empty;
    //        //Подстрока с позициями без дат Work Start и Work End
    //        string EmptyDatesResult = string.Empty;
    //        //Подстрока с некорректно заполненным позициями ECR ADD
    //        string ECRErrorsResult = string.Empty;
    //        List<ShSOWItem> list = GetSOWItemsWithSubc(SOW, context);
    //        //Бежим по всем позициям
    //        foreach (var item in list)
    //        {
    //            //Логика для ECR Add и обычных позиций 
    //            if (!item.IsEcrAddS())
    //            {
    //                //У позиции должны быть даты начала и окончания работ и количество
    //                if (!item.WorkStartDate.HasValue || !item.WorkEndDate.HasValue || !item.Quantity.HasValue)
    //                {
    //                    EmptyDatesResult += item.Code + ",";
    //                }
    //                //Получаем обьект SOW
    //                SOWCode sowCode = item.GetSOWCode(context);
    //                if (sowCode == null)
    //                {
    //                    NotInSOWTableResult += item.Code + ",";
    //                }
    //            }
    //            else
    //            {
    //                if (item.Price == null || item.Currency == null || !item.Quantity.HasValue || string.IsNullOrEmpty(item.Description) || string.IsNullOrEmpty(item.Unit))
    //                {
    //                    ECRErrorsResult += item.Code + ",";
    //                }
    //            }
    //        }
    //        if (!string.IsNullOrEmpty(NotInSOWTableResult))
    //        {
    //            result = string.Format("В таблице SOWCodes нет позиций: {0}.", NotInSOWTableResult.Trim(new char[] { ',' }));
    //        }
    //        if (!string.IsNullOrEmpty(EmptyDatesResult))
    //        {
    //            result += string.Format("У следующих позиций не проставлены дата начала, окончания работ или количество: {0}.", EmptyDatesResult.Trim(new char[] { ',' }));
    //        }
    //        if (!string.IsNullOrEmpty(ECRErrorsResult))
    //        {
    //            result += string.Format("Следующие позиции ECR ADD некорректно заполнены: {0}.", ECRErrorsResult.Trim(new char[] { ',' }));
    //        }
    //        return result;
    //    }
    //    public static List<ShSOWItem> GetSOWItems(ShSOW SOW, Context context)
    //    {
    //        return context.ShSOWItems.Where(i => i.SOW_ID == SOW.SOW).ToList();
    //    }
    //    public static List<ShSOWItem> GetSOWItemsWithSubc(ShSOW SOW, Context context)
    //    {
    //        return GetSOWItems(SOW,context).Where(i => i.Subcontractor != null).ToList();
    //    }
    //    public static ShSOW GetSOW(string SOW, Context context)
    //    {
    //        return context.ShSOWs.FirstOrDefault(s => s.SOW == SOW);
    //    }
    //    public static string GetDateMask()
    //    {
    //        return DateTime.Now.ToString("yyMMddHHmmff");
    //    }
    //    public static string GetDateMask(int subcCount)
    //    {
    //        return DateTime.Now.ToString("yyMMddHHmmf" + subcCount);
    //    }
    //    public static ShWBS GetSOWWBS(ShSOW SOW, Context context)
    //    {
    //        return context.ShWBSs.FirstOrDefault(w => w.WBS == SOW.WBS_Id);
    //    }
    //    public static ShElement GetSOWElement(ShSOW SOW, Context context)
    //    {
    //        ShWBS wbs = GetSOWWBS(SOW, context);
    //        if (wbs != null)
    //        {
    //            return context.ShElements.FirstOrDefault(e => e.Element == wbs.Element);
    //        }
    //        return null;
    //    }
    //    public static ShSite GetSOWSite(ShSOW SOW, Context context)
    //    {
    //        ShElement element = GetSOWElement(SOW, context);
    //        if (element != null)
    //        {
    //            return context.ShSites.FirstOrDefault(s => s.Element == element.Element);
    //        }
    //        return null;
    //    }
    //    #endregion
    //    #region Отправка ПОР-ов в ОД
    //    /// <summary>
    //    /// Проверка ПОРа на позиции , которые должен апрувить Иван Морозов
    //    /// </summary>
    //    /// <param name="POR"></param>
    //    /// <param name="context"></param>
    //    /// <returns></returns>
    //    public static bool MultiplePositionsCorrect(this ShPOR POR, Context context)
    //    {
    //        return POR.GetPORItems(context).Where(p => p.IsMultiple() && p.PriceConfirmation != "Yes").Count() > 0 ? false : true ;

    //    }
    //    /// <summary>
    //    /// Получение списка позиции для Пора
    //    /// </summary>
    //    /// <param name="POR"></param>
    //    /// <param name="context"></param>
    //    /// <returns></returns>
    //    public static List<ShPORItem> GetPORItems(this ShPOR POR, Context context)
    //    {
    //        return context.ShPORItems.Where(p => p.POR_Id == POR.POR).ToList();
    //    }
    //    /// <summary>
    //    /// Получение обьекта ПОР по имени
    //    /// </summary>
    //    /// <param name="PORName"></param>
    //    /// <param name="context"></param>
    //    /// <returns></returns>
    //    public static ShPOR GetPOR(string PORName, Context context)
    //    {
    //        return context.ShPORs.FirstOrDefault(p=>p.POR == PORName);
    //    }
    //    #endregion
    //}
}
