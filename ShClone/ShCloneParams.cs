using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Reflection;
//using Intranet.Projects.ShClone.Tasks;

namespace ShClone
{
    public static class ShCloneParams
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
      
    
        ///// <summary>
        ///// Флаг того, что запланировано обновление базы
        ///// </summary>
        // public static bool SCHEDULEDDBUPDATE { get; set; }
       
        


        /// <summary>
        /// Время когда должно произойти обновление базы
        /// </summary>
         public static DateTime? SCHEDULEDDBUPDATETIME { get; set; }


        /// <summary>
        /// Интервал через который сх клон будет проверять наличие файлов и выполнять рутинные операции.
        /// Надо учесть, что пока операция не завршиться, этот отчет заново не пойдет.
        /// </summary>
        public static int DoWorkInterval = 25 * 1000;
        /// <summary>
        /// интервал который дается пользователям перед обновлением базы.
        /// </summary>
        public static double DbUpdateTimeoutMinuties = 0.01 ;
        //public static string ServerPath = HttpRuntime.AppDomainAppPath;
        public static string ServerPath = Assembly.GetExecutingAssembly().CodeBase;
       // public static string FilesPath = @"//E768B599F0AF1A.ericsson.se/SOLInFiles/";//System.IO.Path.Combine(ServerPath, @"InFiles");

        public static string FilesPath = @"\\RU00112284\InFiles\1721";
        /// <summary>
        /// Максимальное количество значение в инсерт и апдэйт запросе
        /// </summary>
        public static int MaxValuesPerQuery = 750;
        
        
    }
}