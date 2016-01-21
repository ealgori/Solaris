using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NPOI.SS.UserModel;
using System.Diagnostics;
using System.IO;
using FastMember;
using System.Data;
using DbModels.DataContext;


using CommonFunctions.Extentions;
using CommonFunctions;


namespace ShClone.UniReport
{
    /// <summary>
    /// занимается чтением экселек, определением необходимых полей, и их поиск в файле
    /// </summary>
    public class UniReportBulkCopy : UniReportProtoType
    {
        public UniReportBulkCopy(string tableTypeName, string typeName, List<string> files, Context context, List<DataTable> objects)
            : base(tableTypeName, typeName, files, context)
        {
            ObjectsList = objects ;
        }

        public List<DataTable> ObjectsList { get; set; }
        /// <summary>
        /// Полная процедура чтения фалов
        /// </summary>
        /// <returns></returns>
        public override bool ReadFiles()
        {
            if (GetRequiredFields())
            {
                if (FieldMatching())
                {
                    if (Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Чтение всех файлов и добваление информации из них в объект QueryBuilder
        /// </summary>
        /// <returns></returns>
        public  bool ReadOld()
        {
            try
            {
                int startRow = 1;
                //// Добавляем запрос на очистку таблицы
                //Builder.CreateDeleteQuery(TableTypeName);
                foreach (var file in Files)
                {
                    
                    
                    List<object> objectsList = new List<object>();
                    logger.Info(string.Format("Начинаем чтение файла:{0}", Path.GetFileName(file)));
                    var workBook = NpoiInteract.ConnectExlFile(file);
                    if (workBook == null)
                        return false;
                    ISheet sheet = workBook.GetSheetAt(0);
                    // SQLInsertStaimentBuidler builder = new SQLInsertStaimentBuidler(TableTypeName, Fields.Select(f => f.Key).ToList());


                    int rowReaded = 0;

                    // var table = StaticHelpers.GetTableByName(TableTypeName, TypeName, Context);
                    if (sheet != null)
                        for (int rowNum = Fields.FirstOrDefault().Value.Item2.RowIndex + startRow; rowNum < sheet.PhysicalNumberOfRows; rowNum++)
                        {

                            // Оббегаем все строки в них
                            IRow row = sheet.GetRow(rowNum);
                            if (row != null)
                            {
                                rowReaded++;
                                var testCell = row.GetCell(0);
                                if ((testCell != null)&&(!string.IsNullOrEmpty(testCell.StringCellValue)))
                                {

                                    //List<Field> values = new List<Field>();
                                    var OBJ = Activator.CreateInstance(Type.GetType(TypeName));
                                    var accessor = TypeAccessor.Create(OBJ.GetType());


                                    foreach (var field in Fields)
                                    {
                                        try
                                        {
                                            var cell = row.GetCell(field.Value.Item2.ColumnIndex);
                                            if (cell == null)
                                            {
                                                StaticHelpers.SetProperty(accessor, OBJ, field.Key, null);
                                                // values.Add(new Field() { FieldType = field.Value.Item1, NameValue = null });
                                            }
                                            else
                                            {
                                                var cellValue = NpoiInteract.GetCellObjectValueExt(cell, field.Value.Item1);
                                                StaticHelpers.SetProperty(accessor, OBJ, field.Key, cellValue);
                                                // values.Add(new Field() { FieldType = field.Value.Item1, NameValue = cellValue });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // если вы сюда попали, значит скорее всего ошибка в типе field.Value.Item1. такой тип не описан в GetCellObjectValueExt
                                            logger.Error(string.Format("Ошибка при импорте-Сообщение:{0}, Файл:{1}, Ряд:{2}, Колонка:{3}, Поле:{4}, Тип:{5}", ex.Message, file, row.RowNum, field.Value.Item2.ColumnIndex, field.Key, field.Value.Item1));
                                            return false;
                                        }
                                    }
                                    //var obj = ObjectBuilder.CreateObject(RequiredField,values,TypeName);
                                    //  Builder.AddInsertValue(TableTypeName, RequiredField, values);
                                    // Debug.WriteLine(Builder.Queries.Last().ToSQL());
                                    objectsList.Add(OBJ);
                                }

                            }


                        }

                    DataTable table = objectsList.ToDataTable();
                    objectsList = null;
                    workBook = null;
                    table.TableName = TableTypeName;
                    ObjectsList.Add(table);
                    startRow = 0;


                    logger.Info(string.Format("Прочли:{0} строк;{1}", rowReaded, Path.GetFileName(file)));
                }




                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка при импорте-" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Чтение всех файлов и добваление информации из них в объект QueryBuilder
        /// </summary>
        /// <returns></returns>
        public override  bool Read()
        {
            try
            {
                int startRow = 1;
                //// Добавляем запрос на очистку таблицы
                //Builder.CreateDeleteQuery(TableTypeName);
                foreach (var file in Files)
                {


                    DataTable table = new DataTable();
                    foreach (var field in Fields)
                    {

                        var dataColumn = new DataColumn();
                        dataColumn.ColumnName = field.Key;
                        var type = field.Value.Item1;
                        if (Nullable.GetUnderlyingType(type) != null)
                        {
                            type = Nullable.GetUnderlyingType(type);
                            dataColumn.AllowDBNull = true;
                        }
                        if (field.Value.Item1.BaseType == typeof(Enum))
                        {
                            dataColumn.AllowDBNull = true;
                        }

                        dataColumn.DataType = type;
                        table.Columns.Add(dataColumn);

                    }
                    logger.Info(string.Format("Начинаем чтение файла:{0}", Path.GetFileName(file)));
                    var workBook = NpoiInteract.ConnectExlFile(file);
                    if (workBook == null)
                        return false;
                    ISheet sheet = workBook.GetSheetAt(0);
                   


                    int rowReaded = 0;

                    // var table = StaticHelpers.GetTableByName(TableTypeName, TypeName, Context);
                    if (sheet != null)
                        for (int rowNum = Fields.FirstOrDefault().Value.Item2.RowIndex + startRow; rowNum < sheet.PhysicalNumberOfRows; rowNum++)
                        {

                            // Оббегаем все строки в них
                            IRow row = sheet.GetRow(rowNum);
                            var dtRow = table.NewRow();
                            if (row != null)
                            {
                                rowReaded++;
                                var testCell = row.GetCell(0);
                                if ((testCell != null) && (!string.IsNullOrEmpty(testCell.StringCellValue)))
                                {

                                    //List<Field> values = new List<Field>();
                                   // var OBJ = Activator.CreateInstance(Type.GetType(TypeName));
                                   // var accessor = TypeAccessor.Create(OBJ.GetType());


                                    foreach (var field in Fields)
                                    {
                                        try
                                        {
                                           // var column = table.Columns[field.Key];
                                            var cell = row.GetCell(field.Value.Item2.ColumnIndex);
                                            if (cell == null)
                                            {

                                                dtRow[field.Key] = DBNull.Value;
                                                //StaticHelpers.SetProperty(accessor, OBJ, field.Key, null);
                                                // values.Add(new Field() { FieldType = field.Value.Item1, NameValue = null });
                                            }
                                            else
                                            {
                                                var cellValue = NpoiInteract.GetCellObjectValueExt(cell, field.Value.Item1);
                                                if(cellValue!=null)
                                                    dtRow[field.Key] = cellValue;
                                                else
                                                    dtRow[field.Key] = DBNull.Value;
                                                //StaticHelpers.SetProperty(accessor, OBJ, field.Key, cellValue);
                                                // values.Add(new Field() { FieldType = field.Value.Item1, NameValue = cellValue });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // если вы сюда попали, значит скорее всего ошибка в типе field.Value.Item1. такой тип не описан в GetCellObjectValueExt
                                            logger.Error(string.Format("Ошибка при импорте-Сообщение:{0}, Файл:{1}, Ряд:{2}, Колонка:{3}, Поле:{4}, Тип:{5}", ex.Message, file, row.RowNum, field.Value.Item2.ColumnIndex, field.Key, field.Value.Item1));
                                            return false;
                                        }
                                    }
                                    //var obj = ObjectBuilder.CreateObject(RequiredField,values,TypeName);
                                    //  Builder.AddInsertValue(TableTypeName, RequiredField, values);
                                    // Debug.WriteLine(Builder.Queries.Last().ToSQL());
                                   // objectsList.Add(OBJ);
                                    table.Rows.Add(dtRow);
                                }

                            }


                        }

                
                    workBook = null;
                    table.TableName = TableTypeName;
                    ObjectsList.Add(table);
                    startRow = 0;


                    logger.Info(string.Format("Прочли:{0} строк;{1}", rowReaded, Path.GetFileName(file)));
                }




                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка при импорте-" + ex.Message);
                return false;
            }
        }


    }
}

