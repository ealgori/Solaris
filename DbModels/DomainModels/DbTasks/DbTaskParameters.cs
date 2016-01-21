using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.DbTasks
{
    public class DbTaskParameters
    {
        public int Id { get; set; }
        public DbTask DbTask { get; set; }
        /// <summary>
        /// Тип значения точно описанный
        /// </summary>
        public string ValueType { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// в какие параметры это значение попадет. Два типа параметров: FileHandlerParams и TaskHandlerParams
        /// </summary>
        public string Case { get; set; }
    }
}
