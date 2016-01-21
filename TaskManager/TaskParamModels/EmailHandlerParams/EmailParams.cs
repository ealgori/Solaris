using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TaskManager.TaskParamModels
{
    public class EmailParams
    {
        public readonly string DefaultTempPath = @"C:\Temp\Temp";
        /// <summary>
        /// тема письма
        /// </summary>
        public string Subject { get; set; }
        public List<string> Recipients { get; set; }
        public List<string> CCRecipients { get; set; }
        public List<string> BCCRecipients { get; set; }
        /// <summary>
        /// списко путей к папкам, файлы из которых следует отправить
        /// </summary>
        public List<string> Directories { get; set; }
        /// <summary>
        /// списко путей к файлам для отпавки
        /// </summary>
        public List<string> FilePaths { get; set; }
        /// <summary>
        /// тело письма
        /// </summary>
        public string HtmlBody { get; set; }
        /// <summary>
        /// имя отправки для отображения в логах
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ключ = имя файла + авитоматом подставится таймстэмп
        /// </summary>
        public Dictionary<string, DataTable> DataTables { get; set; }
        /// <summary>
        /// Путь временных файлов. если не указан, или недоступен то будет использоваться DefaultTempPath
        /// </summary>
        public string TempSaveData { get; set; }
        /// <summary>
        /// Позволяет производить отправку письма, даже если нет аттачментов, что в обычно ситуации не происходит
        /// </summary>
        public bool AllowWithoutAttachments { get; set; }


        public EmailParams(List<string> recipients, string subject, string tempSaveData = "", string name = "NoNameEmailSend")
        {
            Recipients = recipients;
            CCRecipients = new List<string>();
            BCCRecipients = new List<string>();
            Subject = subject;
            Directories = new List<string>();
            FilePaths = new List<string>();
            DataTables = new Dictionary<string, DataTable>();
            Name = name;
            if (string.IsNullOrEmpty(tempSaveData))
            {
                TempSaveData = DefaultTempPath;
            }
            else
            {
                TempSaveData = tempSaveData;
            }

        }


    }
}
