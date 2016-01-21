using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace ExcelParser.Extentions
{
    public static class Extention
    {
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
