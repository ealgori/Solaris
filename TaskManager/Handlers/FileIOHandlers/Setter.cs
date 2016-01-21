


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TaskManager.Handlers.FileIOHandlers
{
    public static class Setter
    {
      
        
        /// <summary>
        /// создают экземпляр объекта и заполняют его свойства по хедерам. в хедерах указаны и названия свойств
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fileHeaders"></param>
        /// <returns></returns>
        public static bool SetValuesFromLine (string line, string[] separators, List<FileHeader> fileHeaders, ref object obj)
        {
            try
            {

                var parts = line.Split(separators, StringSplitOptions.None);
                //T obj = new T();
                foreach (var fileHeader in fileHeaders)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(fileHeader.PropName, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        string val = parts[fileHeader.Column].Trim();
                        object value = null;
                        //пусто рекваред поле. такого быть не должно. фильтруем.
                        if (string.IsNullOrWhiteSpace(val) && fileHeader.Required)
                        {
                            return false;
                        }

                        if (fileHeader.Type == typeof(decimal))
                        {
                            
                            value = decimal.Parse(val.Replace(" ", "").Replace(".", ""));
                        }
                        if (fileHeader.Type == typeof(decimal?))
                        {
                            value = decimal.Parse(val.Replace(" ", "").Replace(".", ""));
                        }

                        if (fileHeader.Type == typeof(DateTime))
                        {
                            value = DateTime.Parse(val);
                        }
                        if (fileHeader.Type == typeof(DateTime?))
                        {
                            DateTime _value;
                            if (DateTime.TryParse(val, out _value))
                                value = _value;

                        }

                        if (fileHeader.Type == typeof(string))
                        {
                            value = val;
                        }
                        prop.SetValue(obj, value, null);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
