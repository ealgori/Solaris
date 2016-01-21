//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.IO;

//namespace TaskManager.Handlers.ImportHandler.Models.ImportClasses
//{
//    /// <summary>
//    /// Поверхностный импортер. Использует копирование и перемещение файлов.
//    /// </summary>
//    public static class SurfaceImporter
//    {
//         #region realPaths

//        public const string LogDir = @"\\E768B599F0AF1A.ericsson.se\import$\1721\AutoImport\Log";
//        public const string ActiveDir = @"\\E768B599F0AF1A.ericsson.se\import$\1721\AutoImport";
//        public const string UploadDir = @"\\E768B599F0AF1A.ericsson.se\import$\1721\upload";
//        public const string CorrectDir = @"\\E768B599F0AF1A.ericsson.se\import$\1721\correct";
        

//        #endregion
//        #region UNrealPaths

//      /// <summary>
//      /// 
//      /// </summary>
//        //public const string ActiveDir = @"C:\import\AutoImport";
//        //public const string UploadDir = @"C:\import\upload";
//        //public const string CorrectDir = @"C:\import\correct";
//        //public const string LogDir = @"C:\import\Log";

//        #endregion

//        public const int UploadFileLoseInterval = 5000;
//        /// <summary>
//        /// Процесс импорта. Кидаем файл в нужную папку и ждем пока он пропадет. Пока он не пропадет, нам рыпаться некуда
//        /// </summary>
//        /// <param name="attachment"></param>
//        private static bool  Import(byte[] file, string fileName)
//        {
//            string filePath = string.Empty;
//            if (!Directory.Exists(UploadDir))
//            {
                
//                try
//                {
//                    Directory.CreateDirectory(UploadDir);
//                }
//                catch (System.Exception ex)
//                {
//                    return false;
//                }
//            }
//                filePath = string.Format("{0}/{1}", UploadDir, fileName);
//                File.WriteAllBytes(filePath, file);


//                bool fileLost = false;
//                while (!fileLost)
//                {
//                    try
//                    {
//                        fileLost = !File.Exists(filePath);
//                    }
//                    catch
//                    {

//                    }

//                    System.Threading.Thread.Sleep(UploadFileLoseInterval);
//                }
            
              
//            return true;
          

//        }
//        /// <summary>
//        /// Поиск лога. Один раз. если файл пропал, значит он либо в мусоре, либо прогрузился, и лог лежит в нужной папке
//        /// </summary>
//        /// <param name="attachment"></param>
//        private static byte[]  FindLog(string fileName)
//        {
        
//            try
//            {
//                string searchPath =
//                    System.IO.Path.Combine(
//                        CorrectDir,
//                        DateTime.Today.Month.ToString("D2"),
//                        DateTime.Today.ToString("yyyy.MM.dd")
//                                           );

//                if (!Directory.Exists(searchPath))
//                {
//                   // attachment.Result = new Result() { Status = Enums.Statuses.Fail, Message = string.Format(Messages.NoLogFile, attachment.File) };
//                    return null;
//                }

//                List<string> logFiles = Directory.GetFiles(searchPath,
//                    string.Format("*{0}*{1}*", fileName.Substring(0, fileName.Length - 5), ".log.html"), SearchOption.AllDirectories).ToList();
//                if (logFiles.Count > 0)
//                {
                  
//                    return File.ReadAllBytes(logFiles.FirstOrDefault());
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            catch (Exception exc)
//            {
//                return null;
//            }

//        }

//        public static byte[] PeformImport(byte[] file, string fileName)
//        {
//            if (Import(file, fileName))
//            {
//                byte[] logBytes;
//                if ((logBytes=FindLog(fileName))!=null)
//                    return logBytes;
//                else
//                    return null;
//            }
//            else
//                return null;

//        }

    

//    }
//}