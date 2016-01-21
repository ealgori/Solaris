using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using TaskManager.TaskParamModels;
using DbModels.DataContext;
using TaskManager.Handlers.ImportHandler.Models.ExcelExport;

using DbModels.DomainModels.DbTasks;

namespace TaskManager.Handlers.ImportHandlers
{
    public class ImportHandler : AImportHandler
    {

        public ImportHandler(TaskParameters parameters)
            : base(parameters)
        {

        }




        private string GetImportFileName(string nearlyName, int id)
        {

            return string.Format("{0}(id{1})(2).xls", nearlyName, id);
        }
        /// <summary>
        /// Метод получает последний импортид, для создания уникального айдишника, для отслеживания импортов в сх. Он находится в имени файла.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private int GetLastImportId(Context context)
        {
            var import = context.Imports.OrderByDescending(imp=>imp.Id).FirstOrDefault();
            if (import == null)
                return 0;
            else
                return import.Id;

        }
      

       public override bool Import()
        {
           int index = GetLastImportId(TaskParameters.Context);
           foreach (var importParam in TaskParameters.ImportHandlerParams.ImportParams)
	        {
                if (importParam.Objects != null && importParam.Objects.Count > 0)
                {
                    var files = NPOIExcelExport.ImportToSH.ImportParamsToListWorkbooks(importParam, TaskParameters.DbTask.MaxImportObjectPerFile);

                    foreach (var file in files)
                    {
                        string fileName = GetImportFileName(importParam.ImportFileNearlyName, index);
                        byte[] logByte;
                        TaskParameters.TaskLogger.LogDebug("импорт файла:" + fileName, file);
                        TaskParameters.Context.SHCImports.Add(new SHCImport() { FileName = fileName });
                        SurfaceImporter.SurfaceImporter importer = new SurfaceImporter.SurfaceImporter("SOLARIS");
                        if ((logByte = importer.PeformImport(file, fileName)) != null)
                        {
                            TaskParameters.TaskLogger.LogDebug("импорт файла успешен:" + fileName, logByte);

                        }
                        else
                        {
                            TaskParameters.TaskLogger.LogWarn("импорт файла НЕ успешен:" + fileName);

                        }

                        index++;
                    }
                }
                else
                {
                   // TaskParameters.TaskLogger.LogWarn("Импорт :" + importParam.ImportFileNearlyName+ ": нет объектов для прогрузки.");
                }
            TaskParameters.Context.SaveChanges();
	    }
           return true;
       
       }



       
    }
}