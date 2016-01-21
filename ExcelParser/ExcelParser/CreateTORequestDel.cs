using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;
using CommonFunctions;

namespace ExcelParser.EpplusInteract
{
    public static  class CreateTORequestDel
    {
        private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\TORequestDel.xlsx";

        public static  byte[] Create(string agreement, bool draft, out string error)
        {
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {
                var agreem = context.ShAddAgreements.FirstOrDefault(a => a.AddAgreement == agreement);
                AgreementRepository reposit = new AgreementRepository(context);
                TORepository repository = new TORepository(context);
                var items = reposit.GetAgreementItems(agreem.AddAgreement);
                var randomItem = items.FirstOrDefault();
                var shTo = context.ShTOes.FirstOrDefault(t=>t.TO==randomItem.TOId);
                var subcontractor = context.SubContractors.FirstOrDefault(s=>s.ShName==shTo.Subcontractor);
                var satPor = context.SATTOs.Where(s => s.TO == shTo.TO && s.UploadedToSh == true).OrderByDescending(s => s.ShUploadDate).FirstOrDefault();
                if (satPor == null)
                {
                    error = string.Format("Для выпуска дел необходимо наличие в САТ опрайсованного ПОР, отправленного и в сотоянии комплитед");
                    return null;
                }
                 var intersectedItems = satPor.SATTOItems.Join(items, s => s.TOItemId, i => i.TOItem, (s, i) => s).ToList();

                    if(intersectedItems.Count!=items.Count)
                    {
                        error = string.Format("Некоторые из выбранных позиций отсутствуют в выпущеном поре");
                        return null;
                    }
                   
                
                var ItemAdditionList = intersectedItems.Select(s=>new ItemAdditionModel(){
                 // PlanDate = s.PlanDate,
                   Description = s.Description,
                    Address = s.SiteAddress,
                     Quantity = s.Quantity,
                      Site = s.Site,

                       

                }).ToList();
                 int index = 0;
                    foreach (var item in ItemAdditionList)
                    {
                        index++;
                        item.SId = index.ToString();
                    }
                
                //var _satTo = repository.GetLastSATTOList().FirstOrDefault(t => t.Id == satToId);

                //if (_satTo != null)
                //{
                //    var satTo = context.SATTOs.Find(satToId);

                //    var _itemSpecTable = satTo.SATTOItems.Where(i => i.Type == "Service").Select((i, ind) => new ItemSpecViewModel()
                //    {
                //        Description = i.Description,
                //        Price = i.Price,
                //        PricePerItem = i.PricePerItem,
                //        Quantity = i.Quantity,
                //        Units = i.Unit,
                //        Id = ind
                //    }).ToList();
                //    var itemSpecTable = _itemSpecTable.GroupBy(g => g.Description).OrderBy(o => o.Key).Select((i, ind) => new ItemSpecViewModel()
                //    {
                //        Description = i.FirstOrDefault().Description,
                //        Price = i.Sum(p => p.Price),
                //        PricePerItem = i.FirstOrDefault().PricePerItem,
                //        Quantity = i.Sum(q => q.Quantity),
                //        Units = i.FirstOrDefault().Units,
                //        Id = ind + 1,
                //        SId = string.Format("{0}.XXX", ind + 1),
                //        Empty = "#merger(1,0)"
                //    }).ToList();
                //    var itemSpecTableDataTable = itemSpecTable.ToDataTable();
                //    itemSpecTableDataTable.Columns.Remove("Id");
                //    var itemSpecMatTable = satTo.SATTOItems.Where(i => i.Type == "Material").Select((i, ind) => new ItemSpecMatViewModel()
                //    {
                //        Description = i.Description,
                //        Price = i.Price,
                //        PricePerItem = i.PricePerItem,
                //        Quantity = i.Quantity,
                //        Units = i.Unit,
                //        Id = ind + 1,
                //        Site = string.Format("{0}", i.Site)
                //    }).ToList();

                //    var _itemObjectCardTable = satTo.SATTOItems.Where(i => i.Type == "Service");
                //    var itemObjectCardTable = _itemObjectCardTable.GroupBy(g => g.Description).OrderBy(f => f.Key).Select((g, gind) => g.Select((i, ind) =>


                //        new ItemObjectCardViewModel()
                //        {
                //            Description = i.Description,
                //            Price = i.Price,
                //            Address = i.SiteAddress,
                //            Quantity = i.Quantity,
                //            Site = string.Format("{0}", i.Site),
                //            PlanDate = i.PlanDate,
                //            Id = ind + 1,
                //            SId = string.Format("{1}.{0}", (ind + 1).ToString("000"), gind + 1)
                //        })).SelectMany(x => x).ToList();
                    var ItemAdditionTable = ItemAdditionList.ToDataTable();
                    ItemAdditionTable.Columns.Remove("Id");


                //    service.InsertTableToPatternCellInWorkBook("ItemSpecTable", itemSpecTableDataTable, new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                //    service.InsertTableToPatternCellInWorkBook("ItemMatSpecTable", itemSpecMatTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
              
                service.InsertTableToPatternCellInWorkBook("ItemAdditionTable", ItemAdditionTable, new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                    
                Dictionary<string, string> dict = new Dictionary<string, string>();
                //    // var shTO
                //    string servicendsText;
                //    string materialndsText;
                //    string totalndsText;
                //    string cardNDSText;

                //    decimal ndskoeff;
                //    if (satTo.WOVAT)
                //    {
                //        ndskoeff = 0;
                //        servicendsText = "без НДС";
                //        materialndsText = "без НДС";
                //        totalndsText = "без НДС";
                //        cardNDSText = "без НДС";
                //    }
                //    else
                //    {
                //        ndskoeff = 0.18M;
                //        servicendsText = "а кроме того НДС - 18%, что составляет #ServiceTotalNDS# рублей";
                //        materialndsText = "а кроме того НДС - 18%, что составляет #MatTotalNDS# рублей";
                //        totalndsText = "а кроме того НДС - 18%, что составляет #TotalNDS# рублей";
                //        cardNDSText = "кроме того НДС - 18%, что составляет #TotalNDS# рублей";

                //    }

                    //var sostavRabot = repository.GetTOSostavRabot(satTo.TO);

                    //var totalwoNDS = satTo.Total;
                    //var nds = totalwoNDS * ndskoeff;
                    //var totalWNDS = totalwoNDS + nds;
                    //var serviceTotalNDS = satTo.TotalServices * ndskoeff;
                    //var serviceTotalWNDS = satTo.TotalServices + serviceTotalNDS;
                    //dict.Add("NDSText", string.Format("{0:P0}", ndskoeff));
                    //dict.Add("ServiceNDSText", servicendsText);
                    //dict.Add("MaterialNDSText", materialndsText);
                    //dict.Add("TotalNDSText", totalndsText);
                    //dict.Add("CardNDSText", cardNDSText);
                    dict.Add("TO", satPor.TO);
                    //dict.Add("ServiceTotalWONDS", satTo.TotalServices.ToString("F"));
                    //dict.Add("ServiceTotalNDS", serviceTotalNDS.ToString("F"));
                    //dict.Add("ServiceTotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(satTo.TotalServices, CommonFunctions.InWords.Заглавные.Первая));
                    //dict.Add("ServiceTotalWNDS", serviceTotalWNDS.ToString("F"));

                    //var matTotalNDS = satTo.TotalMaterials * ndskoeff;
                    //var matTotalWNDS = satTo.TotalMaterials + matTotalNDS;
                    //dict.Add("MatTotalWONDS", satTo.TotalMaterials.ToString("F"));
                    //dict.Add("MatTotalNDS", matTotalNDS.ToString("F"));
                    //dict.Add("MatTotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(satTo.TotalMaterials, CommonFunctions.InWords.Заглавные.Первая));
                    //dict.Add("MatTotalWNDS", matTotalWNDS.ToString("F"));

                    //dict.Add("TotalWONDS", totalwoNDS.ToString("F"));
                    //dict.Add("TotalWONDSp", CommonFunctions.InWords.Валюта.Рубли.Пропись(totalwoNDS, CommonFunctions.InWords.Заглавные.Первая));
                    //dict.Add("TotalNDS", nds.ToString("F"));
                    //dict.Add("TotalWNDS", totalWNDS.ToString("F"));
                    dict.Add("DogNum", satPor.NomerDogovora);
                    dict.Add("PONumber", shTo.PONumber);
                    dict.Add("AddAgreementNum", agreem.AddAgreement);
                    if (shTo != null)
                        dict.Add("WorkDescription", shTo.WorkDescription);
                    dict.Add("DogDate", satPor.DataDogovora.HasValue ? satPor.DataDogovora.Value.ToString("dd.MM.yyyy") : "");
                    dict.Add("Subcontractor", satPor.SubContractor);
                    //dict.Add("porFile", string.IsNullOrEmpty(porFileName) ? "" : porFileName);
                    //var shSubcontractor = context.SubContractors.FirstOrDefault(s => s.Name == satTo.SubContractor || s.ShName == satTo.SubContractor);
                    //if (shSubcontractor != null)
                    //{

                    //    var shContact = context.ShContacts.FirstOrDefault(s => s.Contact == shSubcontractor.ShName);
                    //    if (shContact != null && !string.IsNullOrWhiteSpace(shContact.SubcFace))
                    //    {
                    //        dict.Add("SubcFace", shContact.SubcFace);
                    //    }
                    //    else
                    //    {
                    //        dict.Add("SubcFace", @"""please fill in SH""");
                    //    }
                    //}
                    //var _firstItem = _itemObjectCardTable.FirstOrDefault();
                    // нефига печатать пор, если там трабл с сервис айтемами. А то он еще отправится автоматом...
                    //if (_firstItem != null)
                    //{
                    //var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == _firstItem.Site);
                    ////   if (shSite != null)
                    //{
                    //    dict.Add("SiteBranch", shSite.Branch);
                    //}

                    // }



                    //if (sostavRabot != null && sostavRabot.Count > 0 && shTO.PrintSOW)
                    //{

                    //    string text = string.Format("Подрядчик выполняет на каждом Объекте следующие работы:");
                    //    dict.Add("SOWText", text);
                    //    service.InsertTableToPatternCellInWorkBook("SOWTable", sostavRabot.Select(s => new { desc = s.Description, merge = "#merger(1,7)" }).ToList().ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, MinSeparatedRows = 0, CopyFirstRowStyle = true });
                    //}
                    //else
                    //{
                    //    dict.Add("SOWText", "");
                    //    dict.Add("SOWTable", "");

                    //}

                    if (draft)
                        service.Draft();

                    service.ReplaceDataInBook(dict, true);
                    service.CellsMerger();

                    var stream = new MemoryStream();

                    service.app.SaveAs(stream);
                    error = "";
                    return StaticHelpers.ReadToEnd(stream);
                
            }
           
        }
    private  class ItemAdditionModel
    {
           public int Id { get; set; }
           public string SId { get; set; }
           public string Site { get; set; }
           public string Address { get; set; }
           public string Description { get; set; }
           public decimal Quantity { get; set; }
           //public DateTime? PlanDate { get; set; }
          
    }
    }
    
}
