using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
//using Intranet.Models.DomainClasses.ShClone;

//using Intranet.Projects.ShClone.Tasks;

namespace ShClone.Abstract
{
    public abstract class ShClone_prototype
    {
        public  Logger logger = LogManager.GetCurrentClassLogger();
        public abstract void DoWork();
    }
}