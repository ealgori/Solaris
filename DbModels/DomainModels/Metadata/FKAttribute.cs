using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.ShClone
{
    public class FKAttribute : Attribute
    {
        public string Name;
        public Type Type;
        public FKAttribute(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
