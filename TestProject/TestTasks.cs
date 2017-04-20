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
//using Redemption;
using System.Reflection;
using AutoImport.Rev3.AutoImportHandlers.Models;
using EFCache;
using TaskManager.Handlers.ImportHandler.Models.ExcelExport;
//using CommonFunctions.Extentions;
//using UnidecodeSharpFork;
using ShClone.UniReport;
using System.Data;
using TaskManager.Handlers.TaskHandlers.Models.WIH;
using System.Globalization;
using System.Net;

namespace TestProject
{
	using UniReportN;
	using static TaskManager.Handlers.TaskHandlers.Models.Putevie.PutevieImportHandler;

	[TestClass]
	public class TestTasks
	{

		[TestMethod]
		public void CreateAct()
		{
			try
			{
				var context = new Context();
			}
			catch (Exception exc)
			{

				throw;
			}
			var bytes = ExcelParser.EpplusInteract.CreateAct.CreateActFile(1290, true);
			CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Temp\actArhive.zip", bytes);
		}

		[TestMethod]
		public void GetSubcs()
		{

			var eapi = new EDiadocApi.EDiadocApi();
			var orgs = eapi.GetOrganizationList();
			var dt = orgs.ToDataTable();
			var bytes = NpoiInteract.DataTableToExcel(dt);
			CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Temp\Orgs.xls", bytes);


		}
		[TestMethod]
		public void TestPorDel()
		{

			string error;
			var porBytes = ExcelParser.EpplusInteract.CreatePorDel.GenerateDelPOR("2016_ЕКАТЕРИНБУРГ_ТО-1_ПСКВ_ДС№1", false, out error);
			var path = @"C:\Temp\testporDEL.xlsx";
			CommonFunctions.StaticHelpers.ByteArrayToFile(path, porBytes);


		}
		[TestMethod]
		public void FolderAccess()
		{

			NetworkCredential theNetworkCredential = new NetworkCredential(@"aleksyg", "Ericsson");
			CredentialCache theNetCache = new CredentialCache();
			theNetCache.Add(new Uri(@"\\sde.internal.ericsson.com"), "Basic", theNetworkCredential);
			string[] theFolders = Directory.GetDirectories(@"https://sde.internal.ericsson.com/Sde.Template/api/SirInstanceApi/DownloadPulledSirReport?instanceId=53803");

		}


		[TestMethod]
		public void AstelitFileUpload()
		{
			string poName = "PO - MANUAL - 4510966808 - KI0001";
			string filePath = @"C:\Users\ealgori\Documents\SlimTuneCLR-log.txt";
			string arhFilePath = @"\\RU00112284\p\Archive\AstelitPorFiles";
			SHInteract.Handlers.Astelit.PORUploader.Handle(filePath, poName, arhFilePath);
		}


		[TestMethod]
		public void Combinatoric()
		{

			var fileName = "449_путевой лист_01.2015.xls";
			var parts = fileName.Split(new char[] { '_' });
			if (parts.Count() != 2)
			{

			}
			var carPart = parts[0];
			var datePart = new string(parts[1].Where(c => Char.IsDigit(c)).Take(7).ToArray());
			// машину определяем по первым трем цифрам
			string carNum = GetCarNum(carPart);
			bool letBool = true;
			bool digbool = true;


			//var list = new List<int> { 1, 2, 3, 4, 5 };
			//var combs = list.GetKCombs(3).ToList();
			//foreach (var comb in combs)
			//{
			//    var a = comb.ToList();
			//}
		}

		public static string GetCarNum(string carName)
		{
			string carNum = string.Empty;
			foreach (var ch in carName.Where(c => c != ' '))
			{
				if (char.IsDigit(ch))
				{
					carNum += ch;
				}
				if (carNum.Length > 2)
					break;
			}
			return carNum;
		}

		[TestMethod]
		public void PropisTest()
		{

			// var upl = WayListUpload.Upload("TESTLIST", @"C:\Temp\avrPORTest2.xlsx");
			StringBuilder builder;
			//  var res2 = SHInteract.Handlers.FileUploader.Upload("SOLARIS","TESTLIST", "Путевые листы admin", @"C:\Temp\avrPORTest2.xlsx", 520830, 1253,out builder);
			var number = 13452.22M;
			MoneyToStr ds = new MoneyToStr("UAH", "UKR", "");

			ds = null;


			var res = ds?.convertValue((double)(number));
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

			//SHInteract.Handlers.FileDownloader downloader = new SHInteract.Handlers.FileDownloader("ASTELIT");
			//downloader.Download("KI0844", null);
		}

		[TestMethod]
		public void TestAVROrder()
		{
			using (Context context = new Context())
			{
				context.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
				var testAVR = "205126";
				//var avr2 = context.ShAVRs.Where(AVRRepository.Base).FirstOrDefault(a=>a.AVRId==testAVR);
				var request = context.VCRequestsToCreate.Where(a => a.AVRId == testAVR).GroupBy(g => g.VCRequestNumber).FirstOrDefault();
				var musItems = context.SatMusItems.Where(i => i.VCRequestNumber == request.Key).ToList();



				// var bytes = ExcelParser.EpplusInteract.CreateAVROrder.CreateOrderFile(musItems, request.Key);
				//StaticHelpers.ByteArrayToFile(@"C:\Temp\AVROrderTest.xlsx", bytes);
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
				//string testAVR = "205836";
				//var avrs4 = AVRRepository.GetReadyForPricingAVRList(context).Where(s => string.IsNullOrEmpty(s.PurchaseOrderNumber)).OrderBy(av=>av.AVRId).ToList();
				//var avr5 = avrs4.Select(av => new { avr = av.AVRId, conf = av.RukFiliala, workStart = av.WorkStart, workEnd = av.WorkEnd, needPreprice = av.NeedPreprice }).ToList();
				//var avrs = AVRRepository.GetReadyForPricingAVRList(context).Where(av => av.AVRId == testAVR).ToList(); ;
				//var avrs3 = AVRRepository.GetReadyForPricingAVRList(context).Where(s => string.IsNullOrEmpty(s.PurchaseOrderNumber)).Where(av => av.AVRId == testAVR).ToList();
				//var avrs2 = context.ShAVRs.Where(av => av.AVRId == testAVR).ToList();
				//var a = context.ShAVRs.FirstOrDefault(av => av.AVRId == testAVR);
				//var s1 = a.Priority.HasValue;

				//var s2 = s1 && (a.RukFiliala == "Утвержден");
				//var s3 = s2 && a.TotalAmount > 50000 && (a.RukRegionApproval == "Утвержден");
				//var s4 = s3 ||(a.TotalAmount <= 50000);
				//var s5 = s4 && a.Items != null && a.Items.Any();
			}

			//   var mail = new RDOMail();
			//  mail.Subject = "test";
			//   mail.SaveAs(@"c:\temp\mail.msg");  
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

			//    //var avrs = context.ShAVRs.Where(AVRRepository.Base);

			//}





		}



		[TestMethod]
		public void Transliterate()
		{
			var path = @"C:\Temp\Logs\04.04.2016\0330 SH tracking.xlsb";
			var wsObjs = EpplusSimpleUniReport.ReadFile(path, "zzpomon", 2);
			//foreach (var price in wsObjs)
			//{
			//    price.Column5 = price.Column4.Unidecode();
			//}


			//var bytes = NpoiInteract.DataTableToExcel(wsObjs.ToDataTable());
			//CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\temp\TranslateItems.xls", bytes);

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

		//[TestMethod]
		//public void GetCellvalueTest() 
		//{
		//    var path = @"C:\Temp\Logs\09.10.2015\ShTOItem_2015-10-09_08.31.27_44511.1.xls";
		//    var workBook = NpoiInteract.ConnectExlFile(path);
		//    var sheet = workBook.GetSheetAt(0);
		//    var row = sheet.GetRow(1);
		//    var cell = row.GetCell(7);
		//    var cellValue = NpoiInteract.GetCellObjectValueExt(cell, typeof(DateTime?));





		//}


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
			attachment.FilePath = @"C:\Temp\Temp\29062016_2.xlsx";

			TOItemApproveAiHandler handler = new TOItemApproveAiHandler();
			handler.Handle(attachment);

		}

		[TestMethod]
		public void TestTOImportHandler()
		{

			var attachment = new AutoImport.Rev3.DomainModels.Attachment();
			attachment.Id = 0;
			attachment.Mail = new AutoImport.Rev3.DomainModels.Mail();
			attachment.Mail.Sender = "aleksey.gorin@ericsson.com";
			attachment.FilePath = @"C:\Temp\Temp\29062016_2.xlsx";

			TOImportHandler handler = new TOImportHandler();
			handler.Handle(attachment);

		}


		//[TestMethod]
		//public void EpplusImageTest()
		//{

		//    using (EpplusService service = new EpplusService(@"C:\TEMP\RAN VC_2 (2).xlsx"))
		//    {
		//        var ws = service.GetSheet("Installation");
		//        var imgMask = "Img";
		//        List<ExcelRangeBase> headersList = new List<ExcelRangeBase>();
		//        List<ExcelRangeBase> imageHeadersList = ws.Cells.Where(c => c.Text.Trim().StartsWith(imgMask)).ToList();
		//        List<ImageClass> images = new List<ImageClass>();

		//        foreach (OfficeOpenXml.Drawing.ExcelPicture draw in ws.Drawings.Where(d => d is OfficeOpenXml.Drawing.ExcelPicture))
		//        {
		//            var image = new ImageClass();
		//            string imageHeader = string.Empty;
		//            byte[] bytes;
		//            if (draw.From.Row > 0)
		//            {

		//                image.HeaderCell = ws.Cells[draw.From.Row, draw.From.Column + 1];


		//            }
		//            image.Picture = draw;
		//            images.Add(image);

		//        }


		//        var sectGroups = images.GroupBy(i => i.Section).ToList();

		//        foreach (var group in sectGroups)
		//        {
		//            string section = string.Empty;
		//            var minSectCell = group.FirstOrDefault(i => i.HeaderText == group.Min(m => m.HeaderText));
		//            if(minSectCell.HeaderCell!=null)
		//                section = ws.Cells[minSectCell.HeaderCell.Start.Row - 3, minSectCell.HeaderCell.Start.Column + 3].Text.Trim();

		//            var subSectGroup = group.GroupBy(i => i.SubSection).ToList();
		//            foreach (var sgroup in subSectGroup)
		//            {


		//                var minsubSectCell = sgroup.FirstOrDefault(i => i.HeaderText == group.Min(m => m.HeaderText));
		//                if (minsubSectCell != null)
		//                {
		//                    try
		//                    {
		//                        if (minsubSectCell.HeaderCell != null)
		//                        {
		//                            if (minsubSectCell.HeaderCell.Start.Row - 4 > 0)
		//                            {


		//                                string questionary = ws.Cells[minsubSectCell.HeaderCell.Start.Row - 1, minsubSectCell.HeaderCell.Start.Column].Text.Trim();
		//                                string answer = ws.Cells[minsubSectCell.HeaderCell.Start.Row - 1, minsubSectCell.HeaderCell.Start.Column + 3].Text.Trim();
		//                                 foreach (var item in group)
		//                                {
		//                                    item.Questionary = questionary;
		//                                    item.QuestionaryAnswer = answer;
		//                                    item.SectionText = section;
		//                                }



		//                            }
		//                        }
		//                    }
		//                    catch (Exception ex)
		//                    {

		//                        throw;
		//                    }
		//                }
		//            }
		//        }
		//        //var image = ws.Drawings.FirstOrDefault(d=>d.From.Row==28&&d.From.Column==2);
		//        //var imagedr = ((OfficeOpenXml.Drawing.ExcelPicture)image);
		//        //byte[] bytes;
		//        //using (var ms = new MemoryStream())
		//        //{
		//        //    try
		//        //    {
		//        //        imagedr.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
		//        //        bytes = ms.ToArray();
		//        //    }
		//        //    catch (Exception)
		//        //    {

		//        //        throw;
		//        //    }

		//        //}
		//        //CommonFunctions.StaticHelpers.ByteArrayToFile(@"c:\temp\test.jpg",bytes);
		//        //var cell261 = ws.Cells[28,2];
		//        //var cell262 = ws.Cells[29,10];

		//    }

		//}

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

			//var handler = new TOIFIHandler();
			//var amail = new global::Models.AutoMail() { Attachments = new List<Models.Attachment>() };
			//amail.Subject = "FileImport#TOI#ТЮМЕНЬ_ТО-1_СКВ#72509";
			//var attachment = new Models.Attachment() { FilePath = @"C:\temp\ТО-1 СКВ Сыктывкар.xlsx", File = "ТО-1 СКВ Сыктывкар.xlsx" };
			//amail.Attachments.Add(attachment);
			//handler.Handle(amail);


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
		public void FuelListNotifierHandlerTest()
		{

			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "FuelListNotifierHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}

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
		public void LimitsAutoImportHandlerTest()
		{

			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "LimitsAutoImportHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}

		}
		[TestMethod]
		public void UploadVCReqToCreateHandler()
		{

			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "UploadVCReqToCreateHandler") };
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
		public void SaveMailToAdmin2Test()
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
				TaskManager.Handlers.TaskHandlers.Models.AVR.CreateVCRequest.Handle("206325", context);

			}

		}
		[TestMethod]
		public void AVRPorSync()
		{

			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "AVRSynchronization") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
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

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "WIHAnalyzer") };
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

			//using (Context context = new Context())
			//{

			//    var shAvr = context.ShAVRs.FirstOrDefault(f => f.AVRId == "205836");
			//    if (shAvr != null)
			//    {
			//        var needPreprice = NeedVCPrepriceCondition.Need(shAvr);
			//        var readyForRequest = ReadyToRequestCondition.Ready(shAvr);
			//        var readyForPOR = ReadyToPORCondition.Ready(shAvr);
			//    }
			//}

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
				var successRequest2 = new ShVCRequest { Id = "succes2", RequestSend = DateTime.Now, HasRequest = true, RequestAccepted = DateTime.Now };
				var successRequest3 = new ShVCRequest { Id = "succes3", RequestSend = DateTime.Now, HasRequest = true, RequestAccepted = DateTime.Now, HasOrder = true, OrderAccepted = DateTime.Now };

				Assert.IsTrue(VCRequestRepository.SuccessRequest(successRequest1));
				Assert.IsTrue(VCRequestRepository.SuccessRequest(successRequest2));
				Assert.IsTrue(VCRequestRepository.SuccessRequest(successRequest3));

				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest1));
				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest1));
				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest1));




				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest1));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest2));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest3));



				var unSuccessRequest1 = new ShVCRequest { Id = "unsucces1", RequestSend = DateTime.Now };
				var unSuccessRequest2 = new ShVCRequest { Id = "unsucces2", RequestSend = DateTime.Now, HasOrder = true, OrderRejected = DateTime.Now };
				var unSuccessRequest3 = new ShVCRequest { Id = "unsucces3", RequestSend = DateTime.Now, HasRequest = true, RequestAccepted = DateTime.Now, RequestRejected = DateTime.Now };
				var unSuccessRequest4 = new ShVCRequest { Id = "unsucces3", RequestSend = DateTime.Now, HasRequest = true, RequestAccepted = DateTime.Now, RequestRejected = DateTime.Now, HasOrder = true, OrderAccepted = DateTime.Now, OrderRejected = DateTime.Now };


				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(unSuccessRequest1));
				Assert.IsTrue(VCRequestRepository.UnSuccessRequest(unSuccessRequest2));
				Assert.IsTrue(VCRequestRepository.UnSuccessRequest(unSuccessRequest3));
				Assert.IsTrue(VCRequestRepository.UnSuccessRequest(unSuccessRequest4));

				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest1));
				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest2));
				Assert.IsFalse(VCRequestRepository.UnSuccessRequest(successRequest3));



				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest1));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest2));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(successRequest3));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(unSuccessRequest1));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(unSuccessRequest2));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(unSuccessRequest3));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(unSuccessRequest4));
				Assert.IsTrue(VCRequestRepository.CompleteRequest(unSuccessRequest2));


				//var completed = VCRequestRepository.CompleteRequest(request);
				//var succed =   VCRequestRepository.SuccessRequest(request);
				//var unsucced = VCRequestRepository.UnSuccessRequest(request);
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
		public void ConditionsHandlerTest()
		{

			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ConditionsHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}

		}
		[TestMethod]
		public void TestSize()
		{

			RedemptionMailProcessor proc = new RedemptionMailProcessor("SOLARIS");

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
		public void TypeCheckHandler()
		{
			var props = typeof(TaskManager.Handlers.TaskHandlers.Models.Billing.DataToSH.TOVympelcomManSer).GetProperties();
			try
			{
				using (Context context = new Context())
				{

					DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "TypeCheckHandler") };
					var task = TaskFactory.GetTaskTest(paramsdd, context);
					task.Process();
				}
			}
			catch (Exception exc)
			{

				throw;
			}


		}
		[TestMethod]
		public void TestMaterialize()
		{
			string command1 = @"select '' as ShAVR, [PO Number] as PO,
					 [Invoice Number] as Invoice, 
					  [Factura Number] as Factura, [document date] as DocumentDate, 
					 [total amount] as TotalAmount, [receiving date] as ReceivingDate, 
					  [approved by od] as ApprovedByOD, comments as Comments, 
					 [passed to finance] as PassedToFinance, 
					 [scanned to ocr wf] as ScannedToocrwf, 
					  [sent to subcontractor] as SentToSubcontractorInv, 
					  [Delivery Note Number] as DeliveryNoteInv, [Item ID] as ItemID
					from dbo.[Accounts_Payable_CU_Russia]
					where [PO Number] is not null AND [Approved by OD]='Approved' 
					order by [item id] ";
			var InvoiceVymManSerResult = CommonFunctions.StaticHelpers.GetStoredProcDataFromServer<TaskManager.Handlers.TaskHandlers.Models.Billing.DataToSH.InvoiceVymManSer>("MAStorage", command1);


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
				if (shCode != null)
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

			try
			{

				//                var path2 = @"\\RU00112284\p\OrderTemplates\SolarisTemplates\TORequest - Copy (2).xlsm";
				//                var path1 = @"\\RU00112284\p\OrderTemplates\SolarisTemplates\TORequest.xlsm";
				//                var path4 = @"\\RU00112284\p\OrderTemplates\SolarisTemplates\TORequest.xlsx";
				//                var path3 = @"\\RU00112284\p\OrderTemplates\SolarisTemplates\POR.xlsx";
				//                var package = new ExcelPackage(new FileInfo(path1), true);
				//                var sheet0 = package.Workbook.Worksheets.ToList()[0];
				//                var sheet1 = package.Workbook.Worksheets.ToList()[1];
				//                var testText = @"3. Стоимость Работ составляет #ServiceTotalWONDS# (#ServiceTotalWONDSp#)  #ServiceNDSText#.
				//Кроме того, Заказчик дополнительно оплачивает Подрядчику стоимость дополнительных материалов, запасных частей и транспортных расходов в соответствии и п. 5 настоящего Заказа, что составляет #MatTotalWONDS# (#MatTotalWONDSp#)  #MaterialNDSText#.
				//Общая стоимость Работ с материалами, запасными частями составляет #TotalWONDS# (#TotalWONDSp#)  #TotalNDSText#.";
				//                for (int i = 1; i < 16; i++)
				//                {
				//                    ExcelRangeBase cell = sheet0.Cells["A"+i.ToString()];
				//                    cell.Value = "test";
				//                    if (!string.IsNullOrEmpty(cell.Value.ToString()))
				//                    {

				//                    }
				//                }

				//ExcelRangeBase cell = sheet0.Cells["A7"];
				//cell.Value = "test";
				//ExcelRangeBase cell2 = sheet0.Cells["A8"];
				//cell.Value = "test";
				//ExcelRangeBase cell1 = sheet1.Cells["A7"];
				//cell1.Value = "test";

				var listTuple = new List<Tuple<string, string, string, string>>
									{
									   new Tuple<string, string, string,string>("1007","4514476477","2017_ЕКАТЕРИНБУРГ_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("991","4514404213","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_1_1М","Ural"),
new Tuple<string, string, string,string>("1008","4514476778","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_1_2М","Ural"),
new Tuple<string, string, string,string>("1009","4514476776","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_1_3М","Ural"),
new Tuple<string, string, string,string>("992","4514404559","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_2_1М","Ural"),
new Tuple<string, string, string,string>("1010","4514476786","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_2_2М","Ural"),
new Tuple<string, string, string,string>("1011","4514476910","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_2_3М","Ural"),
new Tuple<string, string, string,string>("993","4514404608","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_3_1М","Ural"),
new Tuple<string, string, string,string>("1012","4514476794","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_3_2М","Ural"),
new Tuple<string, string, string,string>("1013","4514476828","2017_ЕКАТЕРИНБУРГ_ТО_ВОЛС_3_3М","Ural"),
new Tuple<string, string, string,string>("994","4514404649","2017_ЕКАТЕРИНБУРГ_ТО_МЖКЛС_1М","Ural"),
new Tuple<string, string, string,string>("1014","4514476878","2017_ЕКАТЕРИНБУРГ_ТО_МЖКЛС_2М","Ural"),
new Tuple<string, string, string,string>("1015","4514476958","2017_ЕКАТЕРИНБУРГ_ТО_МЖКЛС_3М","Ural"),
new Tuple<string, string, string,string>("1016","4514477419","2017_ЕКАТЕРИНБУРГ_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1017","4514477204","2017_ЕКАТЕРИНБУРГ_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1018","4514477230","2017_ИЖЕВСК_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("995","4514404673","2017_ИЖЕВСК_ТО_ВОЛС_1М","Ural"),
new Tuple<string, string, string,string>("1019","4514477268","2017_ИЖЕВСК_ТО_ВОЛС_2М","Ural"),
new Tuple<string, string, string,string>("1020","4514477374","2017_ИЖЕВСК_ТО_ВОЛС_3М","Ural"),
new Tuple<string, string, string,string>("1021","4514477977","2017_ИЖЕВСК_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1022","4514478093","2017_ИЖЕВСК_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1023","4514478000","2017_КИРОВ_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("996","4514404717","2017_КИРОВ_ТО_ВОЛС_1М","Ural"),
new Tuple<string, string, string,string>("1024","4514477536","2017_КИРОВ_ТО_ВОЛС_2М","Ural"),
new Tuple<string, string, string,string>("1025","4514477567","2017_КИРОВ_ТО_ВОЛС_3М","Ural"),
new Tuple<string, string, string,string>("1026","4514478028","2017_КИРОВ_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1027","4514478116","2017_КУРГАН_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("997","4514404762","2017_КУРГАН_ТО_ВОЛС_1М","Ural"),
new Tuple<string, string, string,string>("1028","4514478050","2017_КУРГАН_ТО_ВОЛС_2М","Ural"),
new Tuple<string, string, string,string>("1029","4514484185","2017_КУРГАН_ТО_ВОЛС_3М","Ural"),
new Tuple<string, string, string,string>("1030","4514477460","2017_КУРГАН_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1031","4514478032","2017_ПЕРМЬ_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("1032","4514477899","2017_ПЕРМЬ_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1033","4514477551","2017_ПЕРМЬ_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1034","4514477528","2017_СУРГУТ_ТО_АУГПТ_1_1К","Ural"),
new Tuple<string, string, string,string>("998","4514404789","2017_СУРГУТ_ТО_АУГПТ_2_1М","Ural"),
new Tuple<string, string, string,string>("1035","4514477969","2017_СУРГУТ_ТО_АУГПТ_2_2М","Ural"),
new Tuple<string, string, string,string>("1036","4514477702","2017_СУРГУТ_ТО_АУГПТ_2_3М","Ural"),
new Tuple<string, string, string,string>("1037","4514477674","2017_СУРГУТ_ТО_АУГПТ_3_1К","Ural"),
new Tuple<string, string, string,string>("1038","4514477444","2017_СУРГУТ_ТО_АУГПТ_4_1К","Ural"),
new Tuple<string, string, string,string>("1039","4514477638","2017_СУРГУТ_ТО_АУГПТ_5_1К","Ural"),
new Tuple<string, string, string,string>("1040","4514477586","2017_СУРГУТ_ТО_АУГПТ_6_1К","Ural"),
new Tuple<string, string, string,string>("1041","4514477366","2017_СУРГУТ_ТО_АУГПТ_7_1К","Ural"),
new Tuple<string, string, string,string>("1042","4514477715","2017_СУРГУТ_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1043","4514478003","2017_СУРГУТ_ТО-1_СКВ_2М","Ural"),
new Tuple<string, string, string,string>("1044","4514484218","2017_СУРГУТ_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1045","4514484341","2017_СЫКТЫВКАР_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("999","4514404828","2017_СЫКТЫВКАР_ТО_БС_1М","Ural"),
new Tuple<string, string, string,string>("1057","4514484439","2017_СЫКТЫВКАР_ТО_БС_2М","Ural"),
new Tuple<string, string, string,string>("1058","4514485574","2017_СЫКТЫВКАР_ТО_БС_3М","Ural"),
new Tuple<string, string, string,string>("1000","4514404629","2017_СЫКТЫВКАР_ТО_ВОЛС_1М","Ural"),
new Tuple<string, string, string,string>("1046","4514477830","2017_СЫКТЫВКАР_ТО_ВОЛС_2М","Ural"),
new Tuple<string, string, string,string>("1047","4514478083","2017_СЫКТЫВКАР_ТО_ВОЛС_3М","Ural"),
new Tuple<string, string, string,string>("1048","4514477593","2017_СЫКТЫВКАР_ТО-1_СКВ_3М","Ural"),
new Tuple<string, string, string,string>("1049","4514478228","2017_ТЮМЕНЬ_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("1050","4514478132","2017_ТЮМЕНЬ_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1051","4514477704","2017_ЧЕЛЯБИНСК_ТО_АУГПТ_1К","Ural"),
new Tuple<string, string, string,string>("1001","4514404538","2017_ЧЕЛЯБИНСК_ТО_ВОЛС_1М","Ural"),
new Tuple<string, string, string,string>("1052","4514477789","2017_ЧЕЛЯБИНСК_ТО_ВОЛС_2М","Ural"),
new Tuple<string, string, string,string>("1053","4514477757","2017_ЧЕЛЯБИНСК_ТО_ВОЛС_3М","Ural"),
new Tuple<string, string, string,string>("1002","4514404234","2017_ЧЕЛЯБИНСК_ТО_МЖКЛС_1М","Ural"),
new Tuple<string, string, string,string>("1054","4514484895","2017_ЧЕЛЯБИНСК_ТО_МЖКЛС_2М","Ural"),
new Tuple<string, string, string,string>("1055","4514478146","2017_ЧЕЛЯБИНСК_ТО_МЖКЛС_3М","Ural"),
new Tuple<string, string, string,string>("1056","4514478103","2017_ЧЕЛЯБИНСК_ТО-1_АМС_3М","Ural"),
new Tuple<string, string, string,string>("1003","4514414021","SIB TO АУГПТ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("972","4514404218","SIB ТО ВОЛС KRS ИНТАКТИКА 1Q2017","Siberia"),
new Tuple<string, string, string,string>("973","4514404241","SIB ТО ВОЛС KRS МАКРОС(534122,2) 1Q2017","Siberia"),
new Tuple<string, string, string,string>("983","4514404701","SIB ТО ВОЛС KRS МАКРОС(86994,43) 1Q2017","Siberia"),
new Tuple<string, string, string,string>("984","4514404730","SIB ТО ВОЛС NSK АВАНТЕЛ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("985","4514404756","SIB ТО ВОЛС NSK ГОРЭЛЕКТРОСВЯЗЬ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("1004","4514414123","SIB ТО ВОЛС TMS ГТС 1Q2017","Siberia"),
new Tuple<string, string, string,string>("986","4514404925","SIB ТО ВОЛС АТС BRN 0112ВК 1Q2017","Siberia"),
new Tuple<string, string, string,string>("974","4514404302","SIB ТО ВОЛС АТС BRN 4008ВК 1Q2017","Siberia"),
new Tuple<string, string, string,string>("987","4514404985","SIB ТО ВОЛС АТС KEM 1АТСВОЛС 1Q2017","Siberia"),
new Tuple<string, string, string,string>("988","4514405006","SIB ТО ВОЛС АТС KRS 06022012 1Q2017","Siberia"),
new Tuple<string, string, string,string>("989","4514405025","SIB ТО ВОЛС АТС NSK 0112НСКВК 1Q2017","Siberia"),
new Tuple<string, string, string,string>("990","4514405040","SIB ТО ВОЛС АТС OMS 1АТСВК2013 1Q2017","Siberia"),
new Tuple<string, string, string,string>("975","4514404460","SIB ТО ПРОЧЕЕ МАКРОС СПУТНИК 1Q2017","Siberia"),
new Tuple<string, string, string,string>("976","4514404524","SIB ТО ПРОЧЕЕ НОРИЛЬСК-ТЕЛЕКОМ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("977","4514404536","SIB ТО ПРОЧЕЕ РАДИОСВЯЗЬ  1Q2017","Siberia"),
new Tuple<string, string, string,string>("978","4514404610","SIB ТО ПРОЧЕЕ САЙТМОНТАЖ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("979","4514404643","SIB ТО ПРОЧЕЕ СЕВЕРНЫЕ ТЕЛРАДКОМ 1Q2017","Siberia"),
new Tuple<string, string, string,string>("982","4514405059","SIB ТО ПРОЧЕЕ СИБИНТЕК 1Q2017","Siberia"),
new Tuple<string, string, string,string>("980","4514404262","SIB ТО ПРОЧЕЕ ТЫШИК  1Q2017","Siberia"),
new Tuple<string, string, string,string>("981","4514404670","SIB ТО ПРОЧЕЕ ЦЕНТР ТЕЛ И СВЯЗИ  1Q2017","Siberia"),
new Tuple<string, string, string,string>("1005","4514416822","SIB ТО ПСКВ САЙТМОНТАЖ 01.2017","Siberia"),
new Tuple<string, string, string,string>("1006","4514476258","SIB ТО ПСКВ САЙТМОНТАЖ 02.2017","Siberia"),

									};

				var ural = listTuple.Where(t => t.Item4 == "Ural");
				var siberia = listTuple.Where(t => t.Item4 == "Siberia");
				foreach (var item in ural)
				{
					var bytes3 = ExcelParser.EpplusInteract.CreateTORequest.CreateTORequestFile(int.Parse(item.Item1));
					StaticHelpers.ByteArrayToFile($@"C:\Temp\URAL\{item.Item2}.xlsm", bytes3);
				}

				foreach (var item in siberia)
				{
					var bytes3 = ExcelParser.EpplusInteract.CreateTORequest.CreateTORequestFile(int.Parse(item.Item1));
					StaticHelpers.ByteArrayToFile($@"C:\Temp\Siberia\{item.Item2}.xlsm", bytes3);
				}




				//using (Context context = new Context())
				//{

				//    DbTaskParams paramsdd = new DbTaskParams
				//                                {
				//                                    DbTask =
				//                                        context.DbTasks.FirstOrDefault(
				//                                            t => t.Name == "SendWIHPORDelRequests")
				//                                };
				//    var task = TaskFactory.GetTaskTest(paramsdd, context);
				//    task.Process();
				//}
			}
			catch (Exception exc)
			{

			}

		}


		[TestMethod]
		public void CreatePORsinSH()
		{






			using (Context context = new Context())
			{


				var cachedShAVR = context.ShAVRs.ToList().Where(AVRRepository.Base).ToList();
				var pors = context.AVRPORs.OrderByDescending(p => p.PrintDate).ToList().Select(por => new
				
				{
					Id = por.Id,
					PrintDate = por.PrintDate,
					UserName = por.UserName,
					SubContractorName = por.SubContractorName,
					WorkStart = por.WorkStart,
					WorkEnd = por.WorkEnd,
					Project = por.Project.Name,
					AVR = por.AVRId

				}).ToList();


				var result = new List<string>();
				foreach (var por in pors.OrderByDescending(p => p.PrintDate).ToList())
				{
					if (!string.IsNullOrEmpty(por.AVR))
					{
						var shAvr = cachedShAVR.FirstOrDefault(a => a.AVRId == por.AVR);
						if (shAvr != null)
							if (shAvr.PorAccesible)
								result.Add(por.AVR);
					}
					else
					{
						result.Add(por.AVR);
					}

				}

			}
			//    DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PORtoSH") };
			//    var task = TaskFactory.GetTask(paramsdd, context);
			//    task.Process();
			//}
			var b1 = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(10206, false);
			File.WriteAllBytes(@"c:\temp\205915.xlsx", b1);

			var b2 = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(10207, false);
			File.WriteAllBytes(@"c:\temp\205916.xlsx", b2);

			var b3 = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(10208, false);
			File.WriteAllBytes(@"c:\temp\206564.xlsx", b3);

			var b4 = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(10209, false);
			File.WriteAllBytes(@"c:\temp\206565.xlsx", b4);

			//var b = ExcelParser.EpplusInteract.CreateTOPOR.CreatePorFile(581,true);
			//File.WriteAllBytes(@"c:\temp\toPORTest.xlsx", b);

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

				try
				{
					SHInteract.Handlers.Solaris.UploadTOPOR.Handle(@"C:\Temp\test.gif", @"C:\Temp\test.gif", "SIB ТО ПРОЧЕЕ САЙТМОНТАЖ  ИЮНЬ 2016");
				}
				catch (Exception exc)
				{

					throw;
				}

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
			try
			{
				using (Context context = new Context())
				{

					DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "ActToSHHandler") };
					var task = TaskFactory.GetTaskTest(paramsdd, context);
					task.Process();
				}
			}
			catch (Exception exc)
			{

				throw;
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
		public void SendToSubcontractorHandler()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendToSubcontractorHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}
		[TestMethod]
		public void SendWIHGRRequestTest()
		{



			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "SendWIHGRRequest") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}

		[TestMethod]
		public void TestTwoMonthRange()
		{
			DateTime? trueDate1 = new DateTime(2016, 1, 1);
			DateTime? trueDate2 = new DateTime(2016, 2, 1);
			DateTime? trueDate3 = new DateTime(2016, 2, 28);

			DateTime? falseDate1 = new DateTime(2016, 3, 1);
			DateTime? falseDate2 = new DateTime(2016, 4, 1);

			var now = new DateTime(2016, 4, 1);

			Assert.IsTrue(trueDate1.TwoMonthRange(now));
			Assert.IsTrue(trueDate2.TwoMonthRange(now));
			Assert.IsTrue(trueDate3.TwoMonthRange(now));
			Assert.IsFalse(falseDate1.TwoMonthRange(now));
			Assert.IsFalse(falseDate2.TwoMonthRange(now));



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
		public void EmptyAVRDitstHandlerManagers()
		{
			using (Context context = new Context())
			{



				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "EmptyAVRDitstHandlerManagers") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				var should1 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(1));
				var should2 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(2));
				var should3 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(3));
				var should4 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(4));
				var should5 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(5));
				var should6 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(6));
				var should7 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(7));
				var should8 = TaskManager.TaskManager.Instance.ShouldStart(paramsdd.DbTask, DateTime.Now.AddDays(8));

				task.Process();
			}
		}
		[TestMethod]
		public void EmptyAVRDistHandlerResponsibles()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "EmptyAVRDistHandlerResponsibles") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}
		[TestMethod]
		public void NewAVRDistrHandler()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "NewAVRDistrHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}
		[TestMethod]
		public void PutevieNotifierHandler()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PutevieNotifierHandler") };
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
		public void PutevieImportHandler()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "PutevieImportHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}


        #region PutevoiImport
        [TestMethod]
        public void PutevoiImportTest()
		{
			var now = DateTime.Now;
			var waylistImport = new List<WaylistImport>();
			//var waylistRequiredImport = new List<WaylistRequiredImport>();
			// var mails = Processor.GetMails(new List<string> { "#путевой#" });
			//TaskParameters.TaskLogger.LogDebug($"Писем : {mails.Count}");

			//foreach (var mail in mails)
			using (var context = new Context())
			{

				try
				{


					// берем первый файл из письма. условились что на одну машину и одну дату одно письмо
					// из этого первого файла узнаем машину и дату и пытаемся их обработать
					bool mailHandled = true;
					//var testAttachments = mail.Attachments.Where(a => a.File.Contains("_") && Path.GetExtension(a.File).ToUpper() == ".XLSX");
					//if (!testAttachments.Any())
					//{
					//     AddErrorEmailMessage(mail);
					//    continue;
					// }
					//TaskParameters.TaskLogger.LogDebug($"{mail.Author} - {Path.GetFileName(testAttachments.FirstOrDefault().File)}");



					var filePath = @"C:\Temp\A976ЕК50_03.2017 .xlsx";
					var testAttachments = new List<string> { filePath };
					var fileName = Path.GetFileNameWithoutExtension(testAttachments.FirstOrDefault());
					var parts = fileName.Split(new char[] { '_' });
					if (parts.Count() != 2)
					{
						//TaskParameters.TaskLogger.LogError($"неверный формат письма");
						// Processor.MoveToUnhandled(mail.ConversationId);
						//continue;
						return;
					}
					var carPart = parts[0];
					var datePart = new string(parts[1].Where(c => Char.IsDigit(c)).Take(7).ToArray());
					// машину определяем по первым трем цифрам
					string carNum = GetCarNum(carPart);
					bool letBool = true;
					bool digbool = true;




					var shCar = context.ShCars.FirstOrDefault(c => c.CarId.Contains(carNum));
					if (shCar == null)
					{
						//TaskParameters.TaskLogger.LogError($"Авто '{carNum}' не найдено");
						//continue;
						return;

					}
					DateTime date;
					if (
						!DateTime.TryParseExact(
							datePart,
							"MMyyyy",
							System.Globalization.CultureInfo.InvariantCulture,
							System.Globalization.DateTimeStyles.None,
							out date))
					{
						return;
					}

					var wayListName = GetWaylistName(carNum, date);
					if (!string.IsNullOrEmpty(wayListName))
					{
						PutevoiContent waylistContent = new PutevoiContent();
						bool found = false;
						foreach (var file in testAttachments)
						{
							if (TryReadPutevoi(filePath, out waylistContent))
							{
								found = true;
								break;
							}
						}
						if (!found)
						{
							// шлем ошибку
							// Перемещаем письмо
							//AddErrorEmailMessage(mail);
							return;
						}

						var shWaylist = context.ShWaylists.FirstOrDefault(w => w.Waylist == wayListName);
						if (shWaylist == null)
						{
                            //Изначально импорт был только на Create

                            var import = new WaylistImport();

                            import.Car = shCar.CarId;
                            import.Name = wayListName;
                            import.Date = now;
                            import.MeterStart = waylistContent.MeterStart;
                            import.MeterEnd = waylistContent.MeterEnd;
                            import.Refill = waylistContent.Refill;
                            waylistImport.Add(import);
                            //continue;
                            return;
						}

                        // было бы неплохо для существующих прогрузить этот пробег. но для этого пришлось сделать импорт еще и на апдэйт
                        var importMet = new WaylistImport();
                        importMet.Name = wayListName;
                        importMet.MeterStart = waylistContent.MeterStart;
                        importMet.MeterEnd = waylistContent.MeterEnd;
                        importMet.Refill = waylistContent.Refill;
                        waylistImport.Add(importMet);



                        //List<string> fileFields = new List<string> { "Файл1", "Файл2", "Файл3", "Файл4" };
                        //int index = 0;
                        //StringBuilder results = new StringBuilder();
                        //foreach (var attach in mail.Attachments)
                        //{
                        //    var field = fileFields[index];
                        //    var result = SHInteract.Handlers.Solaris.WayListUpload.Upload(
                        //        wayListName,
                        //        attach.FilePath,
                        //        field);
                        //    results.AppendLine(result);
                        //    // прогружаем файл
                        //    index++;
                        //}

                        //if (shWaylist.Required == "Yes")
                        //{
                        //    var import = new WaylistRequiredImport();
                        //    import.Name = wayListName;
                        //    waylistRequiredImport.Add(import);
                        //    continue;
                        //}




                        //if (mailHandled)
                        //{
                        //    Processor.MoveToSuccess(mail.ConversationId);
                        //    // ответить
                        //    Processor.SendMail(
                        //        new AutoMail
                        //        {
                        //            Email = DistributionConstants.EgorovEmail,
                        //            Body =
                        //                    $"Файлы по '{shCar.CarId}' за '{date.ToString("MM.yyyy")}' успешно прогружены.",
                        //            Subject = "Re. Путевые"
                        //        });
                        //}
                    }
				}
				catch (Exception exc)
				{
				   // TaskParameters.TaskLogger.LogError($"{exc.Message} - {exc.StackTrace}");
				}

			}
			//if (waylistImport.Count > 0)
			//    TaskParameters.ImportHandlerParams.ImportParams.Add(
			//        new ImportParams
			//        {
			//            ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1,
			//            Objects = new ArrayList(waylistImport)
			//        });
			//if (waylistRequiredImport.Count > 0)
			//    TaskParameters.ImportHandlerParams.ImportParams.Add(
			//        new ImportParams
			//        {
			//            ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2,
			//            Objects = new ArrayList(waylistRequiredImport)
			//        });

			//return true;
		}

	   

		
		#endregion

		[TestMethod]
		public void TestReadPutevie()
		 {
			string path = @"C:\Temp\B119HP_032017.xlsx";
			string path1 = @"C:\Temp\B119HP_022017.xlsx";
			var content = new TaskManager.Handlers.TaskHandlers.Models.Putevie.PutevieImportHandler.PutevoiContent();
			TaskManager.Handlers.TaskHandlers.Models.Putevie.PutevieImportHandler.TryReadPutevoi(path1, out content);

			 //using (Context context = new Context())
			 //{

			//    DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "FuelListNotifierHandler") };
			//    var task = TaskFactory.GetTaskTest(paramsdd, context);
			//    task.Process();
			//}
		}
		[TestMethod]
		public void FuelListNotifierHandler()
		{
			using (Context context = new Context())
			{

				DbTaskParams paramsdd = new DbTaskParams { DbTask = context.DbTasks.FirstOrDefault(t => t.Name == "FuelListNotifierHandler") };
				var task = TaskFactory.GetTaskTest(paramsdd, context);
				task.Process();
			}
		}
		[TestMethod]
		public void GetTable()
		{
			//UniReport.UniReportBulkCopy<MDClass> report = new UniReport.UniReportBulkCopy<MDClass>(@"C:\Users\ealgori\Documents\MasterData.xls");
			//// считали объекты из эксель файла
			//var objs = UniReport.Read<MDClass>();
			//List<MDClass2> md2Collect = new List<MDClass2>();
			//foreach (var obj in objs)
			//{
			//    var md2 = new MDClass2() { Code = obj.Code, Description = obj.Description, DescriptionEng = obj.DescriptionEng, Unit = obj.Unit };
			//    md2.CodeEnd = obj.Code.CUnidecode();
			//    md2Collect.Add(md2);
			//}
			//// конвертируем их в дататэйбл, чтобы воспользоваться существующим функционалом
			//var dataTable = md2Collect.ToDataTable();
			//var bytes = NpoiInteract.DataTableToExcel(dataTable);
			//CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Users\ealgori\Documents\MasterData2.xls", bytes);

		}

		private class MDClass
		{
			public string Code { get; set; }
			public string Description { get; set; }
			public string Unit { get; set; }
			public string DescriptionEng { get; set; }
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
			//RedemptionMailProcessor processor2 = new RedemptionMailProcessor("VCSRS");
			//var mails2 = processor2.GetMails(new List<string> { " created" });
			RedemptionMailProcessor processor = new RedemptionMailProcessor("VCSRS");
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
			RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
			var mail = new AutoMail();
			mail.Email = "aleksey.gorin@ericsson.com";
			mail.Body = "test";
			processor.SendMail(mail);
		}
		[TestMethod]
		public void TestRedemption2()
		{

			// var rSession = new RDOSession();
			// //rSession.Logon();
			// // UserName подменяется в Options.cs в конце.

			// rSession.Logon();
			// var box = rSession.GetSharedMailbox("VIMPELCOM ADMIN 02");

			// RDOFolder outbox = box.GetDefaultFolder(Redemption.rdoDefaultFolders.olFolderOutbox);
			// RDOMail mail = outbox.Items.Add(rdoItemType.olMailItem);

			// mail.Recipients.Add("aleksey.gorin@ericsson.com");
			//// mail.SenderEmailAddress = "vimpelcom.admin.02@ericsson.com";
			// mail.SentOnBehalfOfName = "vimpelcom.admin.02@ericsson.com";

			// mail.Subject = "test";
			// mail.Send();


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
			RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
			AutoMail mail = new AutoMail();

			mail.Email = ("aleksey.gorin@ericsson.com");//;aleksey.chekalin@ericsson.com");
			mail.Subject = "TestSubject";
			processor.SendMail(mail);

		}



		[TestMethod]
		public void TestCircuit()
		{

			TimeSpan span = new TimeSpan();
			DateTime now = DateTime.Now;
			DateTime startDate = DateTime.Now;
			var lastWork = DateTime.Now;
			// если непонятное значение в таймспан, то сработать
			if (TimeSpan.TryParse("3", out span))
			{
				for (int i = 0; i < 20; i++)
				{
					now = now.AddDays(1);

					// если непонятно когда обновлялось, то сработать
					if (lastWork != null)
					{
						var nextForecastDate = startDate;

						// считаем теоретическое время след срабатывания
						while (nextForecastDate < now)
						{
							nextForecastDate = nextForecastDate.Add(span);
						}

						var lastForecastDate = nextForecastDate.Add(-span);

						if (lastWork < lastForecastDate)
						{
							// сейчас
						}
						else
						{
							// в след раз
						}



					}
					else // не выполнялось еще
					{
						// сейчас
					}
				}
			}
			else// кривой таймспан
			{

			}

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
