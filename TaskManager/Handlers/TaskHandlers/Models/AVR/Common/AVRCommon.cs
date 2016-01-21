using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.Common
{
    public static class AVRCommon
    {

        public static readonly string AcceptMask = "Утвердить заявку";
        public static readonly string RejectMask = "Отклонить заявку";
        private static readonly string AVRRequestsPath = @"\\RU00112284\Solaris AVR documentation\";
        public static string GetAVRArhivePath(string orderName)
        {
            orderName = orderName.RemoveIllegalFileNameCharacters();
            var path = Path.Combine(AVRRequestsPath,orderName);
            if(!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {

                    Console.WriteLine("Ошибка при создaнии папки по пути :"+ path);
                    return null;
                }
            }
            return path;
        }

        public static string GetVCRequestName(string avr)
        {
            return string.Format("{0}:{1}", avr, DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <param name="requestName"></param>
        /// <param name="orderBytes"></param>
        /// <returns></returns>
        public static string SaveOrderFile(string requestName, byte[] orderBytes)
        {
            string orderFilePath = Path.Combine(GetAVRArhivePath(requestName), string.Format("{0}{1}", requestName.RemoveIllegalFileNameCharacters(), ".xlsm"));
            try
            {
                CommonFunctions.StaticHelpers.ByteArrayToFile(orderFilePath, orderBytes);
                if (File.Exists(orderFilePath))
                {
                    return orderFilePath;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
