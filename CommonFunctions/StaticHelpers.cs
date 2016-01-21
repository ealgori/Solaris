using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using System.Reflection;
using NPOI.HSSF.UserModel;
using System.IO;
using NLog;

using System.Data.Entity;
using System.Globalization;
using FastMember;
using DbModels.DataContext;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.Xml;

namespace CommonFunctions.StaticHelper
{ 
    public static class StaticHelpers
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Возвращает значение из ячейки внезависимости от типа в стринг формате
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
  public static  MemoryStream ReadFileToMemoryStream(string filePath)
        {
            
            MemoryStream ms = new MemoryStream();
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
            ms.Write(bytes, 0, (int)file.Length);
            file.Close();
           // ms.Close();
            return ms;
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        public static string GetCellValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType.ToString())
                {
                    case "BOOLEAN":
                        return cell.BooleanCellValue.ToString();

                    case "NUMERIC":
                        return cell.NumericCellValue.ToString();

                    case "STRING":
                        return (cell.StringCellValue.ToString());

                    case "BLANK":
                        return string.Empty;

                    case "ERROR":
                        return cell.ErrorCellValue.ToString();

                    case "DATE":
                        return cell.DateCellValue.ToString();

                    // CELL_TYPE_FORMULA will never occur
                    case "FORMULA":
                        return string.Empty;

                }
            }
            return string.Empty;
        }
        public static string GetCellValueExt(ICell cell, Type type)
        {
            if (cell != null)
            {
                //!!!! исправить. для неналовских типов дефолт значение обязательно
                if (string.IsNullOrEmpty(GetCellValue(cell)))
                {

                    return null;
                }
                else
                {
                    string cellValue = string.Empty;
                    var @switch = new Dictionary<Type, Action> {
                    { typeof(DateTime), () => {
                        // тип дэйт тайм. значение должно быть обязательно.
                        try
                        {
                            cellValue= cell.DateCellValue.ToString("MM-dd-yyyy");
                        }
                        catch
                        {
                            cellValue = GetMinDateString();
                        }
                    }
                    },
                     { typeof(DateTime?), () => {
                         // в строке будет дата
                         
                         try
                         {
                             cellValue = cell.DateCellValue.ToString("MM-dd-yyyy");
                         }
                         catch (System.Exception ex)
                         {
                             // Parse date and time with custom specifier.

                             cellValue = GetMinDateString();
                         }
                         
                         
                         
                     } },
                    { typeof(decimal), () => { cellValue= cell.NumericCellValue.ToString();} },
                     { typeof(decimal?), () => { 
                         try
                         {
                             if (cell.CellType == CellType.NUMERIC)
                                 cellValue = cell.NumericCellValue.ToString();
                             else
                                 cellValue = cell.RichStringCellValue.ToString();
                         }
                         catch (System.Exception ex)
                         {
                         
                                cellValue = "";
                         	
                         }
                     }
                         },
                           { typeof(bool?), () => {
                         // в строке будет дата
                         
                         try
                         {
                             cellValue = cell.BooleanCellValue.ToString();
                         }
                         catch (System.Exception ex)
                         {
                             // Parse date and time with custom specifier.

                             cellValue = "";
                         }
                         
                         
                         
                     } },
                       { typeof(bool), () => {
                         // в строке будет дата
                         
                         try
                         {
                             cellValue = cell.BooleanCellValue.ToString();
                         }
                         catch (System.Exception ex)
                         {
                             // Parse date and time with custom specifier.

                             cellValue = "false";
                         }
                         
                         
                         
                     } },
                    { typeof(int), () => {cellValue= cell.NumericCellValue.ToString();} },
                    { typeof(string), () => {cellValue= cell.ToString();} },
                };

                    try
                    {
                        @switch[type]();
                    }
                    catch (System.Exception ex)
                    {
                        return cell.ToString();
                    }

                    if (string.IsNullOrEmpty(cellValue))
                    {
                        return null;
                    }
                    else
                        return cellValue;
                }
            }
            return null;
        }

        static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static object GetCellObjectValueExt(ICell cell, Type type)
        {
            if (cell != null)
            {
                //!!!! исправить. для неналовских типов дефолт значение обязательно
                if (string.IsNullOrEmpty(GetCellValue(cell)))
                {
                    // если наалбл объект, то ему допускается значение налл. если же нет, то его значение определим в коде ниже
                    if (IsNullable(type))
                        return null;
                    else
                    {
                        //objValue = Type.GetType(type.Name).IsValueType ?
                        //    Activator.CreateInstance(Type.GetType(type.Name)) : null;
                    }

                }

                {
                    object cellValue = string.Empty;
                    var @switch = new Dictionary<Type, Action> {
                    
                { typeof(DateTime), () => {
                        // тип дэйт тайм. значение должно быть обязательно.
                        try
                        {
                            cellValue= cell.DateCellValue;
                        }
                        catch
                        {
                            try
                            {
                                var strDate = cell.StringCellValue;
                                cellValue = DateTime.Parse(strDate);

                            }
                            catch (System.Exception ex)
                            {
                                cellValue = GetMinDateObject();
                            }
                            
                           
                        }
                    }
                },

                { typeof(DateTime?), () => {
                         try
                         {
                             DateTime? nullableDateTime = new DateTime?();
                             nullableDateTime = cell.DateCellValue;
                             cellValue = nullableDateTime;
                         }
                         catch (System.Exception ex)
                         {
                             cellValue = GetMinDateObjectNullable();
                         }
                     } 
                },

                  { typeof(bool?), () => {
                         try
                         {
                             bool? nullableBool = new bool?();
                             nullableBool = cell.BooleanCellValue;
                             cellValue = nullableBool;
                         }
                         catch (System.Exception ex)
                         {
                             cellValue = null;
                         }
                     } 
                },

                 { typeof(bool), () => {
                         try
                         {
                             bool boolable = false;
                             boolable = cell.BooleanCellValue;
                             cellValue = boolable;
                         }
                         catch (System.Exception ex)
                         {
                             cellValue = false;
                         }
                     } 
                },

                { typeof(decimal), () => { 
                        try
                        {
                            cellValue = Convert.ToDecimal(cell.NumericCellValue);
                        }
                        catch (System.Exception ex)
                        {
                            cellValue = 0;
                        }
                        
                        ;} 
                 },

                 { typeof(double), () => { 
                        try
                        {
                            cellValue = cell.NumericCellValue;
                        }
                        catch (System.Exception ex)
                        {
                            cellValue = 0;
                        }
                        
                        ;} 
                 },

                 { typeof(decimal?), () => { 
                         try
                         {
                             decimal? nullableDecimal = new decimal?();
                             if (cell.CellType == CellType.NUMERIC)
                                 nullableDecimal = (decimal?)cell.NumericCellValue;
                             else
                             {
                                 nullableDecimal = decimal.Parse(cell.RichStringCellValue.ToString());
                             }
                             cellValue = nullableDecimal;

                         }
                         catch (System.Exception ex)
                         {
                                cellValue = null;
                         }
                     }
                },
                     { typeof(double?), () => { 
                         try
                         {
                             double? nullableDouble = new double?();
                             if (cell.CellType == CellType.NUMERIC)
                                 nullableDouble = (double?)cell.NumericCellValue;
                             else
                             {
                                 nullableDouble = double.Parse(cell.RichStringCellValue.ToString());
                             }
                             cellValue = nullableDouble;

                         }
                         catch (System.Exception ex)
                         {
                                cellValue = null;
                         }
                     }
                },
                { typeof(int), () => 
                    {
                        try
                        {
                            cellValue = Convert.ToInt32(cell.StringCellValue);
                        }
                        catch (System.Exception ex)
                        {
                            try
                            {
                                cellValue = (int)cell.NumericCellValue;
                            }
                            catch (System.Exception ex2)
                            {
                                cellValue = 0;	
                            }
                           
                        }
                        
                    } 
                },

                { typeof(string), () => {cellValue= cell.ToString();} 
                },
                };

                    // место очень важное, и в трайкетч его засовывать не стоит
                    @switch[type]();
                    return cellValue;
                }
            }
            return null;
        }

        public static void SetProperty(TypeAccessor accessor, object obj, string fieldName, object objValue)
        {

            accessor[obj, fieldName] = objValue; // fill in name/value here

        }

        public static string GetMinDateString()
        {
            string dateString = "01-01-1900";
            //string format = "dd-MM-yyyy";
            //CultureInfo provider = CultureInfo.InvariantCulture;
            //    string result = DateTime.ParseExact(dateString, format, provider);
            return dateString;


        }

        public static DateTime GetMinDateObject()
        {
            string dateString = "01-01-1900";
            string format = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime result = DateTime.ParseExact(dateString, format, provider);
            return result;


        }

        public static DateTime? GetMinDateObjectNullable()
        {
            string dateString = "01-01-1900";
            string format = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime? result = DateTime.ParseExact(dateString, format, provider);
            return result;


        }

        /// <summary>
        /// Типозависимое возвращение значения из ячейки
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static object GetCellValueTyped(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType.ToString())
                {
                    case "BOOLEAN":
                        return cell.BooleanCellValue;

                    case "NUMERIC":
                        return cell.NumericCellValue;

                    case "STRING":
                        return (cell.StringCellValue);

                    case "BLANK":
                        return string.Empty;

                    case "ERROR":
                        return cell.ErrorCellValue;


                    // CELL_TYPE_FORMULA will never occur
                    case "FORMULA":
                        return string.Empty;

                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Подключение к рабочей книге экселя
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static HSSFWorkbook ConnectExlFile(string filepath)
        {
            {
                try
                {

                    using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        var hssfwb = new HSSFWorkbook(file);
                        logger.Info("Подключились к файлу Excel:" + Path.GetFileName(filepath));
                        return hssfwb;
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error("Ошибка подключения к файлу Excel:" + ex.Message);

                    return null;
                }
            }
        }
        /// <summary>
        /// Получение свойств типа. Другими словами получение рекваред филдов для дальнейшего использования
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TypeName"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public static object GetTableByName(string name, string TypeName, Context Context)
        {
            try
            {
                PropertyInfo tableInfo = Context.GetType().GetProperty(name);
                var _type = Type.GetType(TypeName);

                //var table = ((DbSet<Intranet.Models.DomainClasses.ShClone.SHCloneSite>)tableInfo.GetValue(Context, null));
                var objType = Type.GetType(TypeName);
                var tableType = typeof(DbSet<>).MakeGenericType(objType);
                var table = (tableInfo.GetValue(Context, null));


                return Convert.ChangeType(table, tableType);
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка получения таблицы по имени:" + name + " -" + ex.Message);
                return null;
            }
        }

        [Obsolete]
        public static object GetElementListFromTable(string name, string TypeName, Context Context)
        {
            try
            {
                PropertyInfo tableInfo = Context.GetType().GetProperty(name);
                var _type = Type.GetType(TypeName);

                //var table = ((DbSet<Intranet.Models.DomainClasses.ShClone.SHCloneSite>)tableInfo.GetValue(Context, null));
                var objType = Type.GetType(TypeName);
                var tableType = typeof(DbSet<>).MakeGenericType(objType);
                var table = (tableInfo.GetValue(Context, null));
                var ListObj = InvokeMethod("ToList", table, new object[] { });

                return ListObj;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка получения таблицы по имени:" + name + " -" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение свойства объекта по его имени(свойства).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PropertyInfo GetObjPropByName(string name, object obj)
        {
            try
            {
                var prop = obj.GetType().GetProperty(name);
                return prop;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка получения свойства по имени:" + name + " -" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Присвоение свойству значения по его имени(свойства)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetObjPropByName(string name, object obj, object value)
        {
            try
            {
                var prop = GetObjPropByName(name, obj);
                // если стринг, то ставим 
                if (value == null)
                {
                    if (prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        prop.SetValue(obj, value, null);
                    }
                }
                else
                {
                    prop.SetValue(obj, value , null);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка назначения свойства по имени:" + name + " -" + ex.Message);
                return false;

            }
        }
        /// <summary>
        /// Вызов метода объекта по его имени
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="parametrs"></param>
        /// <returns></returns>
        public static bool InvokeMethod(string name, object obj, params object[] parametrs)
        {
            try
            {
                MethodInfo method = obj.GetType().GetMethod(name);
                method.Invoke(obj, parametrs);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка вызова метода по имени:" + name + " -" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Вызов функции объекта по ее имени
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="parametrs"></param>
        /// <returns></returns>
        public static object InvokeFunction(string name, object obj, params object[] parametrs)
        {
            try
            {
                MethodInfo method = obj.GetType().GetMethod(name);
                var result = method.Invoke(obj, parametrs);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка вызова метода по имени:" + name + " -" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка свойств объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetProperties(object obj)
        {
            var properties = obj.GetType().GetProperties().ToList();
            return properties;
        }

        public static string GetValueExt(this PropertyInfo propInfo, object obj)
        {
            if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
            {
                var val = (propInfo.GetValue(obj, null));
                if (val !=null && (DateTime)val != DateTime.MinValue)
                {
                    return ((DateTime)val).ToString("dd.MM.yyyy");

                }
                else
                {
                    return "";
                }
            }
            return (propInfo.GetValue(obj, null) ?? "").ToString();
        }
        /// <summary>
        /// Создание объекта по имени его типа
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static object CreateInstaceByTypeName(string typeName)
        {
            var obj = Activator.CreateInstance(Type.GetType(typeName));
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private static List<SqlParameter> DictionaryToSqlParameterList(Dictionary<string, object> dict)
        {
            if (dict == null)
            {
                return null;
            }
            // мы не может создать экземпляр коллекции. конструктор у нее приватный. попробуем схитрить

            List<SqlParameter> parameters = new List<SqlParameter>();
            foreach (var key in dict.Keys)
            {

                if (dict[key].GetType().Equals(typeof(int)))
                {
                    parameters.Add(new SqlParameter(key, SqlDbType.Int) { Value = dict[key] });
                    continue;
                }
                if (dict[key].GetType().Equals(typeof(string)))
                {
                    parameters.Add(new SqlParameter(key, SqlDbType.NVarChar) { Value = dict[key].ToString() });
                    continue;
                }
                if (dict[key].GetType().Equals(typeof(DateTime)))
                {
                    parameters.Add(new SqlParameter(key, SqlDbType.DateTime) { Value = dict[key] });
                    continue;
                }
                if (dict[key].GetType().Equals(typeof(decimal)))
                {
                    parameters.Add(new SqlParameter(key, SqlDbType.Decimal) { Value = dict[key] });
                    continue;
                }
                if (dict[key].GetType().Equals(typeof(double)))
                {
                    parameters.Add(new SqlParameter(key, SqlDbType.Float) { Value = dict[key] });
                    continue;
                }
            }
            return parameters;
        }


        /// <summary>
        /// Выполнение хранимки на 400 сервере с параметрами
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> GetStoredProcDataFromServer<T>(string storedProcName, Dictionary<string, object> parameters)
        {
            try
            {
                List<T> list = new List<T>();
                using (SqlConnection cn = new SqlConnection(@"Data Source=EV001B78BFE400\CRM;Initial Catalog=OHDB;MultipleActiveResultSets=true;Integrated Security=SSPI"))
                {

                    using (SqlCommand Cmd = new SqlCommand(storedProcName, cn))
                    {


                        Cmd.CommandType = CommandType.StoredProcedure;
                        var paramCollect = DictionaryToSqlParameterList(parameters);
                        if (paramCollect != null)
                        {
                            foreach (var param in paramCollect)
                            {
                                Cmd.Parameters.Add(param);
                            }
                        }
                        cn.Open();
                        //Cmd.ExecuteNonQuery();
                        using (SqlDataReader dr = Cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var obj = StaticHelpers.CreateInstaceByTypeName(string.Format("{0},{1}", typeof(T).FullName, Path.GetFileNameWithoutExtension(typeof(T).Module.Name)));
                                var fields = StaticHelpers.GetProperties(obj);
                                foreach (var field in fields)
                                {
                                    // мы заполняем примитивные типа, дециами, дэйт тайм и такие же нуллабл типы
                                    if (
                                        (field.PropertyType.IsPrimitive) ||
                                        field.PropertyType.Equals(typeof(string)) ||
                                        field.PropertyType.Equals(typeof(decimal)) ||
                                        field.PropertyType.Equals(typeof(DateTime)) ||
                                            (field.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                                (field.PropertyType.GetGenericArguments()[0].IsPrimitive ||
                                                 field.PropertyType.GetGenericArguments()[0].Equals(typeof(decimal)) ||
                                                 field.PropertyType.GetGenericArguments()[0].Equals(typeof(DateTime))


                                                )

                                            )
                                        )
                                        SetObjPropByName(field.Name, obj, dr[field.Name]);
                                }

                                list.Add((T)obj);
                            }
                            dr.Close();
                        }
                    }
                    cn.Close();
                }
                return list;
            }
            catch (Exception exc)
            {
                logger.Error("Ошибка получения информации от хранимой процедуры " + storedProcName + " " + exc.Message);
            }
            return null;
        }

        static bool IsNullable<T>()
        {
            // if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
        /// <summary>
        /// Скалярное Выполнение хранимки на 400м сервере
        /// В результате выбирается значение первой строки первого столбца выходного селекта
        /// </summary>
        /// <param name="storedProcName">Имя СП</param>
        /// <param name="parameters">Параметры</param>
        /// <returns></returns>
        public static string GetStoredProcScalarValueFromServer(string storedProcName, Dictionary<string, object> parameters)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(@"Data Source=EV001B78BFE400\CRM;Initial Catalog=OHDB;MultipleActiveResultSets=true;Integrated Security=SSPI"))
                {
                    using (SqlCommand Cmd = new SqlCommand(storedProcName, cn))
                    {
                        cn.Open();
                        Cmd.CommandType = CommandType.StoredProcedure;
                        // Добавляем параметры к СП
                        var paramCollect = DictionaryToSqlParameterList(parameters);
                        if (paramCollect != null)
                        {
                            foreach (var param in paramCollect)
                            {
                                Cmd.Parameters.Add(param);
                            }
                            string result = string.Empty;
                            result = Cmd.ExecuteScalar().ToString();
                            cn.Close();
                            return result;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Ошибка получения информации от хранимой процедуры " + storedProcName + " " + exc.Message);
            }
            return null;
        }

        /// <summary>
        /// Выполнение хранимки с параметрами
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> GetStoredProcDataFromContext<T>(Context context, string procedureName, Dictionary<string, object> parameters)
        {

            try
            {
                var paramList = DictionaryToSqlParameterList(parameters);
                var list = new List<T>();
                if (paramList == null)
                {
                    list = context.Database.SqlQuery<T>(procedureName).ToList();

                }
                else
                {
                    list = context.Database.SqlQuery<T>(procedureName + " " + string.Join(", ", parameters.Keys), paramList.ToArray()).ToList();
                }

                return list;
            }
            catch (Exception exc)
            {
                logger.Error("Ошибка получения информации из хранимки " + procedureName + " " + exc.Message);
            }
            return null;
        }

        //public static List<Dictionary<string,object>> GetStoredProcDataFromServer(string storedProcName, Dictionary<string, string> parameters)
        //{
        //    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        //    SqlConnection cn = new SqlConnection(@"Data Source=EV001B78BFE400\CRM;Initial Catalog=OHDB;MultipleActiveResultSets=true;Integrated Security=SSPI");
        //    cn.Open();

        //    SqlCommand Cmd = new SqlCommand(storedProcName, cn);
        //    Cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    SqlDataReader dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //    while (dr.Read())
        //    {
        //        var obj = new Dictionary<string,object>();
        //        var fields = StaticHelpers.GetProperties(obj);
        //        foreach (var field in fields)
        //        {

        //           Dictionary
        //                SetObjPropByName(field.Name, obj, dr[field.Name]);
        //        }

        //        list.Add((T)obj);
        //    }
        //    dr.Close();
        //    return list;

        //}

   public static DateTime? RusStringDateToDateTime(string dateString)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                DateTime date = new DateTime();
                if (DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date;
                }
                else
                {
                    Dictionary<string, string> mounts = new Dictionary<string, string>();
                    mounts.Add("января", "01");
                    mounts.Add("январь", "01");
                    mounts.Add("февраля", "02");
                    mounts.Add("февраль", "02");
                    mounts.Add("марта", "03");
                    mounts.Add("март", "03");
                    mounts.Add("апреля", "04");
                    mounts.Add("апрель", "04");
                    mounts.Add("мая", "05");
                    mounts.Add("май", "05");
                    mounts.Add("июня", "06");
                    mounts.Add("июнь", "06");
                    mounts.Add("июля", "07");
                    mounts.Add("июль", "07");
                    mounts.Add("августа", "08");
                    mounts.Add("август", "08");
                    mounts.Add("сентября", "09");
                    mounts.Add("сентябрь", "09");
                    mounts.Add("октября", "10");
                    mounts.Add("октябрь", "10");
                    mounts.Add("ноября", "11");
                    mounts.Add("ноябрь", "11");
                    mounts.Add("декабря", "12");
                    mounts.Add("декабрь", "12");

                }
            }
            return null;
        }


    }
}


namespace CommonFunctions.ExchangeRates
{

    public class CurrencyRate
    {
        /// <summary>
        /// Закодированное строковое обозначение валюты
        /// Например: USD, EUR, AUD и т.д.
        /// </summary>
        public string CurrencyStringCode;

        /// <summary>
        /// Наименование валюты
        /// Например: Доллар, Евро и т.д.
        /// </summary>
        public string CurrencyName;

        /// <summary>
        /// Обменный курс
        /// </summary>
        public double ExchangeRate;
    }

    public class CurrencyRates
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public class ValCurs
        {
            [XmlElementAttribute("Valute")]
            public ValCursValute[] ValuteList;
        }

        public class ValCursValute
        {

            [XmlElementAttribute("CharCode")]
            public string ValuteStringCode;

            [XmlElementAttribute("Name")]
            public string ValuteName;

            [XmlElementAttribute("Value")]
            public string ExchangeRate;
        }

        /// <summary>
        /// Получить список котировок ЦБ ФР на данный момент
        /// </summary>
        /// <returns>список котировок ЦБ РФ</returns>
        public static List<CurrencyRate> GetExchangeRates()
        {
            List<CurrencyRate> result = new List<CurrencyRate>();
            XmlSerializer xs = new XmlSerializer(typeof(ValCurs));
            try
            {
                XmlReader xr = new XmlTextReader(@"http://www.cbr.ru/scripts/XML_daily.asp");
                foreach (ValCursValute valute in ((ValCurs)xs.Deserialize(xr)).ValuteList)
                {
                    result.Add(new CurrencyRate()
                    {
                        CurrencyName = valute.ValuteName,
                        CurrencyStringCode = valute.ValuteStringCode,
                        ExchangeRate = Convert.ToDouble(valute.ExchangeRate)
                    });
                }
            }
            catch
            {
                logger.Error("Ошибка получения курсов валют");
            }
            return result;
        }

        public static double? GetExchangeRateByValuteStringCode(string code)
        {
            var result = GetExchangeRates().FirstOrDefault(r => r.CurrencyStringCode == code);
            if (result != null)
            {

                return result.ExchangeRate;
            }
            else
                return null;
        }

    }
}