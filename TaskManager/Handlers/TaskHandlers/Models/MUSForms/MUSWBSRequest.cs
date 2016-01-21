//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.IO;
//using TaskManager.Service;
//using CommonFunctions.StaticHelper;
//using CommonFunctions.Extentions;

//namespace TaskManager.Handlers.TaskHandlers.Models.MUSForms
//{
//    public class MUSWBSRequest : ATaskHandler
//    {
//        public MUSWBSRequest(TaskParameters taskParameters) : base(taskParameters) { }
//        public override bool Handle()
//        {
//            var list = TaskParameters.Context.Database.SqlQuery<MUSWBSRequestModel>("MUS_WBSrequest").ToList();
            
//            List<PreparedMuses> PreparedMUSs = new List<PreparedMuses>();
//            List<SOW> SOWs = new List<SOW>();

            
//            foreach (var value in list)
//            {
//                FileInfo info = new FileInfo(TaskParameters.DbTask.TemplatePath);
//                using (EpplusService EXcelService = new EpplusService(info))
//                {

//                    try
//                    {
//                        Dictionary<string, string> dict = new Dictionary<string, string>();
//                        var paramDict = new Dictionary<string,object>();
//                            paramDict.Add("@SOW", value.SOW);

//                            var subList = StaticHelpers.GetStoredProcDataFromContext<CostsTable>(TaskParameters.Context, 
//                            "MUS_WBSrequest_Costs",
//                        paramDict);


//                            // TaskParameters.Context.Database.SqlQuery<CostsTable>("MUS_WBSrequest_Costs", "@SOW=" + value.SOW).ToList();
//                        // вставка таблицы
//                        EXcelService.InsertTableToPatternCellInWorkBook("TotalCost", subList.ToDataTable(typeof(CostsTable)),
//                            new EpplusService.InsertTableParams());

//                        // вставка одиночных значений
//                        foreach (var item in StaticHelpers.GetProperties(value))
//                        {
//                            dict.Add(item.Name, item.GetValue(value, null).ToString());
//                        }
//                        EXcelService.ReplaceDataInBook(dict);

//                        // сохранение
//                        string fileName = string.Format("MUS-WBS-{0}-{1}.xlsx",DateTime.Now.ToString("yyMMddHHmmss"), value.Element.RemoveBadChars() );
//                        string filePath = Path.Combine(TaskParameters.DbTask.ArchiveFolder, fileName);

//                        FileInfo info1 = new FileInfo(filePath);
//                        EXcelService.app.SaveAs(info1);



//                        PreparedMUSs.Add(new PreparedMuses() { MUSFileName = fileName, ToDay = DateTime.Now, WBSShName = value.WBS.RemoveBadChars(), MusType = "WBS", MacroRegion = value.MacroRegion });
//                        SOWs.Add(new SOW() { SOW_Name=value.SOW });
//                    }
//                    catch (Exception exc)
//                    {
//                        TaskParameters.TaskLogger.LogError(string.Format("Ошибка при формировании муса для {0}:{1}", value.Element, exc.Message));
//                    }
//                }

//            }

//            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { Objects = new System.Collections.ArrayList(PreparedMUSs), ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1 });
//            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { Objects = new System.Collections.ArrayList(SOWs), ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2 });
//            return true;
//        }
//    }
//    public class MUSWBSRequestModel
//    {
//        public string EarlyStartID { get; set; }
//        public string SuperiorWBS { get; set; }
//        public string WBSName { get; set; }
//        public string ProjectDefinition { get; set; }
//        public string ProjectName { get; set; }
//        public string CPMFullName { get; set; }
//        public string CPMName { get; set; }
//        public string CPMSignum { get; set; }
//        public string NowDate { get; set; }
//        public string ShipToParty { get; set; }
//        public string Element { get; set; }
//        public string SOW { get; set; }
//        public string WBS { get; set; }
//        public string MacroRegion { get; set; }
//    }

//    public class PreparedMuses
//    {
//        public string MUSFileName { get; set; }
//        public DateTime ToDay { get; set; }
//        public string WBSShName { get; set; }
//        public string MusType { get; set; }
//        public string MacroRegion { get; set; }
//    }

//    public class SOW
//    {
//        public string SOW_Name { get; set; }

//    }


//    public class CostsTable
//    {
//        public double TotalCost { get; set; }

//    }

//}

