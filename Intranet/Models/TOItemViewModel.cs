using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbModels.DomainModels.ShClone;
using DbModels.Models;
using DbModels.DataContext.Repositories;

namespace Intranet.Models
{
    public class ToItemViewModel
    {
        public TORepository.TOItemViewModel ToItemVM { get; set; }
        public ShSITE ShSite { get; set; }
        public ShFOL ShFOL { get; set; }
        public PriceListRevisionItem PLRItem{ get; set; }
    }
}