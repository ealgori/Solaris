using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapReader;

namespace TestProject.GR_TO_Test.SapReader
{
    [TestClass]
    public class XlsxSapReaderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = @"C:\Temp\Logs\19.05.2016\zzpomon.xlsx";
            ISapReader reader = new XlsxSapReader(path);
            reader.Read();
            if(reader.Succeed)
            {
                var rows = reader.Rows;
            }
        }
    }
}
