using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses
{
    public class ItemPrepriceImportModel
    {
        public int ItemId { get; set; }
        public int? PriceListRevisionItemId { get; set; }
        public decimal? VCQuantity { get; set; }
        public decimal? VCPrice { get; set; }
        public string VCDescription { get; set; }
        public bool VCCustomItem { get; set; }
        public bool VCUseCoeff { get; set; }
        public string  NoteVC { get; set; }
    }
}
