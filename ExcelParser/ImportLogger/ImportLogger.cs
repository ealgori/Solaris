using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels;

using System.Web;
using DbModels.DataContext;
using DbModels.Models.ImportFilesModels;

namespace ExcelParser.ImportLogger
{
    public class ImportLogger:IDisposable
    {
        private Context Context { get; set; }
        private Import Import { get; set; }
        private ImportFile ImportFile { get; set; }
        public ImportLogger(string username)
        {
            Context = new Context();
            Import = new Import();
            
            Import.User = username;
            Context.Imports.Add(Import);
            Context.SaveChanges();
        }
        public int GetCurrentImportFileId()
        {
            return ImportFile.Id;
        }

        public void AddError(string message)
        {
            AddImportFileLog(message, "ERROR");
        }

        public void AddWarning(string message)
        {
            AddImportFileLog(message, "WARN");
        }

        public void AddMessage(string message)
        {
            AddImportFileLog(message, "INFO");
        }

        private void AddImportFileLog(string message, string level )
        {
            ImportFileLog ifl = new ImportFileLog();
            ifl.Level = level;
           ifl.Message = message;
          // importFile = Context.ImportFiles.FirstOrDefault(ifi => ifi.Id == importFile.Id);
            //if (importFile.ImportFileLogs == null)
             //   importFile.ImportFileLogs = new List<ImportFileLog>();
            //importFile.ImportFileLogs.Add(ifl);
            ifl.ImportFile = ImportFile;
            Context.ImportFileLogs.Add(ifl);
            Context.SaveChanges();
        }

        public void AddImportFile(ImportFile importFile)
        {
            //if (Import.Files == null)
            //    Import.Files = new List<ImportFile>();
            //Import.Files.Add(importFile);
            importFile.Import = Import;
            Context.SaveChanges();
           

        }

        public void SetImportFile(HttpPostedFileBase fileBase)
        {
            ImportFile = new ImportFile();
            ImportFile.Name = fileBase.FileName;
            byte[] bytes = new byte[fileBase.InputStream.Length];
            fileBase.InputStream.Read(bytes, 0, (int)fileBase.InputStream.Length);
            ImportFile.File = bytes;
            ImportFile.Import = Import;
            Context.ImportFiles.Add(ImportFile);
            Context.SaveChanges();
        }
        public void SetSuccessImportFile()
        {

          
            ImportFile.Success = true;
            Context.SaveChanges();


        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
