//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.IO;
//using DbModels.DataContext;
//using TaskManager.Service;
////using TaskManager.Handlers.TaskHandlers.Models.PrintOut;
//using OfficeOpenXml;

//using DbModels.DomainModels.ShClone;
//using CommonFunctions.StaticHelper;
//using CommonFunctions.Extentions;
//namespace TestProject
//{
//    [TestClass]
//    public class TestPrinOut
//    {
//        public const string WorkOrderSheet = "Work Order";
//        public const string SoWSheet = "SoW";
//        public const string TempSoWSheet = "Temp SoW";
//        public const string GantChartSheet = "Gant Chart";
//        [TestMethod]
//        public void TestSow()
//        {

//            string FileName = @"C:\Users\esovalr\Desktop\WO-TEST-123.xlsm";
//            FileInfo template = new FileInfo(FileName);
//            EpplusService service = new EpplusService(template);
//            var sht = service.GetSheet(GantChartSheet);

//           var firstForm = sht.ConditionalFormatting.FirstOrDefault();
//           firstForm.Address = new ExcelAddress("K20:EF20 K21:EF21)");
//            var form = sht.ConditionalFormatting.AddExpression(sht.Cells[21, 11, 21, 136]);
//            form.Formula = "AND(AB$10>=$C21;AB$10<$C21+$H21+1)";
//            form.Style.Font.Color = firstForm.Style.Font.Color;
           
//            //service.AutoFit(sht);
//            service.app.Save();
//            //List<DefaultWorkItem> list = new List<DefaultWorkItem>();
//            //list.Add(new DefaultWorkItem
//            //{
//            //    Code = "Code1",
//            //    Type = "PIR"
//            //});
//            //list.Add(new DefaultWorkItem
//            //{
//            //    Code = "Code2",
//            //    Type = "PIR"
//            //});
//            //list.Add(new DefaultWorkItem
//            //{
//            //    Code = "Code3",
//            //    Type = "Services"
//            //});
//            //list.Add(new DefaultWorkItem
//            //{
//            //    Code = "Code4",
//            //    Type = "Services"
//            //});
//            //foreach (var type in list.Distinct().Select(l => l.Type))
//            //{
//            //    var range = service.app.Workbook.Names["range" + type];
//            //    for (int i = range.Start.Row; i <= range.End.Row; i++)
//            //    {
//            //        sht.Row(i).Hidden = true;
//            //    }
//            //    service.InsertTableToPatternCellInWorkBook(type, list.Where(l => l.Type == type).ToList().ToDataTable(typeof(DefaultWorkItem)), new EpplusService.InsertTableParams());
              
//            //}
           
//        }
//        [TestMethod]
//        public void TestGant()
//        {

//            string FileName = @"C:\Users\esovalr\Desktop\WO-TEST-123.xlsm";
//            FileInfo template = new FileInfo(FileName);
//            EpplusService service = new EpplusService(template);
//            var shtGant = service.GetSheet(GantChartSheet);
//            ExcelNamedRange GantStart = service.app.Workbook.Names["GantStart"];
//            ExcelNamedRange GantEnd = service.app.Workbook.Names["GantEnd"];
//            ExcelNamedRange RowStart = service.app.Workbook.Names["RowStart"];
//            ExcelNamedRange Scope = service.app.Workbook.Names["Scope"];
//            var timePlan = new TimePlan
//            {
//                StartDate = new DateTime(2013, 1, 1),
//                EndDate = new DateTime(2013, 1, 30),
//                Rows = new List<TimePlanRow>()
//            };
//            timePlan.Rows.Add(new TimePlanRow
//            {
//                Position = "2.1",
//                Code = "Code1",
//                Diruation = 10,
//                StartDate = new DateTime(2013, 1, 1),
//                EndDate = new DateTime(2013, 1, 10)
//            });

//            timePlan.Rows.Add(new TimePlanRow
//            {
//                Position = "2.2",
//                Code = "Code2",
//                Diruation = 10,
//                StartDate = new DateTime(2013, 1, 11),
//                EndDate = new DateTime(2013, 1, 20)
//            });
//            timePlan.Rows.Add(new TimePlanRow
//            {
//                Position = "2.3",
//                Code = "Code3",
//                Diruation = 10,
//                StartDate = new DateTime(2013, 1, 21),
//                EndDate = new DateTime(2013, 1, 30)
//            });
//            shtGant.Cells[GantStart.Address].Value = timePlan.StartDate;
//            shtGant.Cells[GantEnd.Address].Value = timePlan.EndDate;
//            shtGant.Cells[Scope.Address].Value = timePlan.StartDate;
//            shtGant.Cells[Scope.Address].Offset(0, 1).Value = timePlan.EndDate;
//            shtGant.Cells[Scope.Address].Offset(0, 2).Value = timePlan.Rows.Sum(s => s.Diruation);
//            int curRow = RowStart.Start.Row + 1;
//            //shtGant.InsertRow(curRow, timePlan.Rows.Count, curRow-1);
//            //shtGant.Cells[CopyRows.Address].Copy(shtGant.Cells[curRow,1,curRow+ timePlan.Rows.Count,CopyRows.End.Column]);

//            foreach (var row in timePlan.Rows)
//            {
//                shtGant.Row(curRow).Height = 9;
//                shtGant.Cells[curRow, 1].Value = row.Position;
//                shtGant.Cells[curRow, 2].Value = row.Code;
//                shtGant.Cells[curRow, 3].Value = row.StartDate;
//                shtGant.Cells[curRow, 4].Value = row.EndDate;
//                shtGant.Cells[curRow, 5].Value = row.Diruation;
//                curRow++;
//            }
//            //shtGant.DeleteRow(RowStart.Start.Row, 1);
//            //service.InsertTableToPatternCellInWorkBook("GanttTable", timePlan.Rows.ToDataTable(typeof(TimePlanRow)),
//            //       new EpplusService.InsertTableParams() { PrintHeaders = false, MinSeparatedRows =1 });
//            FileInfo OutputPath = new FileInfo(@"C:\Users\esovalr\Desktop\test1.xlsx");

//            service.app.SaveAs(OutputPath);
//        }
//        [TestMethod]
//        public void TestEpplus()
//        {
//            string FileFolderPath = @"C:\Users\esovalr\Desktop\";
//            string FileName = @"\\E768B599F0AF1A.ericsson.se\OrderTemplates\WO Templates\PrintOutWorkOrder.xlsx";

//            string FilePath = Path.Combine(FileFolderPath, FileName);
//            FileInfo template = new FileInfo(FilePath);
//            EpplusService service = new EpplusService(template);
//            ExcelWorkbook book = service.app.Workbook;
//            ExcelWorksheet shtWo = service.GetSheet(WorkOrderSheet);
//            ExcelWorksheet shtSoW = service.GetSheet(SoWSheet);
//            ExcelWorksheet tempShtSoW = service.GetSheet(TempSoWSheet);
//            ExcelWorksheet shtGant = service.GetSheet(GantChartSheet);

//            //ExcelNamedRange SoWHeader = shtSoW.Names["SoWHeader"];
//            ExcelNamedRange TableHeader = book.Names["TableHeader"];
//            //ExcelNamedRange SoWTotal = shtSoW.Names["SoWTotal"];
//            ExcelNamedRange Signature = book.Names["Signature"];
//            ExcelNamedRange Start = book.Names["Start"];
//            int RowStart = Start.Start.Row;

//            int ColumnStart = Start.Start.Column + 1;
//            int lastPageBreakRow = 0;
//            for (int i = 0; i < 80; i++)
//            {
//                shtSoW.Cells[RowStart, ColumnStart + 1].Value = "fdsaffffffffffffffffffffffffffffff   sdffsdfffffff   fsdfsdfsdfsdfdfsfsd  fsdfsdfsdfsdffffffffffffffffffffffffffffffff        fsdfsdfsdsdfsfff" + i;
//                //shtSoW.Cells[RowStart, ColumnStart + 1].Style.WrapText = true;

//                if (PageBreakNeeded(shtSoW, lastPageBreakRow))
//                {
//                    TableHeader.Copy(shtSoW.Cells[RowStart, ColumnStart]);
//                    lastPageBreakRow = RowStart;
//                }
//                RowStart++;
//            }
//            shtSoW.View.PageBreakView = true;


//            #region Сохранение(ПДФ, Xls)
//            string OutPath = Path.Combine(FileFolderPath, template.Name.Replace(template.Extension, string.Format("(1){0}", template.Extension)));
//            FileInfo outFile = new FileInfo(OutPath);

//            service.app.SaveAs(outFile);
//            #endregion
//        }
//        public bool PageBreakNeeded(ExcelWorksheet sht, int lastPageBreakRow)
//        {
//            double TotalHeight = 0;
//            for (int i = lastPageBreakRow; i < sht.Dimension.End.Row; i++)
//            {
//                TotalHeight += sht.Row(i).Height;
//            }
//            if (TotalHeight > 705)
//            {
//                return true;
//            }
//            return false;
//        }
//        [TestMethod]
//        public void WorkOrder()
//        {
//            string FileFolderPath = @"C:\Users\esovalr\Desktop\";
//            string FileName = @"C:\Users\esovalr\Desktop\PrintOutWorkOrder.xlsx";

//            string FilePath = Path.Combine(FileFolderPath, FileName);
//            FileInfo template = new FileInfo(FilePath);
//            EpplusService service = new EpplusService(template);
//            ExcelWorkbook book = service.app.Workbook;
//            ExcelWorksheet shtWo = service.GetSheet(WorkOrderSheet);
//            ExcelWorksheet shtSoW = service.GetSheet(SoWSheet);
//            ExcelWorksheet tempShtSoW = service.GetSheet(TempSoWSheet);
//            ExcelWorksheet shtGant = service.GetSheet(GantChartSheet);

//            using (Context context = new Context())
//            {
//                List<PrintOutProc> list = context.Database.SqlQuery<PrintOutProc>("PrintOutWorkOrder").ToList();
//                int i = 0;
//                //Начинаем обрабатывать конкретный WO
//                foreach (var wo in list)
//                {
//                    #region Проверка ошибок

//                    if (!SHWOService.WoExists(wo.WO, context))
//                    {
//                        throw new NullReferenceException(string.Format("Заказ {0} не найден в !", wo.WO));
//                    }


//                    ShWO shWo = SHWOService.GetWO(wo.WO, context);
//                    if (!SHWOService.CheckEcrAddPositions(shWo, context))
//                    {
//                        throw new NullReferenceException(string.Format("Данные для позиций ECR Add не заполнены", wo.WO));
//                    }
//                    ShSite shSite = SHWOService.GetWoSite(shWo, context);
//                    if (shSite == null)
//                    {
//                        throw new NullReferenceException(string.Format("Cайт для заказа не найден(или элемент, или вбс+_+)", wo.WO));
//                    }
//                    ShNode shNode = SHWOService.GetSiteNode(shSite, context);
//                    if (shNode == null)
//                    {
//                        throw new NullReferenceException(string.Format("Нода для сайта не найдена", shSite.Site));
//                    }

//                    #endregion

//                    #region Формирование списка работ, вставка в шаблон

//                    List<SoWGroup> SoWGroupList = SHWOService.GetSoWGroupList(shWo, context);
//                    ExcelNamedRange Signature = shtSoW.Names["Signature"];
//                    ExcelNamedRange Start = shtSoW.Names["Start"];
//                    int RowStart = Start.Start.Row;
//                    int ColumnStart = Start.Start.Column;
//                    int CurrentRow = RowStart;
//                    int CurrentColumn = ColumnStart;


//                    //SoWHeader.Copy(shtSoW.Cells[1, 1]);
//                    foreach (var soWGroup in SoWGroupList)
//                    {


//                        shtSoW.Cells[CurrentRow, CurrentColumn].Value = soWGroup.Position;
//                        shtSoW.Cells[CurrentRow, CurrentColumn + 1].Value = soWGroup.Position;
//                        CurrentRow++;
//                        foreach (var item in soWGroup.SoWList)
//                        {
//                            shtSoW.Cells[CurrentRow, CurrentColumn].Value = item.Position;
//                            shtSoW.Cells[CurrentRow, CurrentColumn + 1].Value = item.Code;
//                            shtSoW.Cells[CurrentRow, CurrentColumn + 2].Value = item.Description;
//                            shtSoW.Cells[CurrentRow, CurrentColumn + 3].Value = item.Quantity;
//                            shtSoW.Cells[CurrentRow, CurrentColumn + 4].Value = item.Price;
//                            shtSoW.Cells[CurrentRow, CurrentColumn + 5].Value = item.TotalPrice;
//                            CurrentRow++;
//                        }
//                    }
//                    Signature.Copy(shtSoW.Cells[shtSoW.Dimension.Address].Offset(1, 0));
//                    #endregion

//                    #region Заполнение данных для план-графика

//                    var timePlan = SHWOService.GetTimePlan(shWo, context);
//                    ExcelNamedRange rangeStartDate = shtSoW.Names["TimePlanStartDate"];
//                    ExcelNamedRange rangeRowStart = shtSoW.Names["TimePlanRowStart"];

//                    ExcelNamedRange rangePosition = shtSoW.Names["TimePlanPosition"];
//                    ExcelNamedRange rangeCode = shtSoW.Names["TimePlanCode"];
//                    ExcelNamedRange rangeStart = shtSoW.Names["TimePlanStart"];
//                    ExcelNamedRange rangeEnd = shtSoW.Names["TimePlanEnd"];
//                    ExcelNamedRange rangeDuriation = shtSoW.Names["TimePlanDuriation"];
//                    //Заполняем поле с начальной датой, от которой зависит период дат на оси времени графика
//                    rangeStartDate.Value = timePlan.StartDate;
//                    //В заголовке таблицы есть именованный диапазон, прибавляю к строчке адреса которого единицу, мы получим текущую строчку
//                    int currentRow = rangeRowStart.Start.Row + 1;
//                    for (int timePlanRow = 0; timePlanRow < timePlan.Rows.Count; timePlanRow++)
//                    {
//                        shtGant.InsertRow(currentRow, 1);
//                        shtGant.Cells[currentRow, rangePosition.Start.Column].Value = "2." + timePlanRow;
//                        shtGant.Cells[currentRow, rangeCode.Start.Column].Value = timePlan.Rows[timePlanRow].Code;
//                        shtGant.Cells[currentRow, rangeStart.Start.Column].Value = timePlan.Rows[timePlanRow].StartDate;
//                        shtGant.Cells[currentRow, rangeEnd.Start.Column].Value = timePlan.Rows[timePlanRow].EndDate;
//                        shtGant.Cells[currentRow, rangeDuriation.Start.Column].Value = timePlan.Rows[timePlanRow].Diruation;
//                        currentRow++;
//                    }

//                    #endregion

//                    #region Получение необходимых данных, заполнение листа


//                    //Получаем необходимую для заказа информацию
//                    Dictionary<string, string> dict = new Dictionary<string, string>();
//                    var values = context.Regions.Select(r => new PrintOutDataWorkOrder
//                    {
//                        RbsType = shNode.RbsType,
//                        Site = shSite.Site,
//                        AddressEng = shSite.AddressEng,
//                        AddressRus = shSite.AddressEng,
//                        BranchNameEng = r.BranchNameEng,
//                        BranchNameRus = r.BranchNameRus,
//                        FinDirAttorney = r.FinDirAttorney,
//                        FinDirAttorneyDate = r.FinDirAttorneyDate,
//                        FinDirEng = r.FinDirEng,
//                        FinDirRus = r.FinDirRus,
//                        FinDirRusRP = r.FinDirRusRP,
//                        TechDirAttorney = r.TechDirAttorney,
//                        TechDirAttorneyDate = r.TechDirAttorneyDate,
//                        TechDirEng = r.TechDirEng,
//                        TechDirEngPost = r.TechDirEngPost,
//                        TechDirRus = r.TechDirRus,
//                        TechDirRusPost = r.TechDirRusPost,
//                        TechDirRusPostPR = r.TechDirRusPostPR,
//                        TechDirRusRP = r.TechDirRusRP,
//                        WO = shWo.WO,
//                        ParentWo = SHWOService.GetParentWo(shWo, context).WO,
//                        WoDate = shWo.WoDate.Value.ToShortDateString(),
//                        VatWo = SHWOService.GetWOTotalPriceWithVat(shWo, context),
//                        Vat = SHWOService.GetWOTotalPriceVat(shWo, context),
//                    }).FirstOrDefault();
//                    foreach (var item in StaticHelpers.GetProperties(values))
//                    {
//                        dict.Add(item.Name, item.GetValue(values, null).ToString());
//                    }
//                    service.ReplaceDataInBook(dict);
//                    shtWo.AutoFitRows();
//                    shtSoW.AutoFitRows();
//                    shtGant.AutoFitRows();
//                    #endregion

//                    #region Сохранение(ПДФ, Xls)
//                    string OutPath = Path.Combine(FileFolderPath, string.Format("{0}({1}){2}", template.Name, i, template.Extension));
//                    FileInfo outFile = new FileInfo(OutPath);
//                    service.app.SaveAs(outFile);
//                    #endregion

//                }
//            }
//        }
//    }
//}
