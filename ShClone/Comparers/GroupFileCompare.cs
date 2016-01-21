using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShClone.Comparers
{
    /// <summary>
    /// Компаратор. Сортирует значения по последней цифре перед расширением
    /// </summary>
    public class GroupFileCompare : IComparer<string>
    {

        public int Compare(string name1, string name2)
        {
            try
            {
                var parts1 = name1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                var parts2 = name2.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                string numstr1 = parts1[parts1.Count() - 2];
                string numstr2 = parts2[parts2.Count() - 2];

                int num1 = int.Parse(numstr1);
                int num2 = int.Parse(numstr2);

                return num1 == num2 ? 0 : (Math.Max(num1, num2) == num1 ? 1 : -1);
            }
            catch
            {
                NLog.LogManager.GetCurrentClassLogger().Error("Неожиданное имя файла: " + name1 + " или " + name2);
                return 0;
            }



        }
    }
}
