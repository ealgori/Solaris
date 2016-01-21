using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShClone.UniReport.Model
{
    public class Field
    {
        // содержимое ячейки
        public string NameValue { get; set; }
        // тип содержимого
        public Type FieldType { get; set; }
    }
}