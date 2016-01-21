using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.EmailHandlers.Abstract
{
    public abstract class AEmailHandler
    {

        public TaskParameters TaskParameters { get; set; }
        public AEmailHandler(TaskParameters taskParameters)
        {
            TaskParameters = taskParameters;
        }

        public abstract bool SendMails();

        private List<string> GetFoldersAttach(EmailParams param)
        {
            List<string> filePaths = new List<string>();
            if (param.Directories == null || param.Directories.Count == 0)
                return null;
            foreach (var dir in param.Directories)
            {
                if (Directory.Exists(dir))
                {
                    var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
                    filePaths.AddRange(files);
                }
                else
                {

                    TaskParameters.TaskLogger.LogError("Папка не существует:" + dir);
                }
            }
            return filePaths;
        }

        private List<string> GetFilesAttach(EmailParams param)
        {
            List<string> filePaths = new List<string>();
            if (param.FilePaths == null || param.FilePaths.Count == 0)
                return null;
            foreach (var file in param.FilePaths)
            {
                if (File.Exists(file))
                {
                    filePaths.Add(file);
                }
                else
                {
                    throw new FileNotFoundException("Файл не существует:" + file);

                }
            }
            return filePaths;
        }

        private List<string> GetDataTablesAttach(EmailParams param)
        {
            List<string> filePaths = new List<string>();
            if (param.DataTables == null || param.DataTables.Count == 0)
                return null;
            var stamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var dt in param.DataTables)
            {
                #region DirCreate
                if (!Directory.Exists(param.TempSaveData))
                {

                    try
                    {
                        Directory.CreateDirectory(param.TempSaveData);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            if (!Directory.Exists(param.DefaultTempPath))
                            {
                                Directory.CreateDirectory((param.DefaultTempPath));
                            }
                            param.TempSaveData = param.DefaultTempPath;
                        }
                        catch (Exception exc)
                        {

                            throw exc;
                        }


                    }




                }
                #endregion

                var bytes = NpoiInteract.DataTableToExcel(dt.Value);
                string fileName = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(dt.Key), DateTime.Now.ToString("yyyyMMddHHmmss"), Path.GetExtension(dt.Key));
                string filePath = Path.Combine(param.TempSaveData, fileName);
                if (CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, bytes))
                {
                    filePaths.Add(filePath);
                }
                else
                {
                    throw new Exception("Cannot write: " + filePath);
                    //Throw(param, new Exception("Cannot write: " + filePath));
                }
            }
            return filePaths;
        }

        public List<string> GetAttachments(EmailParams param)
        {
            List<string> filePaths = new List<string>();
            var foldAttach = GetFoldersAttach(param);
            if (foldAttach != null)
            {
                filePaths.AddRange(foldAttach);
            }
            var fileAttach = GetFilesAttach(param);
            if (fileAttach != null)
            {
                filePaths.AddRange(fileAttach);
            }
            var dtAttach = GetDataTablesAttach(param);
            if (dtAttach != null)
            {
                filePaths.AddRange(dtAttach);
            }
            return filePaths;
        }


    }
}
