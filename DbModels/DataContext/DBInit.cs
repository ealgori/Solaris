using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
//using Intranet.Models.DomainClasses.WO;
//using Intranet.Models.DomainClasses.ShClone;
using System.Globalization;
using DbModels.DomainModels.HeadersMap;
using DbModels.DomainModels.ShClone;
using DbModels.DomainModels.DbTasks;
//using Intranet.Models.DomainClasses.Tasks;
//using Intranet.Models.DomainClasses.OpenPosted;
//using Intranet.Projects.OpenPosted.Models;

namespace DbModels.DataContext
{
    public class DBInit : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
           

         

        }
    }
}