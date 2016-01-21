using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using System.Collections;
using TaskManager.TaskParamModels;


namespace TaskManager.Handlers.FileIOHandlers
{
    public abstract class AFileIOHandler
    {
        #region свойства которые перезаписываются файл хендлером во время работы
        //public List<FileHeader> FileHeaders = new List<FileHeader>();
        public int DataStartRow { get; set; }
        #endregion

        public TaskParameters TaskParameters {get;set;}
       // protected Context Context { get; set; }
       // public Type ObjectType { get; set; }

       // public TaskLogger TaskLogger { get; set; }
        /// <summary>
        /// Назначается в конструкторе
        /// </summary>
       // public TaskBase Task { get; set; }
        /// <summary>
        /// с этой строчки происходит чтение данных. Назначение происходит в функции ФайндХеадерс
        /// </summary>
       

        public AFileIOHandler(TaskParameters taskParameters)
        {

            this.TaskParameters = taskParameters;
           // this.ObjectType = objectType;
            //this.TaskLogger = logger;
          //  this.Task = task;
        }

        protected abstract List<FileHeader> FindHeaders(FileParams fileParams);
        protected abstract bool ReadFile(MemoryStream stream);
        protected abstract ObjectParams GetObjects(FileParams parameters, List<FileHeader> FileHeaders);
        public virtual bool  Handle()
        {
          
            var objParams = new Dictionary<string,ObjectParams>(); 
            foreach (var parameter in TaskParameters.FileHandlerParams.StreamParameters)
            {
                
              // Считываем весь файл. Как он это делает и переменная в которой он это сохранит это его дело.
                if (ReadFile(parameter.Value.FileStream))
                {


                    List<FileHeader> fileHeaders;
                    // получаем заголовки этого файла из считанного файла
                    if ((fileHeaders = FindHeaders(parameter.Value)) == null)
                    {
                        continue;
                    }
                    else
                    {
                        // получаем объекты. тип указан в стрим параметре. тобишь в вэлью. вэлью это стрим параметр
                        ObjectParams oParams = GetObjects(parameter.Value,fileHeaders);
                        objParams.Add(parameter.Key, oParams);
                    }
                }
            }
            TaskParameters.FileHandlerParams.ObjectParameters = objParams;
            return true;
        }
    }
}