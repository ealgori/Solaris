using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.TaskParamModels;
using DbModels.DataContext;
using TaskManager;
using System.Text.RegularExpressions;

using CommonFunctions.Extentions;

using DbModels.DomainModels.ShClone;
using MailProcessing;
using CommonFunctions;
using DbModels.DataContext.Repositories;
using WIHInteract;
using SHInteract.Handlers.Solaris;
using DbModels.DomainModels.SAT;
using System.IO;
using DbModels.Repository;
using Models;
using TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers;
using TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomFiHandlers;
using EpplusInteract;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Redemption;
using System.Reflection;
using AutoImport.Rev3.AutoImportHandlers.Models;
using EFCache;
using TaskManager.Handlers.ImportHandler.Models.ExcelExport;
using CommonFunctions.Extentions;
using UnidecodeSharpFork;
using ShClone.UniReport;
using System.Data;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses;


namespace TestProject
{
    
     


    [TestClass]
    public class TestTasks
    {





        [TestMethod]
        public void AstelitFileUpload()
        {
            string poName = "PO - MANUAL - 4510966808 - KI0001";
            string filePath = @"C:\Users\ealgori\Documents\SlimTuneCLR-log.txt";
            string arhFilePath = @"\\RU00112284\p\Archive\AstelitPorFiles";
            SHInteract.Handlers.Astelit.PORUploader.Handle(filePath, poName, arhFilePath);
        }

        [TestMethod]
        public void PropisTest()
        {
           var number=   13452.22M;
            MoneyToStr ds = new MoneyToStr("UAH", "UKR", "");
            var res  = ds.convertValue((double)(number));
            // var result = CommonFunctions.InWords.Валюта.Рубли.Пропись(number, CommonFunctions.InWords.Заглавные.Первая);

        }

        [TestMethod]
        public void SeedDB()
        {
            using (var context = new Context())
            {
                for (int i = 0; i < 5000; i++)
               {
                    var avr = new ShAVRs();
                    avr.AVRId = string.Format("{0}", i.ToString("D6"));
                    avr.Priority = 1;
                    avr.AVRType = "00";
                    avr.TotalAmount = 49999;
                    avr.RukFiliala = "Утвержден";
                    avr.ShVCRequests = new List<ShVCRequest>();

                    var request = new ShVCRequest();
                  
                    request.Id = string.Format("{0}:20151107", avr.AVRId);
                    request.ShAVRs = avr;
                    context.ShVCRequests.Add(request);
                    context.ShAVRs.Add(avr);


                }
                context.SaveChanges();
            }
            

        }

        [TestMethod]
        public void TestAstelitFileDownloader()
        {
            SHInteract.Handlers.Astelit.FileDownloader downloader = new SHInteract.Handlers.Astelit.FileDownloader("ASTELIT");
            downloader.Download("KI0844", null);
        }
          
        [TestMethod]
        public void TestAVROrder()
        {
           using(Context context = new Context())
           {
               var testAVR = "206147";
               var avr2 = context.ShAVRs.Where(AVRRepository.BaseComp).FirstOrDefault(a=>a.AVRId==testAVR);


               var items = avr2.Items.ToList();
               var bytes = ExcelParser.EpplusInteract.CreateAVROrder.CreateOrderFile(items, "testOrder");
               StaticHelpers.ByteArrayToFile(@"C:\Temp\AVROrderTest.xlsx", bytes);
               //var avrs = AVRRepository.GetAvrForLimitsRecalculate(context);

           }
          

           


        }

        [TestMethod]
        public void TestAttributes()
        {
       
            using (Context context = new Context())
            {
                var ur = new UniReportBulkCopy("ShVCRequests", "DbModels.DomainModels.ShClone.ShVCRequest,DbModels", new List<string> { @"C:\temp\ShVCRequests_2015-11-08_17.15.53_31177.1.xls" }, context, new List<DataTable>());
                var res = ur.GetRequiredFields();
                var res2 = ur.FieldMatching();
               // ur.ReadDT();
                
            }
        }


 [TestMethod]
        public void SelectAVRsTest2()
        {
            using (Context context = new Context())
            {
                string testAVR = "205836";
                var avrs4 = AVRRepository.GetReadyForPricingAVRList(context).Where(s => string.IsNullOrEmpty(s.PurchaseOrderNumber)).OrderBy(av=>av.AVRId).ToList();
                var avr5 = avrs4.Select(av => new { avr = av.AVRId, conf = av.RukFiliala, workStart = av.WorkStart, workEnd = av.WorkEnd, needPreprice = av.NeedPreprice }).ToList();
                var avrs = AVRRepository.GetReadyForPricingAVRList(context).Where(av => av.AVRId == testAVR).ToList(); ;
                var avrs3 = AVRRepository.GetReadyForPricingAVRList(context).Where(s => string.IsNullOrEmpty(s.PurchaseOrderNumber)).Where(av => av.AVRId == testAVR).ToList();
                var avrs2 = context.ShAVRs.Where(av => av.AVRId == testAVR).ToList();
                var a = context.ShAVRs.FirstOrDefault(av => av.AVRId == testAVR);
                var s1 = a.Priority.HasValue;

                var s2 = s1 && (a.RukFiliala == "Утвержден");
                var s3 = s2 && a.TotalAmount > 50000 && (a.RukRegionApproval == "Утвержден");
                var s4 = s3 ||(a.TotalAmount <= 50000);
                var s5 = s4 && a.Items != null && a.Items.Any();
            }

            var mail = new RDOMail();
            mail.Subject = "test";
            mail.SaveAs(@"c:\temp\mail.msg");  
     //using(Context context = new Context())
            //{
            //    var avr = context.ShAVRs
            //        //.Include("ShVCRequests")
            //       .Find("200208Test");

            //    var avrItem = context.ShAVRItems.Find(0);
            //    var avrItem2 = context.ShAVRItems.Find(2);
            //    var avr2 = context.ShAVRs
            //        //.Include("ShVCRequests")
            //      .Find("200006");
            //    var res = AVRRepository.ReadyForPorComp(avr);
            //   //var res2 = AVRRepository.ReadyForRequestComp(avr);
            //    //var res3 = AVRRepository.ReadyForRequestComp(avr2);

            //    //var avrs = context.ShAVRs.Where(AVRRepository.BaseComp);

            //}
         

           


        }

        
        
        [TestMethod]
        public void Transliterate()
        {
            var path = @"C:\Temp\PriceDescription.xlsx";
            var wsObjs = EpplusSimpleUniReport.ReadFile(path, "Sheet1", 2);
            foreach (var price in wsObjs)
            {
                price.Column5 = price.Column4.Unidecode();
            }


            var bytes = NpoiInteract.DataTableToExcel(wsObjs.ToDataTable());
            CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\temp\TranslateItems.xls", bytes);

        }




        [TestMethod]
        public void CacheTest()
        {

            var testSW = new System.Diagnostics.Stopwatch();
            testSW.Start();
            Context context = new Context();
            var items = context.ShTOItems.ToList();
            testSW.Stop();
            System.Diagnostics.Debug.WriteLine("first:{0}", testSW.Elapsed.TotalSeconds);
            testSW.Reset();
            testSW.Start();
            for (int i = 0; i < 10; i++)
            {


                Context context2 = new Context();
                var items2 = context2.ShTOItems.ToList();


            }
            testSW.Stop();
            System.Diagnostics.Debug.WriteLine("50 times:{0}", testSW.Elapsed.TotalSeconds);

        }

        [TestMethod]
        public void GetCellvalueTest() 
        {
            var path = @"C:\Temp\Logs\09.10.2015\ShTOItem_2015-10-09_08.31.27_44511.1.xls";
            var workBook = NpoiInteract.ConnectExlFile(path);
            var sheet = workBook.GetSheetAt(0);
            var row = sheet.GetRow(1);
            var cell = row.GetCell(7);
            var cellValue = NpoiInteract.GetCellObjectValueExt(cell, typeof(DateTime?));





        }
        
        
        //[TestMethod]
        //public void EpplusTest()
        //{
        //    //using (Context context = new Context())
        //    //{
        //    //    while (true)
        //    //    {
        //    //        var task = context.DbTasks.FirstOrDefault(t => t.Id == 5);

        //    //        if (task.ImportFileName1 != "0")
        //    //        {
        //    //            break;
        //    //        }
        //    //        System.Threading.Thread.Sleep(10 * 1000);
        //    //    }
        //    //    //PriceListRepository reposit = new PriceListRepository();
        //    //    //var subcId = 99;

        //    //    //var compPl = reposit.GetComparablePriceList(subcId);
        //    //    //var workPl = reposit.GetPriceListsForCompare(subcId).ToList();

        //    //}


        //   // string data = @"Content-disposition: attachment; filename=""=?utf-8?B?0J/QodCa0JIsMV/QutCw0YDRgtC+0YfQutCw?= =?utf-8?B?INC4INC/0YDQvtGC0L7QutC+0Lsuemlw?=""";
        //   //// var replaced = data.Replace('-', '+');
        //   // var res = GetSubStrings(data, "?utf-8?B?", "?=").ToList();
        //   // byte[] plain = Convert.FromBase64String(res[1]);
        //   // Encoding iso = Encoding.GetEncoding("UTF-8");
        //   // string newData = iso.GetString(plain);



        //    var result = AutoImport.Rev3.AttachmentHandler.TemplateFinder.Find("AutoTOItemAppove", "SOLARIS", "1.0");

        //}


        //private IEnumerable<string> GetSubStrings(string input, string start, string end)
        //{
        //    Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
        //    MatchCollection matches = r.Matches(input);
        //    foreach (Match match in matches)
        //        yield return match.Groups[1].Value;
        //}

        [TestMethod]
        public void InvoiceUpdateHandler()
        {

            InvoiceUpdateImportHandler handler = new InvoiceUpdateImportHandler();
            handler.Handle(new AutoImport.Rev3.DomainModels.Attachment() { FilePath = @"C:\temp\Copy of SAP_Payment_terms_tool v3.xlsx", ReadFileName = "Copy of SAP_Payment_terms_tool v3.xlsx" });

        }

        [TestMethod]
        public void TestTOItemApproveHandler()
        {

            var attachment = new AutoImport.Rev3.DomainModels.Attachment();
            attachment.Id = 0;
            attachment.Mail = new AutoImport.Rev3.DomainModels.Mail();
            attachment.Mail.Sender = "aleksey.gorin@ericsson.com";
            attachment.FilePath = @"C:\Temp\Logs\22.10.2015\TestTOItemApprove - Copy.xlsx";

            TOItemApproveAiHandler handler = new TOItemApproveAiHandler();
            handler.Handle(attachment);

        }

        [TestMethod]
        public void EpplusImageTest()
        {

            using (EpplusService service = new EpplusService(@"C:\TEMP\RAN VC_2 (2).xlsx"))
            {
                var ws = service.GetSheet("Installation");
                var imgMask = "Img";
                List<ExcelRangeBase> headersList = new List<ExcelRangeBase>();
                List<ExcelRangeBase> imageHeadersList = ws.Cells.Where(c => c.Text.Trim().StartsWith(imgMask)).ToList();
                List<ImageClass> images = new List<ImageClass>();

                foreach (OfficeOpenXml.Drawing.ExcelPicture draw in ws.Drawings.Where(d => d is OfficeOpenXml.Drawing.ExcelPicture))
                {
                    var image = new ImageClass();
                    string imageHeader = string.Empty;
                    byte[] bytes;
                    if (draw.From.Row > 0)
                    {

                        image.HeaderCell = ws.Cells[draw.From.Row, draw.From.Column + 1];


                    }
                    image.Picture = draw;
                    images.Add(image);

                }


                var sectGroups = images.GroupBy(i => i.Section).ToList();

                foreach (var group in sectGroups)
                {
                    string section = string.Empty;
                    var minSectCell = group.FirstOrDefault(i => i.HeaderText == group.Min(m => m.HeaderText));
                    if(minSectCell.HeaderCell!=null)
                        section = ws.Cells[minSectCell.HeaderCell.Start.Row - 3, minSectCell.HeaderCell.Start.Column + 3].Text.Trim();
                                       
                    var subSectGroup = group.GroupBy(i => i.SubSection).ToList();
                    foreach (var sgroup in subSectGroup)
                    {


                        var minsubSectCell = sgroup.FirstOrDefault(i => i.HeaderText == group.Min(m => m.HeaderText));
                        if (minsubSectCell != null)
                        {
                            try
                            {
                                if (minsubSectCell.HeaderCell != null)
                                {
                                    if (minsubSectCell.HeaderCell.Start.Row - 4 > 0)
                                    {


                                        string questionary = ws.Cells[minsubSectCell.HeaderCell.Start.Row - 1, minsubSectCell.HeaderCell.Start.Column].Text.Trim();
                                        string answer = ws.Cells[minsubSectCell.HeaderCell.Start.Row - 1, minsubSectCell.HeaderCell.Start.Column + 3].Text.Trim();
                                         foreach (var item in group)
                                        {
                                            item.Questionary = questionary;
                                            item.QuestionaryAnswer = answer;
                                            item.SectionText = section;
                                        }



                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                    }
                }
                //var image = ws.Drawings.FirstOrDefault(d=>d.From.Row==28&&d.From.Column==2);
                //var imagedr = ((OfficeOpenXml.Drawing.ExcelPicture)image);
                //byte[] bytes;
                //using (var ms = new MemoryStream())
                //{
                //    try
                //    {
                //        imagedr.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //        bytes = ms.ToArray();
                //    }
                //    catch (Exception)
                //    {

                //        throw;
                //    }

                //}
                //CommonFunctions.StaticHelpers.ByteArrayToFile(@"c:\temp\test.jpg",bytes);
                //var cell261 = ws.Cells[28,2];
                //var cell262 = ws.Cells[29,10];

            }

        }

        private class ImageClass
        {

            public ExcelRangeBase HeaderCell { get; set; }
            public string HeaderText
            {
                get
                {
                    if (HeaderCell != null)
                        return HeaderCell.Text.Trim();
                    return "";
                }
            }
            public string SubSection
            {
                get
                {
                    if (!string.IsNullOrEmpty(HeaderText))
                    {
                        return HeaderText.Substring(4, 5);
                    }
                    return "";
                }
            }
            public string Section
            {
                get
                {
                    if (!string.IsNullOrEmpty(HeaderText))
                    {
                        return HeaderText.Substring(4, 1);
                    }
                    return "";
                }
            }

            public string SectionText { get; set; }
            public string Questionary { get; set; }
            public string QuestionaryAnswer { get; set; }

            public ExcelPicture Picture { get; set; }
            public byte[] ImageBytes
            {
                get
                {
                    if (Picture != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            try
                            {
                                Picture.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                return ms.ToArray();
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }
                    }
                    return null;
                }
            }

        }

        [TestMethod]
        public void TOFIHandler()
        {

            var handler = new TOIFIHandler();
            var amail = new global::Models.AutoMail() { Attachments = new List<Models.Attachment>() };
            amail.Subject = "FileImport#TOI#ТЮМЕНЬ_ТО-1_СКВ#72509";
            var attachment = new Models.Attachment() { FilePath = @"C:\temp\ТО-1 СКВ Сыктывкар.xlsx", File = "ТО-1 СКВ Сыктывкар.xlsx" };
            amail.Attachments.Add(attachment);
            handler.Handle(amail);


        }

        [TestMethod]
        public void ActCreateHandler()
        {

            var handler = new TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers.ActCreateAutoimportHandler();
            var amail = new global::AutoImport.Rev3.DomainModels.Mail() { Attachments = new List<global::AutoImport.Rev3.DomainModels.Attachment>() };
            amail.Author = "ealgori";
            var attachment = new global::AutoImport.Rev3.DomainModels.Attachment { FilePath = @"C:\Users\ealgori\Downloads\ActAutoImport (1).xlsx" };
            //amail.Attachments.Add(attachment);
            attachment.Mail = amail;
            handler.Handle(attachment);


        }


    

        [TestMethod]
        public void AutoImportTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SOLAutoImport") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void NotifyHandlerTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "NotifyHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void DistributionHandler3test()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "DistributionHandler3") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }


        [TestMethod]
        public void SaveMailToAdmin()
        {
           
            using (Context context = new Context())
            {
                ////context.lo
                TaskManager.Handlers.TaskHandlers.Models.AVR.SaveMailToAdmin.Handle("206350", context);
         
            }

        }

        [TestMethod]
        public void SendNotifyHandlerTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendNotifyHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void SendRequestHandlerTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendRequestHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void SendRequestHandler2Test()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendRequestHandler2") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }   
        [TestMethod]
        public void VCAnalyzerTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "VCRequestAnalyzer") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void NeedPrepriceTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "NeedPrepriceHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

 [TestMethod]
        public void NeedPrepriceConditionTest()
        {

            using (Context context = new Context())
            {

                var shAvr = context.ShAVRs.FirstOrDefault(f => f.AVRId == "205836");
                if (shAvr != null)
                {
                    var needPreprice = NeedPrepriceCondition.Need(shAvr);
                    var readyForRequest = ReadyToRequestCondition.Ready(shAvr);
                    var readyForPOR = ReadyToPORCondition.Ready(shAvr);
                }
            }

        }

 [TestMethod]
        public void RequestsTest()
        {

            using (Context context = new Context())
            {

                //var request = context.ShVCRequests.FirstOrDefault(f => f.Id == "205779:20151208125926");
                //if (request != null)
                //{
                var successRequest1 = new ShVCRequest { Id = "succes1", RequestSend = DateTime.Now };
                var successRequest2 = new ShVCRequest { Id = "succes2", RequestSend = DateTime.Now , HasRequest=true, RequestAccepted=DateTime.Now};
                var successRequest3 = new ShVCRequest { Id = "succes3", RequestSend = DateTime.Now , HasRequest=true, RequestAccepted=DateTime.Now, HasOrder = true,  OrderAccepted=DateTime.Now};

                Assert.IsTrue(VCRequestRepository.SuccessRequestComp(successRequest1));
                Assert.IsTrue(VCRequestRepository.SuccessRequestComp(successRequest2));
                Assert.IsTrue(VCRequestRepository.SuccessRequestComp(successRequest3));

                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest1));
                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest1));
                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest1));




                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest1));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest2));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest3));



                var unSuccessRequest1 = new ShVCRequest { Id = "unsucces1", RequestSend = DateTime.Now  };
                var unSuccessRequest2 = new ShVCRequest { Id = "unsucces2", RequestSend = DateTime.Now, HasOrder=true, OrderRejected=DateTime.Now };
                var unSuccessRequest3 = new ShVCRequest { Id = "unsucces3", RequestSend = DateTime.Now , HasRequest=true, RequestAccepted=DateTime.Now, RequestRejected=DateTime.Now};
                var unSuccessRequest4 = new ShVCRequest { Id = "unsucces3", RequestSend = DateTime.Now, HasRequest = true, RequestAccepted = DateTime.Now, RequestRejected = DateTime.Now , HasOrder=true, OrderAccepted=DateTime.Now, OrderRejected=DateTime.Now};


                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(unSuccessRequest1));
                Assert.IsTrue(VCRequestRepository.UnSuccessRequestComp(unSuccessRequest2));
                Assert.IsTrue(VCRequestRepository.UnSuccessRequestComp(unSuccessRequest3));
                Assert.IsTrue(VCRequestRepository.UnSuccessRequestComp(unSuccessRequest4));

                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest1));
                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest2));
                Assert.IsFalse(VCRequestRepository.UnSuccessRequestComp(successRequest3));



                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest1));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest2));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(successRequest3));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(unSuccessRequest1));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(unSuccessRequest2));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(unSuccessRequest3));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(unSuccessRequest4));
                Assert.IsTrue(VCRequestRepository.CompleteRequestComp(unSuccessRequest2));


                    //var completed = VCRequestRepository.CompleteRequestComp(request);
                    //var succed =   VCRequestRepository.SuccessRequestComp(request);
                    //var unsucced = VCRequestRepository.UnSuccessRequestComp(request);
                //}
            }

        }


        [TestMethod]
        public void ReadyForPorTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ReadyForPORHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void ReadyForRequestHandlerTest()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ReadyForRequestHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void TestSize()
        {

            RedemptionMailProcessor proc = new RedemptionMailProcessor("LTE");
          //  proc.GetFolderSize("");

        }

        [TestMethod]
        public void ToImport()
        {

            var a = CommonFunctions.StaticHelpers.GetDatedPath("");
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TOImport") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void PORDelHandlerTest()
        {

            var a = CommonFunctions.StaticHelpers.GetDatedPath("");
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORDelHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void AVRFileUploadTest()
        {

            AVRFileUploaderSol.Handle(@"C:\Temp\test.xlsx", "200003");


        }
        [TestMethod]
        public void PorRequests()
        {

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendWIHPORRequests") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void WIHAnalyzer()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "WIHAnalyzer") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void DataToSH()
        {
            var props = typeof(TaskManager.Handlers.TaskHandlers.Models.Billing.DataToSH.TOVympelcomManSer).GetProperties();
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "DataToSH") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void DistributionHandler()
        {
            var props = typeof(TaskManager.Handlers.TaskHandlers.Models.Billing.DataToSH.TOVympelcomManSer).GetProperties();
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "DistributionHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
         [TestMethod]
        public void SendWIHSapCodeRequest()
        {
            
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendWIHSAPCodeRequest") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

         [TestMethod]
         public void testSapCodeRecored()
         {

             using (Context context = new Context())
             {

                 var testSapCode = "ECR-SOLA-SER-18992";
                 var shCode = context.SAPCodes.FirstOrDefault(s => s.Code == testSapCode);
                 if(shCode!=null)
                 {
                     shCode.EmailId = "test";
                 }
                 context.SaveChanges();
             }

         }

        [TestMethod]
        public void DistributionHandler2()
        {
            var props = typeof(TaskManager.Handlers.TaskHandlers.Models.Billing.DataToSH.TOVympelcomManSer).GetProperties();
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "DistributionHandler2") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void TestPORDEl()
        {
          
            //var bytes2 = ExcelParser.EpplusInteract.CreateTORequestDel.Create("TEST", true, out error);
            //StaticHelpers.ByteArrayToFile(@"C:\Temp\deltor.xlsx", bytes2);

            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendWIHPORDelRequests") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }


        [TestMethod]
        public void CreatePORsinSH()
        {
            //using (Context context = new Context())
            //{

            //    DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORtoSH") };
            //    var task = TaskFactory.GetTask(paramsdd, context);
            //    task.Process();
            //}
            var b2 = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(7079, true);
            File.WriteAllBytes(@"c:\temp\avrPORTest.xlsx", b2);

            var b = ExcelParser.EpplusInteract.CreateTOPOR.CreatePorFile(322,true);
            File.WriteAllBytes(@"c:\temp\toPORTest.xlsx", b);

        }
        //       [TestMethod]
        //       public void SendMUSFSOPOtoOD()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "FSOPORtoOD") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        //       [TestMethod]
        //       public void SendPOtoOD()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORtoOD") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        //       [TestMethod]
        //       public void CreateWorkOrderTest()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PrintOutWorkOrder") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        //       [TestMethod]
        //       public void RecallPORs()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORRecall") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }


        //       [TestMethod]
        //       public void CreatePOSync()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PO Sync") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        //       [TestMethod]
        //       public void CreateWorkOrderTestChekalin()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PrintOutSupplement") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        [TestMethod]
        public void AutoReportTest()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SOLAutoReport") };
                var task = TaskFactory.GetTask(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void AutoReportTestRev2()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SOLAutoReportRev2") };
                var task = TaskFactory.GetTask(paramsdd, context);
                task.Process();
            }

        }

        [TestMethod]
        public void TOImportTest()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TOImport") };
                var task = TaskFactory.GetTask(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void UploadTOPOR()
        {
            using (Context context = new Context())
            {

                SHInteract.Handlers.Solaris.UploadTOPOR.Handle(@"C:\Temp\UpdateImport.xlsx", @"C:\Temp\UpdateWithB.xlsx", "ИЖЕВСК_ТО_ГУ");
            }

        }
        //       [TestMethod]
        //       public void TORefresh()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TORefresh") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }
        //       [TestMethod]
        //       public void TOToSHHandler()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TOToSHHandler") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }

        [TestMethod]
        public void ActToSHHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ActToSHHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        [TestMethod]
        public void ActPrintToSHHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ActPrintToSHHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }

        }
        //       [TestMethod]
        //       public void SiteIndexerHandler()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SiteIndexerHandler") };
        //               var task = TaskFactory.GetTask(paramsdd, context);
        //               task.Process();
        //           }

        //       }

        [TestMethod]
        public void UploadPORTORFiles()
        {
            using (Context context = new Context())
            {
                var satTOes = context.SATTOs.GroupBy(s => s.TO).ToList();
                foreach (var satToGR in satTOes)
                {


                    TORepository repository = new TORepository(context);
                    // var satTo = repository.GetLastSATTOList().FirstOrDefault(s=>s.TO==readyToSendTO.TO&&s.UploadedToSh);
                    var satTo = satToGR.OrderByDescending(s => s.CreateUserDate).FirstOrDefault();
                    var now = DateTime.Now;
                    string fileName = GenerateTOPorName(now);
                    if (fileName.Length > 42)
                    {
                        //TaskParameters.TaskLogger.LogError(string.Format("Название файла больше 42 символов '{0}'", fileName));
                        continue;
                    }
                    string fileName1 = GeneratedTORequestName(now);

                    var porBytes = ExcelParser.EpplusInteract.CreateTOPOR.CreatePorFile(satTo.Id);
                    if (porBytes == null)
                    {
                        //TaskParameters.TaskLogger.LogError(string.Format("Ошибка при генерации пора:'{0}' - id:'{1}'",readyToSendTO.TO, satTo.Id));
                        continue;
                    }
                    var docTOBytes = ExcelParser.EpplusInteract.CreateTORequest.CreateTORequestFile(satTo.Id, fileName);
                    if (docTOBytes == null)
                    {
                        //TaskParameters.TaskLogger.LogError(string.Format("Ошибка при генерации ТО запроса:'{0}' - id:'{1}'", readyToSendTO.TO, satTo.Id));
                        continue;
                    }



                    // сохраним файл пора в архив
                    var archive = Path.Combine(@"C:\temp\TOPORS", now.ToString(@"yyyy\\MM\\dd"));
                    if (!Directory.Exists(archive))
                    {
                        try
                        {
                            Directory.CreateDirectory(archive);
                        }
                        catch (Exception exc)
                        {
                            //TaskParameters.TaskLogger.LogError(string.Format("Ошибка создания папки  '{0}'; {1}", archive, exc.Message));
                            continue;
                        }
                    }
                    var filePath = Path.Combine(archive, fileName);
                    if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, porBytes))
                    {
                        //TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath));
                        continue;
                    }
                    var filePath1 = Path.Combine(archive, fileName1);
                    if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath1, docTOBytes))
                    {
                        //TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath1));
                        continue;
                    }

                    SHInteract.Handlers.Solaris.UploadTOPOR.Handle(filePath, filePath1, satTo.TO);
                }

            }
        }

        public string GenerateTOPorName(DateTime date)
        {
            return string.Format("POR-TO-{0}.xlsx", date.ToString("yyyyMMdd_HHmmss_ffff"));
        }
        public string GeneratedTORequestName(DateTime date)
        {
            return string.Format("TOR-TO-{0}.xlsm", date.ToString("yyyyMMdd_HHmmss_ffff"));
        }

        [TestMethod]
        public void TestPORecallHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORecallHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }

        //       [TestMethod]
        //       public void PrintActToSH()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ActPrintToSHHandler") };
        //               var task = TaskFactory.GetTaskTest(paramsdd, context);
        //               task.Process();
        //           }
        //       }   
        [TestMethod]
        public void ActLink()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ActsInvoiceLinkingHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        } //       }   
        [TestMethod]
        public void LimitCalcHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "LimitCalcHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }
        [TestMethod]
        public void TOTotalAmmountUpdate()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TOTotalAmmountUpdate") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }

        [TestMethod]
        public void SendNotify()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendNotifyHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        } 
        [TestMethod]
        public void FolderBackUpHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "FolderBackUpHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }  
        [TestMethod]
        public void ItemPrepriceUploadHandler()
        {
            using (Context context = new Context())
            {

                DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ItemPrepriceUploadHandler") };
                var task = TaskFactory.GetTaskTest(paramsdd, context);
                task.Process();
            }
        }
        [TestMethod]
        public void GetTable()
        {
            UniReport.UniReportBulkCopy<MDClass> report = new UniReport.UniReportBulkCopy<MDClass>(@"C:\Users\ealgori\Documents\MasterData.xls");
            // считали объекты из эксель файла
            var objs = report.ReadFile();
            List<MDClass2> md2Collect = new List<MDClass2>();
            foreach (var obj in objs)
            {
                var md2 = new MDClass2() { Code = obj.Code, Description = obj.Description, DescriptionEng = obj.DescriptionEng, Unit = obj.Unit };
                md2.CodeEnd = obj.Code.CUnidecode();
                md2Collect.Add(md2);
            }
            // конвертируем их в дататэйбл, чтобы воспользоваться существующим функционалом
            var dataTable = md2Collect.ToDataTable();
            var bytes = NpoiInteract.DataTableToExcel(dataTable);
            CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Users\ealgori\Documents\MasterData2.xls",bytes);

        }

        private class MDClass
        {
            public string Code{get;set;}
             public string    Description{get;set;}
              public string   Unit{get;set;}
               public string     DescriptionEng{get;set;}
              // public string CodeEng { get; set; }

        }

        private class MDClass2
        {
            public string Code { get; set; }
            public string CodeEnd { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public string DescriptionEng { get; set; }
            // public string CodeEng { get; set; }

        }

        //       [TestMethod]
        //       public void SubcSync()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SubcontractorSyncHandler") };
        //               var task = TaskFactory.GetTaskTest(paramsdd, context);
        //               task.Process();
        //           }
        //       }
        //[TestMethod]
        //       public void BackUps()
        //       {
        //           using (Context context = new Context())
        //           {

        //               DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "BackUpHandler") };
        //               var task = TaskFactory.GetTaskTest(paramsdd, context);
        //               task.Process();
        //           }
        //       }
        //       [TestMethod]
        //       public void testports()
        //       {
        //           List<string> unPors = new List<string>();

        //           using (Context context = new Context())
        //           {

        //               var result = context.SATTOs.GroupBy(g => g.TO);
        //               foreach (var group in result)
        //               {
        //                   var firstItems = group.OrderBy(g => g.CreateUserDate).FirstOrDefault().SATTOItems.ToList();
        //                   var lastItems = group.OrderByDescending(g => g.CreateUserDate).FirstOrDefault().SATTOItems.ToList();

        //                   if (firstItems.Sum(c => c.Quantity * c.Price) != lastItems.Sum(c => c.Quantity * c.Price))
        //                   {
        //                       unPors.Add(group.Key);
        //                   }
        //               }

        //               var asdfasd = context.ShTOes.ToList().Join(unPors, t => t.TO, u => u, (t, u) => (t.TO + " - " + t.PONumber));
        //               var text = string.Join(";", asdfasd);
        //           }
        //       }

        [TestMethod]
        public void GetWIHNumbers()
        {
            RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
            var mails = processor.GetMails(new List<string> { " created" });
            List<string> WIH = new List<string>();
            foreach (var mail in mails)
            {
                if (mail.Body.Contains("Please recol"))
                {
                    string subject = mail.Subject;
                    string wih = WIHInteractor.GetWIHId(subject);
                    WIH.Add(wih);
                }
            }
            var wihs = string.Join(";", WIH);
        }
   [TestMethod]
        public void TestRedemption()
        {
            RedemptionMailProcessor processor = new RedemptionMailProcessor("VCSRS");
            var mail = new AutoMail();
            mail.Email = "aleksey.gorin@ericsson.com";
            mail.Body = "test";
            processor.SendMail(mail);
        }
        //       [TestMethod]
        //       public void testFileUpload()
        //       {

        //           FileUploadSol.Handle(new List<string>() { @"C:\Temp\recv.4120.txt" }, "10011");
        //       }


        //       [TestMethod]
        //       public void CreateAct()
        //       {
        //           using (Context contex = new Context())
        //           {
        //               TORepository repository = new TORepository(contex);
        //               var lastSatToes = repository.GetLastSATTOList();
        //               var items = lastSatToes.SelectMany(i => i.SATTOItems).Where(i => i.Type == "Service").Select(i => new { id = i.TOItemId, quantity = i.Quantity });
        //               var matitems = lastSatToes.SelectMany(i => i.SATTOItems).Where(i => i.Type == "Material").Select(i => new { id = i.MatTOItemId, quantity = i.Quantity });
        //               string tempPath = @"C:\Temp\services.xls";
        //               string tempPath1 = @"C:\Temp\materials.xls";
        //               var bytes1 = NpoiInteract.DataTableToExcel(items.ToList().ToDataTable());
        //               var bytes2 = NpoiInteract.DataTableToExcel(matitems.ToList().ToDataTable());
        //               CommonFunctions.StaticHelpers.ByteArrayToFile(tempPath, bytes1);
        //               CommonFunctions.StaticHelpers.ByteArrayToFile(tempPath1, bytes2);

        //           }




        //       }
        //       [TestMethod]
        //       public void testQuery()
        //       {
        //           string tempPath = @"C:\Temp\testAct.xlsx";
        //           string tempPath1 = @"C:\Temp\testAct1.xlsx";
        //           string sheetName = "test1";
        //           var list = new List<TestClass>();
        //           list.Add(new TestClass() { Field1 = "asdf", Field2 = DateTime.Now, Field3 = 12.11 });
        //           list.Add(new TestClass() {Field2 = DateTime.Now, Field3 = 12.11 });
        //           list.Add(new TestClass() { Field1 = "asdf", Field3 = 12.11 });
        //           list.Add(new TestClass() { Field1 = "asdf", Field2 = DateTime.Now});
        //           list.Add(new TestClass() { Field3 = 12.11 });
        //           for (var i = 0; i < 30; i++)
        //           {
        //               list.Add(new TestClass());
        //           }
        //           EpplusInteract.EpplusService service = new EpplusInteract.EpplusService(new FileInfo(tempPath));
        //           service.InsertTableToWorkSheet(sheetName, list.ToDataTable(), new EpplusInteract.EpplusService.InsertTableParams() { ClearEmptyCells=true });

        //          //service.Draft();
        //           service.CreateFolderAndSaveBook(Path.GetDirectoryName(tempPath1), Path.GetFileName(tempPath1));

        //       }
        [Serializable]
        private class TestClass
        {
            public string Field1 { get; set; }
            public DateTime? Field2 { get; set; }
            public double? Field3 { get; set; }
        }

        [TestMethod]
        public void SendTemplateMail2()
        {
            var templatePath = @"\\RU00112284\Solaris AVR documentation\205779_20151202144349\205779_20151202144349.msg";
            RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
            AutoMail mail = new AutoMail();

            mail.Email = ("aleksey.gorin@ericsson.com");//;aleksey.chekalin@ericsson.com");
            mail.Subject = "TestSubject";
            processor.SendMailByTemplate(mail, templatePath,true);

        }

        [TestMethod]
        public void SendTemplateMail()
        {
            using (Context cont = new Context())
            {
                var avrporItems = cont.AVRPORs.SelectMany(p => p.PorItems).Where(i => i.PriceListRevisionItem != null);
                int avrCoeffs = 0;
                System.Diagnostics.Debug.WriteLine(string.Format("Всего аврАЙТЕМОВ:{0}", avrporItems.Count()));
                foreach (var item in avrporItems)
                {
                    if (item.PriceListRevisionItem.Price != 0)
                    {
                        item.Coeff = item.Price / item.PriceListRevisionItem.Price;
                    }
                    else
                    {
                        item.Coeff = 1;
                        if (item.PriceListRevisionItem.Price != item.Price)
                        {
                            System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                    }
                    if (item.Coeff != 1)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("{0} / {1} = {2}{3} ", item.Price, item.PriceListRevisionItem.Price, item.Coeff, item.Coeff > 5 ? "*******" : ""));
                        avrCoeffs++;
                    }

                }

                var porItems = cont.PORs.SelectMany(p => p.PorItems).Where(i => i.PriceListRevisionItem != null);
                int porCoeffs = 0;
                System.Diagnostics.Debug.WriteLine(string.Format("Всего порАЙТЕМОВ:{0}", porItems.Count()));
                foreach (var item in porItems)
                {
                    if (item.PriceListRevisionItem.Price != 0)
                    {
                        item.Coeff = item.Price / item.PriceListRevisionItem.Price;
                    }
                    else
                    {
                        item.Coeff = 1;
                        if (item.PriceListRevisionItem.Price != item.Price)
                        {
                            System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                    }
                    if (item.Coeff != 1)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("{0} / {1} = {2}{3} ", item.Price, item.PriceListRevisionItem.Price, item.Coeff, item.Coeff > 5 ? "*******" : ""));
                        porCoeffs++;
                    }

                }
                System.Diagnostics.Debug.WriteLine(string.Format("ПОР Коэфф !=1 :{0}", porCoeffs));

                System.Diagnostics.Debug.WriteLine(string.Format("АВР Коэфф !=1 :{0}", avrCoeffs));

                cont.SaveChanges();
            }

        }

    }
}
