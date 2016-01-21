using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Intranet.Models
{
    public class GrouppedModel
    {
        //[Display(Name = "ПП")]
        //public int Id { get; set; }
        [Display(Name = "Подрядчик")]
        public string Subcontractor { get; set; }
        //[Display(Name = "Дата подписания")]
        //public DateTime? Date { get; set; }
        [Display(Name = "Месяц")]
        public int Month { get; set; }
        [Display(Name = "Год")]
        public int Year { get; set; }
        //public DateTime? PorDate { get; set; }

        public string DS { get; set; }
        [Display(Name = "Удешевление")]
        public decimal? Plus { get; set; }
        [Display(Name = "Подорожание")]
        public decimal? Minus { get; set; }
        [Display(Name = "Эффективность")]
        public decimal Saving { get; set; }
    }
}