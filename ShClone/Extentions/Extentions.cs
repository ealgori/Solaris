using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel;

namespace ShClone.Extentions
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


        public static DataTable ToDataTable<T>(this IList<T> data, Type type)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type); 
            DataTable table = new DataTable(); 
            foreach (PropertyDescriptor prop in properties) 
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType); 
            foreach (var item in data) { DataRow row = table.NewRow(); 
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value; table.Rows.Add(row);
            } 
            return table;

        }

    }
}