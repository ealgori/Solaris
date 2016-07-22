using DbModels.DomainModels.SAT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFunctions.Extentions;
using EpplusInteract;
using CommonFunctions;

namespace ExcelParser.ExcelParser.TOAct
{
    class OldFormatActGen : IActGenerator
    {
        public string TemplatePath { get; set; }
        
        public SATAct SatAct { get; set; }
        public string WorkDescription { get; private set; }
        public string SubcFace { get; private set; }
        public string Branch { get; private set; }

        public OldFormatActGen(string templatePath,SATAct satAct, string workDescription, string subcFace, string branch)
        {
            this.TemplatePath = templatePath;
            this.SatAct = satAct;
            this.WorkDescription = workDescription;
            this.SubcFace = subcFace;
            this.Branch = branch;

        }
        public Tuple<string,Stream> Generate(List<SATActService> actServices, List<SATActMaterial> actMaterials)
        {
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
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
                    Site = i.Site != null ? i.Site : i.FOL,
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
            if (SatAct.WOVAT)
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
            decimal serviceTotal = actServices.Sum(s => s.Price.FinanceRound() * s.Quantity.FinanceRound());
            decimal materialTotal = actMaterials.Sum(s => s.Price.FinanceRound() * s.Quantity.FinanceRound());

            var totalwoNDS = serviceTotal + materialTotal;
            var nds = (totalwoNDS * ndskoeff).FinanceRound();
            var totalWNDS = (totalwoNDS + nds).FinanceRound();
            var serviceTotalNDS = (serviceTotal * ndskoeff).FinanceRound();
            var serviceTotalWNDS = (serviceTotal + serviceTotalNDS).FinanceRound();

            dict.Add("StartDate", SatAct.StartDate.ToString("dd.MM.yyyy"));
            dict.Add("EndDate", SatAct.EndDate.ToString("dd.MM.yyyy"));
            dict.Add("PONumber", SatAct.PONumber);
            dict.Add("PODate", SatAct.PODate.HasValue ? SatAct.PODate.Value.ToString("dd.MM.yyyy") : "");
            dict.Add("ActId", SatAct.ActName);
            dict.Add("NDSText",  ndskoeff.ToString("F"));
            dict.Add("ServiceNDSText", servicendsText);
            dict.Add("MaterialNDSText", materialndsText);
            dict.Add("TotalNDSText", totalndsText);
            dict.Add("CardNDSText", cardNDSText);
            dict.Add("TO", SatAct.TO);
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
            dict.Add("DogNum", SatAct.NomerDogovora);
            dict.Add("WorkDescription", WorkDescription);
            dict.Add("DogDate", SatAct.DataDogovora.HasValue ? SatAct.DataDogovora.Value.ToString("dd.MM.yyyy") : "");
            dict.Add("Subcontractor", SatAct.SubContractor);
            dict.Add("SubcFace", SubcFace);
               
            var _firstItem = _itemObjectCardTable.FirstOrDefault();
            dict.Add("SiteBranch", Branch);
           






            service.ReplaceDataInBook(dict, true);
            service.CellsMerger();

            var stream = new MemoryStream();
            //if (draft)
            //    if (!service.Draft())
            //    {
            //        // если не удалось вставить драфт, то ничего не отправляем
            //        return null;
            //    }
            service.app.SaveAs(stream);
            return new Tuple<string,Stream>($"Act-{SatAct.ActName}{Path.GetExtension(TemplatePath)}",stream);
        }


         class ItemSpecViewModel
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

         class ItemSpecMatViewModel
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

         class ItemObjectCardViewModel
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
