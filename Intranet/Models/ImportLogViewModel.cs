using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    /// <summary>
    /// Отображение одного файла импорта
    /// </summary>
    public class ImportLogViewModel
    {
        public int ImportFileId { get; set; }
        public DateTime ImportUploadedDate { get; set; }
        public string User { get; set; }
        public string FileName { get; set; }
        public string Processed { get; set; }
    }
    /// <summary>
    /// Логи каждого файла
    /// </summary>
    public class LogViewModel
    {
        public int LogId { get; set; }
        public string LogType { get; set; }
        public string Message { get; set; }
    }
}