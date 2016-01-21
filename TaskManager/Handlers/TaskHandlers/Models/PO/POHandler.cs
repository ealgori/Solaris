using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using TaskManager.Service;
using System.IO;
using CommonFunctions.Extentions;
namespace TaskManager.Handlers.TaskHandlers.Models.PO
{
    /// <summary>
    /// генерация пор
    /// </summary>
    public class POHandler : ATaskHandler
    {



        public class POStoredProcModel
        {
            public string CPMFullName { get; set; }
            public string VendorNameRus { get; set; }
            public DateTime WorkStartDate { get; set; }
            public DateTime WorkEndDate { get; set; }
            public string AddressRus { get; set; }
            public string Network { get; set; }
            public string WBS { get; set; }
            public string VendorNumber { get; set; }
        }

        public class POItemStoredProcClass
        {
            public int No { get; set; }
            public string Cat { get; set; }
            public string Code { get; set; }
            public string Plant { get; set; }
            public decimal NetQty { get; set; }
            public string B1 { get; set; }
            public string B2 { get; set; }
            public string ItemCat { get; set; }
            public string PRtype { get; set; }
            public string B3 { get; set; }
            public string B4 { get; set; }
            public string POrg { get; set; }
            public string B6 { get; set; }
            public string B7 { get; set; }
            public string GLacc { get; set; }
            public decimal Price { get; set; }
            public string Curr { get; set; }
            public string PRUnit { get; set; }
            public string B8 { get; set; }
            public string Vendor { get; set; }
            public string B9 { get; set; }
            public string B10 { get; set; }
            public string B11 { get; set; }
            public string B12 { get; set; }
            public string B13 { get; set; }
            public string B14 { get; set; }
            public string B15 { get; set; }
            public string B16 { get; set; }
            public DateTime Plandate { get; set; }
            public string Description { get; set; }

        }
        public POHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            if (!File.Exists(TaskParameters.DbTask.TemplatePath))
            {
                TaskParameters.TaskLogger.LogError("Не найден темплейт:"+ TaskParameters.DbTask.TemplatePath);
                return false;
            }
            var listModels = GetTestPOModels();
            foreach (var model in listModels)
            {
                
                FileInfo template = new FileInfo(TaskParameters.DbTask.TemplatePath);
                using (EpplusService excelService = new EpplusService(template))
                {
                    Dictionary<string,string> dict = new Dictionary<string,string>();
                    var modelFields = CommonFunctions.StaticHelper.StaticHelpers.GetProperties(model);
                    foreach (var field in modelFields)
                    {
                        dict.Add(field.Name, (field.GetValue(model,null)??"").ToString());
                    }
                    excelService.ReplaceDataInBook(dict);
                    var itemList = GetTestPOItems();
                    excelService.InsertTableToPatternCellInWorkBook("ItemsTable", itemList.ToDataTable(typeof(POItemStoredProcClass)),new EpplusService.InsertTableParams(){
                     BoldHeaders=true,
                      PrintHeaders=true,
                      ShowRowStripes = true,
                      EmptyRowAfterHeaders = true,
                      TableStyle = OfficeOpenXml.Table.TableStyles.Medium24

                    });
                    string sentPath = Path.Combine(TaskParameters.DbTask.EmailSendFolder, string.Format("POR-asklg{0}.xlsx",DateTime.Now.ToString("yyyyMMddHHmmssfffff")));
                    FileInfo saveFile = new FileInfo(sentPath);
                    try
                    {
                        if (!Directory.Exists(TaskParameters.DbTask.EmailSendFolder))
                            Directory.CreateDirectory(TaskParameters.DbTask.EmailSendFolder);
                        excelService.app.SaveAs(saveFile);
                    }
                    catch(Exception exc)
                    {
                        TaskParameters.TaskLogger.LogError("Ошибка сохранения файла:" + sentPath+"::"+exc.Message );
                    }
                }
            }
            return false;
        }


        private List<POStoredProcModel> GetTestPOModels()
        {
            List<POStoredProcModel> models = new List<POStoredProcModel>();
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus1", CPMFullName = "CpmFullName1", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network1", VendorNameRus = "VendorNameRus1", WBS = "WBS1" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus2", CPMFullName = "CpmFullName2", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network2", VendorNameRus = "VendorNameRus2", WBS = "WBS2" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus3", CPMFullName = "CpmFullName3", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network3", VendorNameRus = "VendorNameRus3", WBS = "WBS3" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus4", CPMFullName = "CpmFullName4", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network4", VendorNameRus = "VendorNameRus4", WBS = "WBS4" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus5", CPMFullName = "CpmFullName5", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network5", VendorNameRus = "VendorNameRus5", WBS = "WBS5" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus6", CPMFullName = "CpmFullName6", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network6", VendorNameRus = "VendorNameRus6", WBS = "WBS6" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus7", CPMFullName = "CpmFullName7", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network7", VendorNameRus = "VendorNameRus7", WBS = "WBS7" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus8", CPMFullName = "CpmFullName8", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network8", VendorNameRus = "VendorNameRus8", WBS = "WBS8" });
            models.Add(new POStoredProcModel() { AddressRus = "AddressRus9", CPMFullName = "CpmFullName9", WorkEndDate = DateTime.Now, WorkStartDate = DateTime.Now, Network = "Network9", VendorNameRus = "VendorNameRus9", WBS = "WBS9" });
            return models;
        }

        private List<POItemStoredProcClass> GetTestPOItems()
        {
            List<POItemStoredProcClass> models = new List<POItemStoredProcClass>();
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code1", Plant = "2341", PRtype = "PrType1", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code2", Plant = "2342", PRtype = "PrType2", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code3", Plant = "2343", PRtype = "PrType3", Price =  12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code4", Plant = "2344", PRtype = "PrType4", Price =  12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code5", Plant = "2345", PRtype = "PrType5", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code6", Plant = "2346", PRtype = "PrType6", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code7", Plant = "2347", PRtype = "PrType7", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code8", Plant = "2348", PRtype = "PrType8", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code9", Plant = "23499", PRtype = "PrType9", Price = 12345.31m, Plandate = DateTime.Now });
            models.Add(new POItemStoredProcClass() { No = 1, Code = "Code10", Plant = "234910", PRtype = "PrType10", Price = 12345.31m, Plandate = DateTime.Now });
            return models;


        }
    }
}
