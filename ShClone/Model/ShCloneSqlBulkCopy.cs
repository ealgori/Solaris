using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NLog;
using Ionic.Zip;
using System.Diagnostics;
using System.Data;
using System.Security.AccessControl;
using DbModels.DataContext;
using ShClone.Comparers;
using ShClone.UniReport;
using DbModels.DomainModels.ShClone;
using TaskManager.Handlers.TaskHandlers.Models.WIH;
using TaskManager.TaskParamModels;


namespace ShClone.Model
{
    public class SHCloneBulkCopy : ShClone.Abstract.ShClone_prototype
    {
        #region Singleton
        private static volatile SHCloneBulkCopy instance;
        private static object syncRoot = new Object();

        private SHCloneBulkCopy() { }

        public static SHCloneBulkCopy Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            ClearFolder(ShCloneParams.FilesPath);
                            instance = new SHCloneBulkCopy();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion





        public string WorkDirectory { get; set; }
        /// <summary>
        /// Этой переменной будем обозначать необходимость очистки папки, после окончания работы тасков. 
        /// </summary>
        public bool NeedToCleanFolder { get; set; }

        #region file process
        /// <summary>
        /// Ищем все необходимые файлы. Не помеченные в базе как рекваред не обязательны
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<string> GetFiles(Context context)
        {
            List<ShFiles> files = null;
            List<string> readyFiles = new List<string>();


            files = context.ShFiles.ToList();



            if (Directory.Exists(ShCloneParams.FilesPath))
            {
                // перебераем все сх файлы из базы
                foreach (var shfile in files)
                {
                    var file = Directory.EnumerateFiles(ShCloneParams.FilesPath, shfile.Mask).FirstOrDefault();
                    // если в папке не оказывается нужного файла
                    if (file != null)
                    {
                        //// уже непонимаю зачем этот код
                        //string fileName = file.ToString();//.OrderBy(fil => fil, new Extentions.SHFileCompare()).LastOrDefault();

                        //if (!string.IsNullOrEmpty(fileName))
                        //{
                        readyFiles.Add(file);
                        LogInfo(Path.GetFileName(file));
                        //}
                        //else
                        //{

                        //}
                    }
                    // и если этот файл обязателен, то ничего не переносим. не начинаем роботу
                    else
                    {
                        if (shfile.Required)
                        {
                            LogInfo("Cannot find file of type:" + shfile.Name);
                           // var task = (new WIHTrashCleaner(new TaskParameters())).Handle();
                            return null;
                        }
                    }
                }


                return readyFiles;
            }
            else
            {
                LogError("Папка не существует: " + ShCloneParams.FilesPath);
                return null;
            }
        }
        /// <summary>
        /// Перемещаем архивы в отдельную папку
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public bool MoveFiles(List<string> fileList)
        {
            try
            {
                string workPath = Path.Combine(ShCloneParams.FilesPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(workPath);
                //try
                //{
                //    DirectoryInfo info = new DirectoryInfo(workPath);

                //    DirectorySecurity security = info.GetAccessControl();

                //    security.AddAccessRule(new FileSystemAccessRule(System.Environment.UserName, FileSystemRights.Modify, AccessControlType.Allow));

                //    info.SetAccessControl(security);
                //}
                //catch (System.Exception ex)
                //{
                //    LogInfo(ex.Message + " access grand error" );

                //}
                //try
                //{
                //    System.IO.File.WriteAllLines(Path.Combine(ShCloneParams.FilesPath, "file2.txt"), new string[] { "HellowWorld" });
                //}
                //catch
                //{
                //    LogError("Файл в " + ShCloneParams.ServerPath + " не создан");
                //}
                //try
                //{
                //    System.IO.File.WriteAllLines(Path.Combine(workPath,"file1.txt"), new string[] { "HellowWorld" });
                //}
                //catch
                //{
                //    LogError("Файл в " + workPath + " не создан");
                //}

                WorkDirectory = workPath;

                foreach (var file in fileList)
                {

                    File.Move(file, Path.Combine(WorkDirectory, Path.GetFileName(file)));
                }
                return true;
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Распаковка всех файлов полученных путем экспорта их сх
        /// </summary>
        /// <returns></returns>
        public bool UnZipFiles()
        {
            try
            {
                while (true)
                {
                    string fileName = Directory.GetFiles(WorkDirectory, "*.zip").FirstOrDefault();
                    if (string.IsNullOrEmpty(fileName))
                    {
                        break;
                    }
                    else
                    {
                        using (ZipFile zip1 = ZipFile.Read(fileName))
                        {
                            foreach (ZipEntry e in zip1)
                            {
                                e.Extract(WorkDirectory, ExtractExistingFileAction.OverwriteSilently);
                            }
                        }
                        LogInfo("Unpacked: " + Path.GetFileName(fileName));
                        File.Delete(fileName);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                LogError(ex.Message);
                return false;
            }
        }
        #endregion



        /// <summary>
        /// Основная процедура. в бесконечном цикле
        /// </summary>
        public override void DoWork()
        {


            //var ri = new RI();
            //var box=ri.GetMailBoxFor("technical.box.for.solaris@ericsson.com");
            //var testMails = box.GetMails(new List<string> { "Утверждение"});
            //Console.WriteLine(testMails.Count);

            //Ищем файлы
            //Перемещаем их в папку
            //Распаковываем
            //Находим необходимые поля при помощи рефлекшн из типа данных таблицы
            //Читаем первый файл и находим колонки в которых расположены эти поля
            //Читаем все файлы с группировкой по типу
            //Планируем обновление и сидим в планировщике пока не придет его время (папку в схклонпараметрах)
            //Выполняем все эти запросы по очереди.

            LogWarn("-------------------ShClone Started:---------------------------------");

            while (true)
            {
                if (TaskManager.TaskManager.Instance.Ready)
                {
                    if (NeedToCleanFolder)
                    {
                        NeedToCleanFolder = false;
                        ClearFolder(ShCloneParams.FilesPath, true);

                    }
                    else
                    {
                        try
                        {

                            using (Context context = new Context())
                            {
                                context.Configuration.AutoDetectChangesEnabled = false;
                                LogInfo("-------------------ShClone Update Start Work:-----------------------");
                                var files = GetFiles(context);
                                if (files != null)
                                {
                                    Console.Clear();
                                    if (MoveFiles(files))
                                    {
                                        if (UnZipFiles())
                                        {
                                            var objects = PrepareQueries(context);
                                            if (objects != null)
                                            {
                                                TaskManager.TaskManager.Instance.SheduleDbUpdate(objects);
                                                NeedToCleanFolder = true;
                                            }
                                            else
                                            {
                                                LogError("Empty object list. Nothing to update");
                                            }
                                            ClearFolder(ShCloneParams.FilesPath,true);
                                           // ClearFolder(ShCloneParams.)
                                        }
                                    }

                                }
                                TaskManager.TaskManager.Instance.SheduleDbFreeTasks();
                                LogInfo("-------------------ShClone Update End Work:-------------------------");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            LogError("SH_CLONE_ERROR:" + ex.Message);
                            LogError("CALLSTACK: <<<<<<-----{" + ex.StackTrace + "}------->>>>>>>");
                            if (ex.InnerException != null)
                                if (!string.IsNullOrEmpty(ex.InnerException.Message))
                                    LogError("SH_CLONE_SUBERROR:" + ex.Message);
                        }
                    }
                }
                System.Threading.Thread.Sleep(ShCloneParams.DoWorkInterval);

            }
        }



        /// <summary>
        /// Чтение распакованных файлов и подготовка запросов
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<DataTable> PrepareQueries(Context context)
        {


            List<DataTable> Objects = new List<DataTable>();
            var SHFiles = context.ShFiles.ToList();
            foreach (var shFile in SHFiles)
            {
                var files = Directory.GetFiles(WorkDirectory, shFile.Mask).ToList();//.OrderBy(fi => fi, new GroupFileCompare()).ToList();
                if (files.Count == 0&&shFile.Required)
                {
                    LogError(string.Format("Cannot find file of type {0} after unpacking({1})", shFile.Name, shFile.Mask));
                    return null;
                }
                if (files.Count > 0)
                {
                    UniReportBulkCopy uniReport = new UniReportBulkCopy(shFile.TableName, shFile.TypeName, files, context, Objects);
                    if (!uniReport.ReadFiles())
                        return null;
                }


            }

            return Objects;


        }

        public static void ClearFolder(string folder, bool delFolders=false)
        {
            var inFilesDirectory = new DirectoryInfo(folder);

            try
            {



                if (inFilesDirectory.Exists)
                {
                    //LogInfo("Очищаем папку:" + ShCloneParams.FilesPath);
                    var fileList = inFilesDirectory.EnumerateFiles();
                    foreach (var file in fileList)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch
                        {

                        }
                    }

                }
                if (delFolders)
                {

                    var directories = inFilesDirectory.EnumerateDirectories().OrderByDescending(d => d.CreationTime).Skip(2);
                    foreach (var directory in directories)
                    {
                        foreach (FileInfo file in directory.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in directory.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                        directory.Delete(true);
                    }


                }
            }
            catch (Exception ex)
            {

                //LogError("SH_CLONE_ERROR:" + ex.Message);
            }
        }

        #region Log
        private  void  LogInfo(string message)
        {
            if (logger!=null)
            logger.Info(message);
            Debug.WriteLine(message);
        }

        private   void LogWarn(string message)
        {
            if (logger != null)
                logger.Warn(message);
            Debug.WriteLine(message);
        }

        private  void LogError(string message)
        {
            if (logger != null)
            logger.Error(message);
            Debug.WriteLine(message);
        }
        #endregion

    }
}