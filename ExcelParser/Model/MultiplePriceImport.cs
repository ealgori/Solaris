using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ExcelParser.Model
{
    public class MultiplePriceImport:IDisposable
    {
      
        public ImportLogger.ImportLogger ImportLogger { get; set; }
        public MultiplePriceImport(string username)
        {
          
            ImportLogger = new ImportLogger.ImportLogger(username);
        }

        public void Process(HttpFileCollectionBase fileCollection, int projectId, string userName, bool comparable)
        {
            if (fileCollection != null && fileCollection.Count > 0)
            {
                for (int fileCount = 0; fileCount < fileCollection.Count; fileCount++)
                {
                    
                    using (PriceImport import = new PriceImport(fileCollection[fileCount], projectId, ImportLogger, comparable))
                    {

                        import.Process(userName);
                          
                        
                    }
                }
            }
            
        }

        public void Dispose()
        {
            ImportLogger.Dispose();
        }
    }
}
