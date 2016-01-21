using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.DbTasks
{
    public class DbTask
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Активен. Т.Е. Вообще будет выполняться
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Порядок. общий для всех типов
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Выполняется при обновлении базы
        /// </summary>
        public bool Triggered { get; set; }
        /// <summary>
        /// Куда кладем результаты жизнедеятельности
        /// </summary>
        public string EmailSendFolder { get; set; }
        /// <summary>
        /// То что оставляем себе, на память
        /// </summary>
        public string ArchiveFolder { get; set; }

        ///// <summary>
        ///// Таймаут в минутах. Ноль значит без таймаута.
        ///// </summary>
        //public int Timeout { get; set; }
        /// <summary>
        /// Имя файла импорта, который будет
        /// </summary>
        public string ImportFileName1 { get; set; }
        public string ImportFileName2 { get; set; }
        public string ImportFileName3 { get; set; }
        public string ImportFileName4 { get; set; }
        public string ImportFileName5 { get; set; }
        /// <summary>
        /// Максимальное количество объектов содержащихся в файле импорта
        /// </summary>
        public int MaxImportObjectPerFile { get; set; }
        
       /// <summary>
       /// Путь к темплейту
       /// </summary>
        public string TemplatePath { get; set; }
        public string TemplatePath2 { get; set; }
        public string TemplatePath3 { get; set; }
        public string TemplatePath4 { get; set; }
        public string Params { get; set; }
        public DateTime? StartDate { get; set; }
        public string Interval { get; set; }

        public string OperationalType { get; set; }
    }
}