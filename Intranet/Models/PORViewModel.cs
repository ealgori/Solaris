using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Models
{
    public class NewPORViewModel
    {
        public NewPORViewModel()
        {
            Items = new List<NewPORItemViewModel>();
        }
        public int ProjectId { get; set; }
        public int SubcId { get; set; }
        /// <summary>
        /// Только для выведения на форму. Инфа для чтения
        /// </summary>
        public string Subcontractor { get; set; }
        public string SubRegion { get; set; }
        public string AVRId { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        public List<NewPORItemViewModel> Items { get; set; }
        public decimal? PriceTotal { get; set; }
        public decimal? PriceTotalSH { get; set; }
        public string Network { get; set; }
        public string Activity { get; set; }
        

    }
    public class NewPORItemViewModel
    {
        public NewPORItemViewModel()
        {
            Koeff = 1;
            Quantity = 1;
            Price = 0;

        }
        public int Id { get; set; }
        public string SHDescription { get; set; }
        public string Site { get; set; }
        public string SiteFix { get; set; }
        public string SiteFol { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public decimal? PricePerItem { get; set; }
        public decimal? PricePerItemSH { get; set; }
        public decimal? Koeff { get; set; }
        public int? PositionId { get; set; }
        public string Unit { get; set; }
        public decimal? PriceSH { get; set; }
        public string ECRType { get; set; }
        /// <summary>
        /// Нужен только для ECR Add
        /// </summary>
        public int? AVRItemId { get; set; }
    }
    public class PORViewModel
    {
        public PORViewModel()
        {
            Items = new List<PORItemViewModel>();
        }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int SubcId { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        public List<PORItemViewModel> Items { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Network { get; set; }
        public string Activity { get; set; }

    }
    public class PORItemViewModel
    {
        public PORItemViewModel()
        {
            Koeff = 1;
            Quantity = 1;
            Price = 0;

        }
        public int Id { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public int? PositionId { get; set; }
        public int? Price { get; set; }
        public decimal? Koeff { get; set; }
        //public bool IsCustom { get; set; }
        //public decimal? CustomPrice { get; set; }
        //public string CustomDescription { get; set; }
        //public string CustomUnit { get; set; }
    }
    /// <summary>
    /// Модель, определяющая входные данные для расчета полной цены цены позиции
    /// </summary>
    public class PorCodeCalcModel
    {
        public int SubcId { get; set; }
        public int ProjectId { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        public int PriceListId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Coeff { get; set; }
    }
}