using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpplusInteract;
using DbModels.DataContext;
using System.IO;
using DbModels.DataContext.Repositories;
using CommonFunctions.Extentions;
using CommonFunctions;

namespace ExcelParser.EpplusInteract
{
    public static class CreateAct
    {
        private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\AKT.xlsm";

        public static byte[] CreateActFile(int ActId, bool draft = false)
        {
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {


                ActRepository repository = new ActRepository(context);
                //var _act = repository.GetReadyToPrintActs().FirstOrDefault(t => t.Id == ActId);
                var _act = context.SATActs.Find(ActId);
                if (_act != null)
                {
                    var act = context.SATActs.Find(ActId);
                    if (!act.UploadedToSH)
                        return null;

                    var actServices = repository.GetSATActServices(act);
                    var actMaterials = repository.GetSATActMaterials(act);



                    var _itemSpecTable = actServices.Select((i, ind) => new ItemSpecViewModel()
                    {
                        Description = i.Description,
                        Price = i.Price,
                        PricePerItem = i.Price,
                        Quantity = i.Quantity,
                        Units = string.IsNullOrEmpty(i.Unit) ? "шт" : i.Unit,
                        Id = ind
                    }).ToList();
                    var itemSpecTable = _itemSpecTable.GroupBy(g => g.Description).OrderBy(o => o.Key).Select((i, ind) => new ItemSpecViewModel()
                    {
                        Description = i.FirstOrDefault().Description,
                        Price = i.Sum(p => p.PricePerItem * p.Quantity),
                        PricePerItem = i.FirstOrDefault().PricePerItem,
                        Quantity = i.Sum(q => q.Quantity),
                        Units = i.FirstOrDefault().Units,
                        Id = ind + 1,
                        SId = string.Format("{0}.XXX", ind + 1),
                        Empty = "#merger(1,1)",

                    }).ToList();
                    var itemSpecTableDataTable = itemSpecTable.ToDataTable();
                    itemSpecTableDataTable.Columns.Remove("Id");
                    var itemSpecMatTable = actMaterials.Select((i, ind) => new ItemSpecMatViewModel()
                    {
                        Description = i.Description,
                        Empty = "#merger(1,0)",
                        Price = i.Price * i.Quantity,
                        PricePerItem = i.Price,
                        Quantity = i.Quantity,
                        Units = string.IsNullOrEmpty(i.Unit) ? "шт" : i.Unit,
                        Id = ind + 1,
                        Site = string.Format("{0}", i.Site)
                    }).ToList();

                    var _itemObjectCardTable = actServices;
                    var itemObjectCardTable = _itemObjectCardTable.GroupBy(g => g.Description).OrderBy(f => f.Key).Select((g, gind) => g.Select((i, ind) =>


                        new ItemObjectCardViewModel()
                        {
                            Description = i.Description,
                            Empty = "#merger(1,0)",
                            Price = i.Price,
                            Address = i.SiteAddress,
                            Quantity = i.Quantity,
                            Site = i.Site!=null?i.Site:i.FOL,
                            FactDate = i.FactDate,
                            Id = ind + 1,
                            SId = string.Format("{1}.{0}", (ind + 1).ToString("000"), gind + 1)
                        })).SelectMany(x => x).ToList();
                    var itemCardDataTable = itemObjectCardTable.ToDataTable();
                    itemCardDataTable.Columns.Remove("Id");


                    service.InsertTableToPatternCellInWorkBook("ItemSpecTable", itemSpecTableDataTable, new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                    service.InsertTableToPatternCellInWorkBook("ItemMatSpecTable", itemSpecMatTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                    service.InsertTableToPatternCellInWorkBook("ItemObjectCardTable", itemCardDataTable, new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                    Dictionary<string, string> dict = new Dictionary<string, string>();

                    string servicendsText;
                    string materialndsText;
                    string totalndsText;
                    string cardNDSText;

                    decimal ndskoeff;
                    if (act.WOVAT)
                    {
                        ndskoeff = 0;
                        servicendsText = "без НДС";
                        materialndsText = "без НДС";
                        totalndsText = "без НДС";
                        cardNDSText = "без НДС";
                    }
                    else
                    {
                        ndskoeff = 0.18M;
                        servicendsText = "а кроме того НДС - 18%, что составляет #ServiceTotalNDS# рублей";
                        materialndsText = "а кроме того НДС - 18%, что составляет #MatTotalNDS# рублей";
                        totalndsText = "а кроме того НДС - 18%, что составляет #TotalNDS# рублей";
                        cardNDSText = "кроме того НДС - 18%, что составляет #TotalNDS# рублей";

                    }
                    decimal serviceTotal = actServices.Sum(s => s.Price * s.Quantity);
                    decimal materialTotal = actMaterials.Sum(s => s.Price * s.Quantity);

                    var totalwoNDS = serviceTotal + materialTotal;
                    var nds = totalwoNDS * ndskoeff;
                    var totalWNDS = totalwoNDS + nds;
                    var serviceTotalNDS = actServices.Sum(s => s.Price * s.Quantity) * ndskoeff;
                    var serviceTotalWNDS = serviceTotal + serviceTotalNDS;

                    dict.Add("StartDate", act.StartDate.ToString("dd.MM.yyyy"));
                    dict.Add("EndDate", act.EndDate.ToString("dd.MM.yyyy"));
                    dict.Add("PONumber", act.PONumber);
                    dict.Add("PODate", act.PODate.HasValue ? act.PODate.Value.ToString("dd.MM.yyyy") : "");
                    dict.Add("ActId", act.ActName);
                    dict.Add("NDSText", string.Format("{0:P0}", ndskoeff));
                    dict.Add("ServiceNDSText", servicendsText);
                    dict.Add("MaterialNDSText", materialndsText);
                    dict.Add("TotalNDSText", totalndsText);
                    dict.Add("CardNDSText", cardNDSText);
                    dict.Add("TO", act.TO);
                    dict.Add("ServiceTotalWONDS", serviceTotal.ToString("F"));
                    dict.Add("ServiceTotalNDS", serviceTotalNDS.ToString("F"));
                    dict.Add("ServiceTotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(serviceTotal, CommonFunctions.InWords.Заглавные.Первая));
                    dict.Add("ServiceTotalWNDS", serviceTotalWNDS.ToString("F"));

                    var matTotalNDS = materialTotal * ndskoeff;
                    var matTotalWNDS = materialTotal + matTotalNDS;
                    dict.Add("MatTotalWONDS", materialTotal.ToString("F"));
                    dict.Add("MatTotalNDS", matTotalNDS.ToString("F"));
                    dict.Add("MatTotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(materialTotal, CommonFunctions.InWords.Заглавные.Первая));
                    dict.Add("MatTotalWNDS", matTotalWNDS.ToString("F"));

                    dict.Add("TotalWONDS", totalwoNDS.ToString("F"));
                    dict.Add("TotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(totalwoNDS, CommonFunctions.InWords.Заглавные.Первая));
                    dict.Add("TotalNDS", nds.ToString("F"));
                    dict.Add("TotalWNDS", totalWNDS.ToString("F"));
                    dict.Add("DogNum", act.NomerDogovora);
                    var shTO = context.ShTOes.Find(act.TO);
                    if (shTO != null)
                        dict.Add("WorkDescription", shTO.WorkDescription);
                    dict.Add("DogDate", act.DataDogovora.HasValue ? act.DataDogovora.Value.ToString("dd.MM.yyyy") : "");
                    dict.Add("Subcontractor", act.SubContractor);
                    // dict.Add("porFile", string.IsNullOrEmpty(porFileName) ? "" : porFileName);
                    var shSubcontractor = context.SubContractors.FirstOrDefault(s => s.Name == act.SubContractor || s.ShName == act.SubContractor);
                    if (shSubcontractor != null)
                    {

                        var shContact = context.ShContacts.FirstOrDefault(s => s.Contact == shSubcontractor.ShName);
                        if (shContact != null && !string.IsNullOrWhiteSpace(shContact.SubcFace))
                        {
                            dict.Add("SubcFace", shContact.SubcFace);
                        }
                        else
                        {
                            dict.Add("SubcFace", @"""please fill in SH""");
                        }
                    }
                    var _firstItem = _itemObjectCardTable.FirstOrDefault();
                    // нефига печатать пор, если там трабл с сервис айтемами. А то он еще отправится автоматом...
                    //if (_firstItem != null)
                    //{
                    var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == _firstItem.Site);
                    var shFOL = context.ShFOLs.FirstOrDefault(s => s.FOL == _firstItem.FOL);
                    if (shSite != null)
                    {
                        dict.Add("SiteBranch", shSite.Branch);
                    }
                    else
                    {
                        if (shFOL != null)
                        {
                            dict.Add("SiteBranch", shFOL.Branch);
                        }
                        else
                            throw new Exception($"Позиция {_firstItem.Id} не привязана ни к сайту ни к фолу");
                    }
                





                service.ReplaceDataInBook(dict, true);
                service.CellsMerger();

                var stream = new MemoryStream();
                if (draft)
                    if (!service.Draft())
                    {
                        // если не удалось вставить драфт, то ничего не отправляем
                        return null;
                    }
                service.app.SaveAs(stream);
                return StaticHelpers.ReadToEnd(stream);
            }
        }
           return null;
       }

    public class ItemSpecViewModel
    {
        public int Id { get; set; }
        public string SId { get; set; }
        public string Description { get; set; }
        public string Empty { get; set; }
        public string Empty1 { get; set; }
        public decimal Quantity { get; set; }
        public string Units { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal Price { get; set; }

    }

    public class ItemSpecMatViewModel
    {
        public int Id { get; set; }
        public string Site { get; set; }
        public string Description { get; set; }
        public string Empty { get; set; }
        public decimal Quantity { get; set; }
        public string Units { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal Price { get; set; }

    }

    public class ItemObjectCardViewModel
    {
        public int Id { get; set; }
        public string SId { get; set; }
        public string Site { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Empty { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? FactDate { get; set; }
        public decimal Price { get; set; }
    }

}
}
