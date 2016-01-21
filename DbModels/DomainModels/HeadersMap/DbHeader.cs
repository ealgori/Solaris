using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.HeadersMap
{
    public class DbHeader
    {
        public int Id { get; set; }
        /// <summary>
        /// Тип объекта которому будут пренадлежать эти поля
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// Название столбца в файле
        /// </summary>
        public string HeadName { get; set; }
        /// <summary>
        /// Название связного свойства в классе
        /// </summary>
        public string PropName { get; set; }
        /// <summary>
        /// Тип этого свойства
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Обязательно должно быть заполнено
        /// </summary>
        public bool Required { get; set; }
    }
}