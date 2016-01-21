using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel;

namespace CommonFunctions.Extentions
{
    public static class Extentions
    {
        //public static DataTable ToDataTable<T>(this IList<T> data, Type type)
        //{
        //    PropertyDescriptorCollection props =
        //        TypeDescriptor.GetProperties(type);
        //    DataTable table = new DataTable();
        //    for (int i = 0; i < props.Count; i++)
        //    {
        //        PropertyDescriptor prop = props[i];
        //        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //        //table.Columns.Add(prop.Name, prop.PropertyType);
        //    }
        //    object[] values = new object[props.Count];
        //    foreach (T item in data)
        //    {
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            try
        //            {
        //                values[i] = props[i].GetValue(item);
        //            }
        //            catch (System.Exception ex)
        //            {

        //            }

        //        }
        //        table.Rows.Add(values);
        //    }
        //    return table;
        //}
        /// <summary>
        /// Удаляем нежелательные символы из строки
        /// Используется в названиях мус форм(удаление дефиса из номера сайта)
        /// И тп.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static String RemoveBadChars(this string text)
        { 
            List<string> CharList = new List<string>();
            CharList.Add("-");
            CharList.Add("`");
            CharList.Add("~");
            CharList.Add("\\");
            CharList.Add("/");
            foreach (var ch in CharList)
            {
                text = text.Replace(ch, string.Empty);
            }
            return text;
        }
        public static DataTable ToDataTable<T>(this IList<T> data, Type type)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        /// <summary>
        /// Специализированная функция. только для записи в эксель. для других операций не использовать. Изменяет все типы в дэйтатэйбл на стринг
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable ToDataTableString<T>(this IList<T> data, Type type)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, typeof(string));
            foreach (var item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    if (prop.PropertyType.GetType() == typeof(DateTime) || prop.PropertyType.GetType() == typeof(DateTime?))
                    {
                        var val = prop.GetValue(item);
                        if (val != null)
                        {
                            row[prop.Name] = ((DateTime)val).ToString("dd-MM-yyyy");
                        }
                    }
                    else
                        row[prop.Name] = (prop.GetValue(item) ?? "").ToString();
                table.Rows.Add(row);
            }
            return table;

        }

    }
}