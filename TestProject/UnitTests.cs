using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TaskManager.Service;
using DbModels.DataContext;
//using TaskManager.Handlers.TaskHandlers.Models.PrintOut;
using OfficeOpenXml;
using TaskManager.TaskModel;
using DbModels.DomainModels.DbTasks;
//using TaskManager.Handlers.TaskHandlers.Models.MUSForms;
using TaskManager.TaskParamModels;

using DbModels.DomainModels.ShClone;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

using System.Globalization;
//using TaskManager.Handlers.TaskHandlers.Models.PO;

using DbModels.Models;

namespace TestProject
{

 //   [TestClass]
 //   public class UnitTests
 //   {
 //       [TestMethod]
 //       public void Save()
 //       {
 //           //string FileFolderPath = @"C:\Users\esovalr\Desktop\";
 //           //FileFolderPath.RemoveBadChars();
 //           //string FileName = @"C:\Users\esovalr\Desktop\PrintOut.xlsx";
 //           //string FilePath = Path.Combine(FileFolderPath, FileName);
 //           //FileInfo template = new FileInfo(FilePath);
 //           //EpplusService service = new EpplusService(template);
 //           //using (Context context = new Context())
 //           //{
 //           //    List<PrintOutProc> list = context.Database.SqlQuery<PrintOutProc>("PrintOut").ToList();
 //           //    int i = 0;
 //           //    //Начинаем обрабатывать конкретный WO
 //           //    foreach (var wo in list)
 //           //    {
 //           //        #region Определение типа заказа, получение необходимых данных, заполнение листа


 //           //        ExcelWorkbook book = service.app.Workbook;

 //           //        string WoType = context.ShWOs.Where(w => w.WO == wo.WO).Select(w => w.WoType).FirstOrDefault();
 //           //        if (string.IsNullOrEmpty(WoType))
 //           //        {

 //           //        }

 //           //        //Получаем необходимую для заказа информацию
 //           //        Dictionary<string, string> dict = new Dictionary<string, string>();
 //           //        var values = context.Regions.Select(r => new PrintOutDataWorkOrder
 //           //        {
 //           //            AddressEng = wo.WO
 //           //        }).FirstOrDefault();
 //           //        foreach (var item in StaticHelpers.GetProperties(values))
 //           //        {
 //           //            dict.Add(item.Name, item.GetValue(values, null).ToString());
 //           //        }
 //           //        ExcelWorksheet sht = service.GetSheet(WoType);
 //           //        service.ReplaceDataInSheet(sht, dict);

 //           //        #endregion

 //           //        #region Формирование списка работ, вставка в шаблон

 //           //        #endregion

 //           //        #region Заполнение данных для план-графика
 //           //        #endregion

 //           //        #region Сохранение(ПДФ, Xls)
 //           //        string OutPath = Path.Combine(FileFolderPath, string.Format("{0}({1}){2}", template.Name, i, template.Extension));
 //           //        FileInfo outFile = new FileInfo(OutPath);
 //           //        service.app.SaveAs(outFile);
 //           //        #endregion

 //           //    }
 //           //}
 //       }


 //       [TestMethod]
 //       public void MusHandlerTest()
 //       {
 //           //Context context = new Context();

 //           //DbTask dbtask = context.DbTasks.FirstOrDefault(sdfsf => sdfsf.Name == "MUSWBSRequest");


 //           ////dbtask.ImportFileName1 = "Importasdf.xls";
 //           ////  TaskBase task = new TaskBase(dbtask,null,contex);
 //           //TaskParameters param = new TaskParameters();
 //           //param.Context = context;
 //           //param.DbTask = dbtask;
 //           //MUSWBSRequest handler = new MUSWBSRequest(param);
 //           //handler.Handle();


 //       }

 //       [TestMethod]
 //       public void POHandlerTest()
 //       {
 //           Context context = new Context();

 //           DbTask dbtask = context.DbTasks.FirstOrDefault(sdfsf => sdfsf.Name == "PO");


 //           //dbtask.ImportFileName1 = "Importasdf.xls";
 //           //  TaskBase task = new TaskBase(dbtask,null,contex);
 //           TaskParameters param = new TaskParameters();
 //           param.Context = context;
 //           param.DbTask = dbtask;
 //           //POHandler handler = new POHandler(param);
 //           //handler.Handle();


 //       }


 //       [TestMethod]
 //       public void RunStored()
 //       {

 //           //StaticHelpers.GetStoredProcDataFromServer<Log>("TestDELETME", null);
 //           //List<MUSApprovedProc> MUSApprovedList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<MUSApprovedProc>("ERUMOMW0009_OHDB_MUS_Approved_Sync", null);
 //           using (Context context = new Context())
 //           {
 //               Dictionary<string, object> dict = new Dictionary<string, object>();
 //               dict.Add("@Subc", "FSO");
 //               List<FSOMUStoODProc> PORList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromContext<FSOMUStoODProc>(context, "SendFSOPORtoOD", dict);

 //               //StaticHelpers.GetStoredProcDataFromServer<Log>("TestDELETME", null);
 //               //int subcFSOId = SHPORService.GetObjectIdByName("GetSubcId", "@Subc", "FSO");
 //               //Assert.AreEqual(subcFSOId, 21);
 //               //int subcFSOId1 = SHPORService.GetObjectIdByName("GetSubcId", "@Subc", "FSOfdfs");
 //               //Assert.AreEqual(subcFSOId1, 0);
 //               //int subcOrbitaId = SHPORService.GetObjectIdByName("GetSubcId", "@Subc", "Orbita PKF");
 //               //Assert.AreEqual(subcOrbitaId, 123);
 //               ////int macroregionId = SHPORService.GetObjectIdByName("GetMacroRegionId", "@MacroRegion", "North West");
 //               ////Assert.AreEqual(macroregionId, 2);
 //               ////int projectId = SHPORService.GetObjectIdByName("GetProjectId", "@Project", "NRO SWAP 2G Siemens network NWE");
 //               //Assert.AreEqual(projectId, 2);
 //               //int subcId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetSubcId", "@Subc", subc);
 //               SAPCode code = context.SAPCodes.FirstOrDefault(c => c.Code == "ECR_SWAPNW_UC.011");
 //               int subcId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetSubcId", "@Subc", "FSO");
 //               int macroregionId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetMacroRegionId", "@macroRegion", "North West");

 //               int projectId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetProjectId", "@Project", "NRO SWAP 2G Siemens network NWE");
 //               decimal priceFSO = SHPORService.GetPrice("ECR_SWAPNW_UC.011", "Республика Коми", macroregionId, new DateTime(2013, 3, 1), projectId, subcId);
 //               //    decimal price = SHPORService.GetPrice("ECR_SWAPNW_PD.010", "Калининградская область", macroregionId, new DateTime(2013, 3, 1), projectId, subcOrbitaId);
 //           }
 //       }


 //       [TestMethod]
 //       public void SHWoItemTest()
 //       {
 //           DataTable table = new DataTable();
 //           List<ShWO> itemList = new List<ShWO>()
 //               {
 //                   new ShWO(){ 
 //                   WO = "TEST WO WORK",
 //                   WBS = "asdfasdfasdf",
 //                  WoType = "asdfasdf",
 //                  WoFor="asdf",
 //                 // Print = false,
 //                  WoDate = DateTime.Now

 //                   }
 //               };

 //           table = itemList.ToDataTable(typeof(ShWO));

 //           string connectString1 = System.Configuration.ConfigurationManager.
 //                  ConnectionStrings["LocalDb"].ConnectionString;
 //           using (SqlConnection connection1 = new SqlConnection(connectString1))
 //           {

 //               try
 //               {
 //                   {


 //                       using (var loader = new System.Data.SqlClient.SqlBulkCopy(connectString1))
 //                       {

 //                           foreach (DataColumn col in table.Columns)
 //                           {
 //                               loader.ColumnMappings.Add(new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName));
 //                           }


 //                           loader.DestinationTableName = "ShWOEs";
 //                           loader.WriteToServer(table);

 //                       }

 //                   }





 //               }

 //               catch (Exception ex)
 //               {

 //               }



 //           }
 //       }

 //       [TestMethod]
 //       public void GetCurrency()
 //       {
 //           var rates = CommonFunctions.ExchangeRates.CurrencyRates.GetExchangeRates();
 //           var rate = CommonFunctions.ExchangeRates.CurrencyRates.GetExchangeRateByValuteStringCode("USD");

 //       }
 //       public class TestClass
 //       {
 //            public DateTime? NullableDateTime { get; set; }
 //            public DateTime NullDateTime { get; set; }
 //            public int Integer { get; set; }
 //       }

 //[TestMethod]
 //       public void GettNuullablePropTest()
 //       {
 //           List<TestClass> tcl = new List<TestClass>();   
 //   tcl.Add(new TestClass());
 //   tcl.Add(new TestClass());
 //   tcl.Add(new TestClass());
 //   var dt = tcl.ToDataTableString(typeof(TestClass));


 //       }


 //   }
}
