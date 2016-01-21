using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Models
{
    public class CompareReportModel
    {
        // public int Id { get; set; }
        [Display(Name = "Подрядчик для анализа")]
        public string SubcontractorFrom { get; set; }
        //[Display(Name = "Опорный подрядчик")]
        //public string SubcontractorTo { get; set; }
        public DateTime? Date { get; set; }
        [Display(Name = "Прайслист ПОРа")]
        public string PriceListFrom { get; set; }
         [Display(Name = "Опорный прайслист")]
        public string PriceListTo { get; set; }
        public decimal PriceListIdFrom { get; set; }
        public decimal? PriceListIdTo { get; set; }
        public decimal RevisionFrom { get; set; }
        public decimal? RevisionTo { get; set; }
        public decimal ItemIdFrom { get; set; }
        public decimal? ItemIdTo { get; set; }
        [Display(Name = "Цена из ПОРа")]
        public decimal PriceFrom { get; set; }
        [Display(Name = "Цена из опорного ПЛ")]
        public decimal PriceTo { get; set; }
        public decimal? MiddlePrice { get; set; }
        [Display(Name = "Описание позиции")]
        public string DescriptionFrom { get; set; }
        public string DescriptionTo { get; set; }
        public string SapCodeFrom { get; set; }





        public string SapCodeTo { get; set; }
        public decimal Value { get; set; }
        public decimal PorQuantity { get; set; }
        public string Por { get; set; }
        public DateTime PorDate { get; set; }
        public string PO { get; set; }

        public decimal? PorPriceCoef { get; set; }

    }
}