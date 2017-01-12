using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.SAT;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;
using System.IO;
using CommonFunctions;
using DbModels.Models.Pors;
using OfficeOpenXml.Table;


namespace ExcelParser.EpplusInteract
{
    public static class CreatePorDel
    {
        private static readonly string TemplatePath = @"\\RU00112284\OrderTemplates\PORTemplates\PORRecallV2-Template.xlsx";
        public static byte[] GenerateDelPOR(string agreement, bool draft, out string error)
        {

           
            if (!System.IO.File.Exists(TemplatePath))
            {
                //Console.WriteLine("Шаблон не существует:" + TemplatePath);
                error="Шаблон не существует:" + TemplatePath;
                return null;
            }
            using (Context context = new Context())
            {
                var agreem = context.ShAddAgreements.FirstOrDefault(a=>a.AddAgreement==agreement);
                AgreementRepository reposit = new AgreementRepository(context);
                TORepository toReposit = new TORepository(context);
                var items = reposit.GetAgreementItems(agreem.AddAgreement);
                if(items.Count==0)
                {
                    error = string.Format("На эгрименте нет ни одной позиции");
                    return null ;
                }
                
                // проверим что они на одном и том же ТО.
                var groups = items.GroupBy(i=>i.TOId).ToList();
                if(groups.Count>1)
                {
                    error= string.Format("Позиции привязаны к разным ТО:{0}", string.Join(", ", groups.Select(g => g.Key).ToList()));
                    return null;
                }

                // среди готовых, выбираем те, у которых все ок с реквестами
                // для этого нам нужен экземпляр ТО
                var randomItem = items.FirstOrDefault();
                
                //у ТО должен быть пор в состоянии комплитед.
             

                    // генерируем поры и торы.
                    var shTo = context.ShTOes.FirstOrDefault(t=>t.TO==randomItem.TOId);
                    if (shTo == null)
                    {
                        error ="ТО не существует в СХ";
                        return null;
                    }
                    
                    // пор на дел генерируется на основе пора из сат
                    var satPor = context.SATTOs.Where(s=>s.TO==shTo.TO&&s.UploadedToSh==true).OrderByDescending(s=>s.ShUploadDate).FirstOrDefault();
                    if(satPor==null)
                    {
                       error = string.Format("Для выпуска дел необходимо наличие в САТ опрайсованного ПОР, отправленного и в сотоянии комплитед");
                        return null;
                    }

                    // позиции агримента должны пересекаться с позииями пора.
                    var satPorItems =  toReposit.GetSATTOPORItemModels(satPor.Id);
                if(satPorItems==null)
                {
                    error = "нет позций. Возможно не совпадают имена подрядчиков в САТ и СХ";
                    return null;
                }
                    var intersectedItems = satPorItems.Join(items, s => s.ItemId, i => i.TOItem, (s, i) => s).ToList();

                    if(intersectedItems.Count!=items.Count)
                    {
                        error = string.Format("Некоторые из выбранных позиций отсутствуют в выпущеном поре");
                        return null;
                    }
                    //int index = 0;
                    //foreach (var item in intersectedItems)
                    //{
                    //    index++;
                    //    item.No = index;
                    //}

                    


                    using (var service = new EpplusService(TemplatePath))
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        //dict.Add("Activity", satPor.Activity);
                        //dict.Add("PorId", string.Format("SH TO Id:{0}", satPor.TO));

                        //dict.Add("SubContractorName", satPor.SubContractor);

                        //dict.Add("SAPNumber", satPor.SubContractorSapNumber);

                        //dict.Add("SubContractorAddress", satPor.SubContractorAddress);


                        //dict.Add("PriceListNumbers", satPor.ProceListNumbers);
                        //var startDate = intersectedItems.Min(a => a.Plandate);
                        //var endDate = intersectedItems.Max(a => a.Plandate);
                        //if (startDate.Value.Date == endDate.Value.Date)
                        //    startDate = startDate.Value.AddDays(-1);
                        //dict.Add("StartDate", startDate.Value.ToString("dd.MM.yyyy"));





                        //dict.Add("EndDate", endDate.Value.ToString("dd.MM.yyyy"));
                        //dict.Add("today", DateTime.Now.ToString("dd.MM.yyyy"));


                        //dict.Add("Network", satPor.Network);
                        //var sampleItem = items.FirstOrDefault();

                        //dict.Add("Region", satPor.Region);
                        //dict.Add("POType", satPor.ToType);
                        ////dict.Add("Signum", satTo.CreateUserName);???????
                        //var networkNum = int.Parse(satPor.Network);
                        //var pr = context.PurchaseRequests.FirstOrDefault(p => p.Activity.Activity == shTo.ActivityCode && p.Network.Network2014 == networkNum);
                        //if (pr != null)
                        //{
                        //    dict.Add("PurchaseRequest", pr.PurchReqNo);
                        //    dict.Add("PRItem", pr.PRItem);
                        //}

                        dict.Add("PORRecallComments" , "PO was not signed, additional agreement is not needed");
                        dict.Add("PONumber", shTo.PONumber);

                        var sites = string.Join(", ", items.Select(s => s.Site).ToList());

                        var all = string.Join(", ", new List<string>() { sites, }.Where(t => !string.IsNullOrWhiteSpace(t)));
                       // dict.Add("Site", all);

                        foreach (var attach in intersectedItems)
                        {
                            attach.Description = string.Format("{0} ({1})", attach.Description, attach.Description.CUnidecode());
                        }

                    var grouppedItemModels = new List<PORTOItem>();

                    var grouppdedModels = intersectedItems.GroupBy(g => g.Code);
                    foreach (var groupModel in grouppdedModels)
                    {
                        int index = 0;
                        var itemForProps = groupModel.FirstOrDefault();
                        if (itemForProps != null)
                        {
                            var item = new PORTOItem()
                            {
                                No = index + 1,
                                Cat = itemForProps.Cat,
                                Code = itemForProps.Code,
                                Plant = itemForProps.Plant,
                                NetQty = groupModel.Sum(s => s.NetQty),
                                ItemCat = itemForProps.ItemCat,
                                PRtype = itemForProps.PRtype,
                                POrg = itemForProps.POrg,
                                GLacc = itemForProps.GLacc,
                                Price = itemForProps.Price,
                                PRUnit = itemForProps.PRUnit,
                                Vendor = itemForProps.Vendor,
                                Plandate = itemForProps.Plandate,
                                //Description = i.PriceListRevisionItem.Name,
                                // PriceListRevisionItem = i.PriceListRevisionItem,
                                // ItemId = i.TOItemId
                            };


                            grouppedItemModels.Add(item);
                        }

                    }




                    var dataTable = grouppedItemModels.ToList().ToDataTable();
                        dataTable.Columns.Remove("POR");
                        dataTable.Columns.Remove("Id");
                        dataTable.Columns.Remove("PriceListRevisionItem");
                        dataTable.Columns.Remove("IsCustom");


                        
                        service.ReplaceDataInBook(dict);
                        service.InsertTableToPatternCellInWorkBook("Table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Medium7, ShowRowStripes = true, EmptyRowAfterHeaders = true });

                        if (draft)
                            service.Draft();
                        var stream = new MemoryStream();

                        service.app.SaveAs(stream);
                        error = "";
                        return StaticHelpers.ReadToEnd(stream);

               

                    }
                
            }
        }
    }
}
