using CommonFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.BackUps
{
    public class FolderBackUpHandler:ATaskHandler
    {
         public FolderBackUpHandler(TaskParameters taskParams)
            : base(taskParams)
        {

           



        }


         public override bool Handle()
         {
             List<Tuple<string, string>> folders = new List<Tuple<string, string>>();
             folders.Add(new Tuple<string, string>(@"\\RU00112284\Solaris documentation\", @"\\RU00112284\Solaris doc backup\"));
             folders.Add(new Tuple<string, string>(@"\\eemea.ericsson.se\ERUMODFS01\GroupECR\KAM_Vimpelcom and Regions\Solaris_delivery\Operations\SiteHandler_TO_Acts_archive", @"B:\Solaris act backup\"));
             folders.Add(new Tuple<string, string>(@"C:\p\", @"B:\P backup\"));
           


             //folders.Add(new Tuple<string, string>(@"", @""));
             //folders.Add(new Tuple<string, string>(@"", @""));
             foreach (var folder in folders)
             {
                 using (var progress = new ConsoleProgressBar())
                 {
                     Clone(folder.Item1, folder.Item2,progress);
                     //for (int i = 0; i <= 100; i++)
                     //{

                     //    progress.Report((double)i / 100);
                     //    Thread.Sleep(20);
                     //}
                     Console.WriteLine();
                 }
                 
             }
             
             
             return true;

         }

         private void Clone(string sourceFolder, string destFolder, IProgress<double> progress)
         {
             //throw new Exception();
             if (Directory.Exists(sourceFolder))
             {
                 // воссоздаем структуру
                 foreach (var path in Directory.EnumerateDirectories(sourceFolder, "*", SearchOption.AllDirectories))
                 {
                     Directory.CreateDirectory(path.Replace(sourceFolder, destFolder));
                 }
                 DirectoryInfo dirInfo = new DirectoryInfo(sourceFolder);
                 var files = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
                 int progressCount = 0;
                 int progressValue = 0;
                 int filesCount = files.Count();
                 foreach (var file in files)
                 {
                     try
                     { 
                     var pValue = ++progressCount * 100 / filesCount;
                     if (pValue != progressValue)
                     {
                         progressValue = pValue;
                         progress.Report(progressValue);
                     }
                     //if (token.IsCancellationRequested)
                     //{
                     //    token.ThrowIfCancellationRequested();
                     //    // return;
                     //}
                     var newPath = file.FullName.Replace(sourceFolder, destFolder);
                     if (File.Exists(newPath))
                     {
                         // если файл уже существует, надо найти все его копии и искать среди них
                         var subDirInfo = new DirectoryInfo(Path.GetDirectoryName(newPath));
                         var sameFileInfo = subDirInfo.EnumerateFiles(
                             string.Format("{0}*{1}", Path.GetFileNameWithoutExtension(newPath), Path.GetExtension(newPath)),
                             SearchOption.TopDirectoryOnly).Where(f => f.Length == file.Length);
                         // если среди копий нет файла с тем же размером, то назначаем новое ими и произойдет копирование
                         if (sameFileInfo.Count()==0)
                             newPath = Path.Combine(
                                 Path.GetDirectoryName(newPath),
                                 Path.GetFileNameWithoutExtension(newPath)
                                 + DateTime.Now.ToString("yyyyMMdd--HHmmss")
                                 + Path.GetExtension(newPath)
                             );
                         // Иначе, нам ничего копировать не надо
                         else
                         {
                             continue;
                         }


                     }
                     
                     
                         File.Copy(file.FullName, newPath, true);
                     }
                     catch(Exception exc)
                     {
                         TaskParameters.TaskLogger.LogError(exc.Message);
                     }
               
                     

                 }

             }

         }


    }

      
}
