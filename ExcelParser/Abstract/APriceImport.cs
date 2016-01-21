using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;

using DbModels;
using System.Web;
using DbModels.DataContext;
using DbModels.Models;


namespace ExcelParser.Abstract
{
    public abstract class APriceImport:IDisposable
    {
         
        public Dictionary<List<string>,ICell> MappedHeaders{get;set;}
        public HSSFWorkbook WorkBook { get; set; }
       
        public Context Context { get; set; }
        public string FileName { get; set; }
      
        public ImportLogger.ImportLogger ImportLogger { get; set; }
        public bool Comparable { get; set; }
        public Project Project { get; set; }
   
        public APriceImport(HttpPostedFileBase fileBase, ImportLogger.ImportLogger importLogger,int projectId, bool comparable)
        {
            ImportLogger = importLogger;
            importLogger.SetImportFile(fileBase);
            this.Comparable = comparable;
            //ImportFile = new ImportFile();
            //ImportFile.Name = fileBase.FileName;
            //byte[] bytes = new byte[fileBase.InputStream.Length];
            //fileBase.InputStream.Read(bytes, 0, (int)fileBase.InputStream.Length);
            //ImportFile.File = bytes;
            //importLogger.AddImportFile(ImportFile);

            Context = new Context();
            WorkBook = ConnectExlFile(fileBase.InputStream);
            Project = Context.Projects.FirstOrDefault(pr => pr.Id == projectId);
            MappedHeaders = new Dictionary<List<string>,ICell>();
            FileName = fileBase.FileName;
          
        }

        // создали или нашли подрядчика
        public abstract SubContractor ProcessSubContractor();
        // создалил или нашли прайслист
        public abstract PriceList ProcessPriceList(SubContractor subContractor);
        // создали ревизию
        public abstract PriceListRevision ProcessPriceListRevision(PriceList priceList);
        // считали айтемы
        public abstract bool ProcessPriceListRevisionItems(PriceListRevision priceListRevision, SubContractor subContractor);

        protected bool MapHeaders(params List<string>[] fields)
        {
            if (WorkBook == null)
            {
                AddError("Произошла ошибка при открытии файла");
                return false;
            }
            else 
            try
            {
                ISheet sheet = WorkBook.GetSheetAt(0);
                if (sheet != null)
                {


                    // Оббегаем все строки в них
                    for (int rowNum = 0; rowNum < 100; rowNum++)
                    {

                        IRow row = sheet.GetRow(rowNum);
                        if (row != null)
                            for (int cellNum = 0; cellNum < row.LastCellNum; cellNum++)
                            {
                                // Оббегаем все ячейки в ряду
                                ICell cell = row.GetCell(cellNum);
                                if (cell != null)
                                {
                                    string cellValue = NpoiInteract.GetCellValue(cell);
                                    if (!string.IsNullOrEmpty(cellValue))
                                    {
                                        // Оббегаем все необходимые поля, и сравниваем содержимое с необходимым. Если совпадает, добавляем в список совпадений
                                        foreach (var field in fields)
                                        {
                                            foreach (var par in field)
                                            {

                                                if (cellValue.Trim() == par)
                                                {
                                                    MappedHeaders.Add(field, cell);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
            }
            catch(Exception exc)
            {
                AddError("Ошибка при маппинге файла "+ FileName+" "+ exc.Message);
                return false;
            }
            if (MappedHeaders.Keys.Count == fields.Count())
            {
                return true;
            }
            else
            {
                var mappedHeaderList = MappedHeaders.Keys.FirstOrDefault();
                if (mappedHeaderList == null)
                    mappedHeaderList = new List<string>();
                string absentFields = string.Join(", ", fields.Select(f => f.First()).ToList().Except(mappedHeaderList));
                AddError("Не все поля найдены в файле. Отсутствуют: "+absentFields);
                return false;

            }
           


            //logger.Error("Количество найденых полей не совпадает с ожидаемым!");
            //logger.Error("Ожидали:{0}; Нашли:{1}",
             //  string.Join(", ", RequiredField.Select(rf => rf.NameValue)),
            //    string.Join(", ", Fields.Select(f => f.Key)));
           // logger.Error(string.Join(",", RequiredField.Select(rf => rf.NameValue).Except(Fields.Select(f => f.Key))));

            return false;
            
        }

        private static HSSFWorkbook ConnectExlFile(Stream stream)
        {
            {
                try
                {

                   // using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        var hssfwb = new HSSFWorkbook(stream);
                        return hssfwb;
                    }
                }
                catch (System.Exception ex)
                {
                    return null;
                }
            }
        }

        protected ICell GetRightCell(ICell cell)
        {
            return cell.Row.GetCell(cell.ColumnIndex + 3);
        }


        public void Dispose()
        {
            Context.Dispose();
            
        }

        protected void AddError(string message)
        {
            ImportLogger.AddError(string.Format("{0}", message));
        }
        protected void AddMessage(string message)
        {
            ImportLogger.AddMessage(string.Format("{0}", message));
        }

        protected void AddWarning(string message)
        {
            ImportLogger.AddWarning(string.Format("{0}", message));
        }

        protected void AddError(string message, ICell cell)
        {
            if(cell!=null)

                ImportLogger.AddError(string.Format("{0}--{1}:{2}", message,cell.RowIndex,cell.RowIndex));
            else
                ImportLogger.AddError(string.Format("{0}", message));
        }
    }
}
