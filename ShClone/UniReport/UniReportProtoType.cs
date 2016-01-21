using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NLog;
//using Intranet.Projects.ShClone.Extentions;
using NPOI.SS.UserModel;
using DbModels.DataContext;
using ShClone.UniReport.Model;
using System.Reflection;
using CommonFunctions;
using DbModels.DomainModels.ShClone;

//using Intranet.Projects.ShClone.ImportClasses.SQLBuider.Abstract;

namespace ShClone.UniReport
{
    public abstract class UniReportProtoType
    {

        /// <summary>
        ///  Файлы для обработки
        /// </summary>
        public List<string> Files { get; set; }
        /// <summary>
        /// Справочник полей с именами и их ячейками.
        /// </summary>
        public Dictionary<string, Tuple<Type, ICell>> Fields { get; set; }
        /// <summary>
        /// Поля и их типы полученный рефлектором из элемента таблицы. 
        /// </summary>
        public List<Field> RequiredField { get; set; }
        public Context Context { get; set; }
        /// <summary>
        /// Имя таблицы. Содержиться в схфайле
        /// </summary>
        public string TableTypeName { get; set; }
        /// <summary>
        /// Имя типа, содержащегося в таблице. Содержится в схфайле
        /// </summary>
        public string TypeName { get; set; }
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public UniReportProtoType(string tableTypeName, string typeName, List<string> files, Context context)
        {
            TableTypeName = tableTypeName;
            TypeName = typeName;
            Files = files;
            Context = context;
            Fields = new Dictionary<string, Tuple<Type, ICell>>();
            //ObjType = Type.GetType(TypeName);
            //TableType = typeof(DbSet<>).MakeGenericType(ObjType);
        }



        public abstract bool ReadFiles();

        public abstract bool Read();
        /// <summary>
        /// Получение полей, которые обязательно должны содержаться во входной эксельке. путем рефакторинга
        /// </summary>
        /// <returns></returns>
        public bool GetRequiredFields()
        {
            try
            {

                //var obj = StaticHelpers.CreateInstaceByTypeName(TypeName);
                //var tableProps = StaticHelpers.GetProperties(obj);
                //RequiredField = tableProps.Select(tp => new Field() { FieldType = Type.GetType(tp.PropertyType.FullName), NameValue = tp.Name }).ToList();
                //return true;
                RequiredField = new List<Field>();

                var obj = StaticHelpers.CreateInstaceByTypeName(TypeName);
                var props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var field = new Field();
                    var fkAttr = prop.GetCustomAttribute(typeof(FKAttribute));
                    var excAttr = prop.GetCustomAttribute(typeof(ExcludeAttribute));
                    if (fkAttr != null)
                    {
                        field.FieldType = ((FKAttribute)fkAttr).Type;
                        field.NameValue = ((FKAttribute)fkAttr).Name;
                    }
                    else
                    {
                        field.FieldType = Type.GetType(prop.PropertyType.FullName);
                        if (field.FieldType == null)
                            field.FieldType = prop.PropertyType.UnderlyingSystemType;
                        field.NameValue = prop.Name;
                    }
                    if(excAttr==null)
                        RequiredField.Add(field);
                }
                return true;
            }
            catch (Exception exc)
            {
                logger.Error("Ошибка получения свойств таблицы, либо создания экземпляра объекта типа указанного в ShFiles:" + TableTypeName + " -" + exc.Message);
                return false;
            }
        }

        /// <summary>
        /// Поиск полей реквайред филдов в первом из эксель файлов. ибо только там они и есть.
        /// </summary>
        /// <returns></returns>
        public bool FieldMatching()
        {
            try
            {
                var workBook = NpoiInteract.ConnectExlFile(Files.First());

                ISheet sheet = workBook.GetSheetAt(0);
                if (sheet != null)
                {
                    int rowNum = 0;

                    // Оббегаем все строки в них
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
                                    foreach (var field in RequiredField)
                                    {
                                        if (cellValue.Trim() == field.NameValue)
                                        {
                                            var tuple = new Tuple<Type, ICell>(field.FieldType, cell);
                                            Fields.Add(field.NameValue, tuple);
                                            break ;
                                        }
                                    }
                                }
                            }
                        }
                }
                if (RequiredField.Count == Fields.Count)
                {
                    logger.Info("Ожидали:{0}; Нашли:{1}",
                    string.Join(", ", RequiredField.Select(rf => rf.NameValue)),
                    string.Join(", ", Fields.Select(f => f.Key)));
                    return true;
                }

                // если ты здесь, ставь точку останова на строчку ниже и запускай обработку файла. попав в ошибку перенеси курсор на начало
                // первого цикла и построчно сравни считанный текст ячейки с рекваред филдами.читается все равно только первые две строчки
                //  Ты найдешь ошибку. я в тебя верю)


                logger.Error("Количество найденых полей не совпадает с ожидаемым!");
                logger.Error("Ожидали:{0}; Нашли:{1}",
                   string.Join(", ", RequiredField.Select(rf => rf.NameValue)),
                    string.Join(", ", Fields.Select(f => f.Key)));
                logger.Error(string.Join(",", RequiredField.Select(rf => rf.NameValue).Except(Fields.Select(f => f.Key))));

                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка при поиске полей в эксель файле-" + ex.Message);
                return false;
            }
        }
    }
}