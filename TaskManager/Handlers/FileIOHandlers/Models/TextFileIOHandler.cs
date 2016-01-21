//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.IO;
//using System.Collections;
//using TaskManager.Handlers.FileIOHandlers;
//using TaskManager.TaskParamModels;


//namespace Intranet.Projects.TaskManager.Handlers.FileIOSubHandlers
//{
//    public class TextFileIOHandler : AFileIOHandler
//    {

//        public TextFileIOHandler(TaskParameters taskParameters)
//            : base(taskParameters)
//        {
            
//        }
//        public string[] separators = new string[] {"\t"};
//        /// <summary>
//        /// Содержимое текстового файла
//        /// </summary>
//        public List<string> FileStrings = new List<string>();

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        protected override List<FileHeader> FindHeaders(FileParams fileParams)
//        {
//            var fileHeaders = new List<FileHeader>();

//            string objTypeString = fileParams.ObjectType.ToString();
//            var dbHeaders = TaskParameters.TaskLogger.context.DbHeaders.Where(dbh => dbh.ObjectType == objTypeString);
//            if (dbHeaders.Count() == 0)
//            {
//                TaskParameters.TaskLogger.LogError("В базе нет ни одного столбца, который собираемся искать в данном файле.");
//                return null;

//            }
//            //в каком количестве первх строк будем искать заголовки
//            int linesCount = 10;
//            // список предполагаемых заголовков

//            List<string> headersLines = new List<string>();
//            for (int l = 0; l < linesCount; l++)
//            {
//                try
//                {
//                    headersLines.Add(ReadLine(l));
//                }
//                catch (System.Exception ex)
//                {

//                }
//            }
//            //ищем строку в которой стодержаться все заголовки
//            var line = headersLines.FirstOrDefault(l => dbHeaders.All(h => l.Contains(h.HeadName)));
//            if(line==null)
//            {
//                TaskParameters.TaskLogger.LogError("Не найдена строка с необходимымим заголовками");
//                return null;
//            }
//            else
//            {
//                // надо поставить мемори стрим на строку следующие после заголовков.
//                //////////////////////////////////////////////////////////////////////////
//                DataStartRow = headersLines.IndexOf(line) + 1;
//                //////////////////////////////////////////////////////////////////////////
//                TaskParameters.TaskLogger.LogDebug("Заголовки найдены в строке " + DataStartRow.ToString());
//                try
//                {
               
//                var parts = line.Split(separators, StringSplitOptions.None).Select(p => p.Trim()).ToList();
//                fileHeaders = new List<FileHeader>();
//                dbHeaders.ToList().ForEach(dbh =>
//                {
//                    fileHeaders.Add(
//                        new FileHeader()
//                        {
//                            Column = parts.IndexOf(dbh.HeadName.Trim()),
//                            PropName = dbh.PropName,
//                            HeadName = dbh.HeadName,
//                            // тип записан в стринг форме с полным путем.
//                            Type = Type.GetType(dbh.Type) ?? GetNullableType(dbh.Type),
//                            Required = dbh.Required
                            
//                        });
//                });
                
//                return fileHeaders;
//                }
//                catch (System.Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError("Ошибка при парсинге типов полей. Проверяйте название типов");
//                    return null;
//                }
//            }
//        }

//        /// <summary>
//        /// получаем список объектов
//        /// </summary>
//        /// <returns></returns>
//        protected override ObjectParams GetObjects(FileParams parameters, List<FileHeader> fileHeaders)
//        {
//           // ObjectParams result = new ObjectParams();
//            var objParams = new ObjectParams();
//            ArrayList objects = new ArrayList();
//            for (int l = DataStartRow; l < FileStrings.Count; l++)
//            {


//                // создаем экземпляр нужного типа
//                var obj = Activator.CreateInstance(parameters.ObjectType);
//                // читаем строку из файла
//                string line = ReadLine(l);
//                // заполняем объект
//                if (Setter.SetValuesFromLine(line, separators, fileHeaders, ref obj))
//                    objects.Add(obj);
//                else
//                {
//                    TaskParameters.TaskLogger.LogWarn("Некорректная строка:" + line);

//                }
//            }
//            objParams.Objects = objects;
//            objParams.ObjectType = parameters.ObjectType;
//            objParams.ObjectParameters = parameters.Parameters;


//            return objParams;
//        }
  
//        /// <summary>
//        /// Преобразует стринг имя нуллабл типа, в нуллабл тип. Работает страшновато конечно
//        /// 
//        /// </summary>
//        /// <param name="typeName"></param>
//        /// <returns></returns>
//        protected Type GetNullableType(string typeName)
//        {
//            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.

//            Type type = Type.GetType(typeName.Remove(typeName.Length - 1));
//            //type = Nullable.GetUnderlyingType(type);
//            if (type != null)
//            {
//                if (type.IsValueType)
//                    return typeof(Nullable<>).MakeGenericType(type);
//                else
//                    return type;
//            }
//            return null;
//        }

//        protected string ReadLine(int i)
//        {
//            try
//            {
//                return FileStrings[i];
//            }
//            catch(Exception exc)
//            {
//                TaskParameters.TaskLogger.LogWarn("Вышли за пределы массива списка строк файла." + exc.Message);
//                return string.Empty;
//            }
//        }
//        /// <summary>
//        /// читаем файл в список стрингов
//        /// </summary>
//        /// <returns></returns>
//        protected override bool ReadFile(MemoryStream stream)
//        {

//            FileStrings = new List<string>();
            
           
//            if (stream.Length == 0)
//            {
               
//            }
//            using(StreamReader reader = new StreamReader(stream))
//            {
//                 string line;
                 
//                 while ((line = reader.ReadLine()) != null)
               
//                    FileStrings.Add(line);
               
//                return true;
//            }
//        }




       
//    }
//}