using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.Util;
using System.Collections;
using ExcelParser.Extentions;


namespace ExcelParser.ExcelExport
{
    public class NPOIExcelExport
    {
        public static class ImportToSH
        {

           // private static Logger logger = LogManager.GetCurrentClassLogger();
            public static  byte[] ListToWorkbook<T>(List<T> data, int objectsPerPage)
            {
                NPOIReportService service = new NPOIReportService();

                // Узнать количество объектов в эррэй лист
                if (objectsPerPage <= 0)
                    objectsPerPage = 65500;
                int pageCount = data.Count / objectsPerPage + 1;

             
                for (int page = 0; page < pageCount; page++)
                {
                    var partDataList = data.Count > objectsPerPage ?
                        data.GetRange(page * objectsPerPage, objectsPerPage) : data;
                    var dataTable = (partDataList.ToDataTable<T>(typeof(T)));

                    try
                    {
                        service.FillReportData(dataTable, "Sheet"+page);
                    }
                    catch (System.Exception ex)
                    {
                       
                    }

                   

                }
                return service.GetBytes();
            }
        }
      
        public static byte[] GetBytes(HSSFWorkbook workbook)
        {
            using (var buffer = new MemoryStream())
            {
                workbook.Write(buffer);
                return buffer.GetBuffer();
            }
        }
    }
   
}

    