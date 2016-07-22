using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.IO;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.SAT
{
    public class ActPrintToSHHandler:ATaskHandler
    {
        public ActPrintToSHHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            List<UpdateActImportModel> importModel = new List<UpdateActImportModel>();
            
            var readyActs =  TaskParameters.Context.ShActs
                .Where(a=>a.GetActLink&&string.IsNullOrEmpty(a.ActLink))
                .Join(TaskParameters.Context.SATActs, 
                    sa=>sa.Act, 
                    sat=>sat.ActName,
                    (sa,sat)=>new {sa,sat});
            foreach(var act in readyActs)
            {
                try
                {
                    var actBytes =   ExcelParser.EpplusInteract.CreateAct.CreateActFile(act.sat.Id);
                    if (actBytes == null)
                        continue;
                    else
                    {
                        if (Directory.Exists(TaskParameters.DbTask.ArchiveFolder))
                        {
                            var datedPath = CommonFunctions.StaticHelpers.GetDatedPath(TaskParameters.DbTask.ArchiveFolder);
                            if (!Directory.Exists(datedPath))
                            {
                                    Directory.CreateDirectory(datedPath);
                            }
                                    string fileName = string.Format("ACT-{0}.zip", act.sat.ActName);
                                    string filePath = Path.Combine(datedPath, fileName);
                                    CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, actBytes);

                                    UpdateActImportModel model = new UpdateActImportModel();
                                    model.ActLink = filePath;
                                    model.ActId = act.sat.ActName;

                                    importModel.Add(model);
                            
                        }
                    }
                }
                catch(Exception exc)
                {
                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при создании акта:{0}", exc.Message));
                }
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModel) });
            }
            return true;

        }

        private class UpdateActImportModel
        {
            public string ActId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string SendInformation { get; set; }
            public string SendStatus { get; set; }
            public string ActLink { get; set; }
            public string TO { get; set; }
        }
    }
}
