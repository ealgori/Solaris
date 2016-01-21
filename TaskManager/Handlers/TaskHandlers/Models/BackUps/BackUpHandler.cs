using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models
{
    public class BackUpHandler : ATaskHandler
    {
        public BackUpHandler(TaskParameters taskParams)
            : base(taskParams)
        {





        }


        public override bool Handle()
        {
            List<string> tables = TaskParameters.DbTask.Params.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();


            foreach (var table in tables)
            {
                try
                {
                    var dt = CommonFunctions.StaticHelpers.GetQueryDataTableFromContext(TaskParameters.Context, string.Format("select * from {0}", table), null);
                    var serv = new EpplusInteract.EpplusService();
                    serv.InsertTableToWorkSheet("table", dt, new EpplusInteract.EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = OfficeOpenXml.Table.TableStyles.Dark8 });
                    var datedPath = CommonFunctions.StaticHelpers.GetDatedPath(TaskParameters.DbTask.ArchiveFolder, false);
                    if (!Directory.Exists(datedPath))
                    {
                        Directory.CreateDirectory(datedPath);
                    }
                    var filePath = Path.Combine(datedPath, string.Format("{0}.xlsx", table));
                    serv.CreateFolderAndSaveBook(filePath);

                    if (!string.IsNullOrEmpty(TaskParameters.DbTask.EmailSendFolder))
                    {
                        var copyDatedPath = CommonFunctions.StaticHelpers.GetDatedPath(TaskParameters.DbTask.EmailSendFolder, false);
                        if (!Directory.Exists(copyDatedPath))
                        {
                            Directory.CreateDirectory(copyDatedPath);
                        }
                        var copyFilePath = Path.Combine(copyDatedPath, Path.GetFileName(filePath));
                        File.Copy(filePath, copyFilePath);
                    }

                }
                catch (Exception exc)
                {

                    TaskParameters.TaskLogger.LogError(exc.Message);
                }
            }
            return true;
        }
    }
}
