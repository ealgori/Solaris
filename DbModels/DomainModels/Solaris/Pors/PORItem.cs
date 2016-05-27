using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.Pors
{
    public class PORItem:Entity
    {
        public int No { get; set; }
        public string Cat { get; set; }
        public string Code { get; set; }
        public string Plant { get; set; }
        public decimal NetQty { get; set; }
        public string B1 { get; set; }
        public string B2 { get; set; }
        public string ItemCat { get; set; }
        public string PRtype { get; set; }
        public string B3 { get; set; }
        public string B4 { get; set; }
        public string POrg { get; set; }
        public string B6 { get; set; }
        public string B7 { get; set; }
        public string GLacc { get; set; }
        public decimal Price { get; set; }
        public string Curr { get; set; }
        public string PRUnit { get; set; }
        public string B8 { get; set; }
        public string Vendor { get; set; }
        public string B9 { get; set; }
        public string B10 { get; set; }
        public string B11 { get; set; }
        public string B12 { get; set; }
        public string B13 { get; set; }
        public string B14 { get; set; }
        public string B15 { get; set; }
        public string B16 { get; set; }
        public bool IsCustom { get; set; }
        public DateTime? Plandate { get; set; }
        public string Description { get; set; }
        public string Site { get; set; }
        public string FIX { get; set; }
        public string FOL { get; set; }
        public virtual PriceListRevisionItem PriceListRevisionItem { get; set; }
        public virtual POR POR { get; set; }
        public decimal? Coeff { get; set; }
        public int? ItemId { get; set; }
        public string Network { get; set; }
    }

    public class PORTOItem : Entity
    {
        public int No { get; set; }
        public string Cat { get; set; }
        public string Code { get; set; }
        public string Plant { get; set; }
        public decimal NetQty { get; set; }
        public string B1 { get; set; }
        public string B2 { get; set; }
        public string ItemCat { get; set; }
        public string PRtype { get; set; }
        public string B3 { get; set; }
        public string B4 { get; set; }
        public string POrg { get; set; }
        public string B6 { get; set; }
        public string B7 { get; set; }
        public string GLacc { get; set; }
        public decimal Price { get; set; }
        public string Curr { get; set; }
        public string PRUnit { get; set; }
        public string B8 { get; set; }
        public string Vendor { get; set; }
        public string B9 { get; set; }
        public string B10 { get; set; }
        public string B11 { get; set; }
        public string B12 { get; set; }
        public string B13 { get; set; }
        public string B14 { get; set; }
        public string B15 { get; set; }
        public string B16 { get; set; }
        public bool IsCustom { get; set; }
        public DateTime? Plandate { get; set; }
        public string Description { get; set; }
        public virtual PriceListRevisionItem PriceListRevisionItem { get; set; }
        public virtual POR POR { get; set; }
        public string ItemId { get; set; }

        public string Act { get; set; }
        
    }
}
