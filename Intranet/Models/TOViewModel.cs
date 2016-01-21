using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class TOViewModel
    {
        public string TO { get; set; }
        [Display(Name = "Подрядчик")]
        public string Subcontractor { get; set; }
        [Display(Name = "Регион")]
        public string Region { get; set; }
        [Display(Name = "Тип ТО")]
        public string Type { get; set; }
        [Display(Name = "Активность")]
        public string Activity { get; set; }
        [Display(Name = "Филиал (определяется по принадлежности первого сайта из списка)")]
        public string Brunch { get; set; }
        [Display(Name="Общая сумма")]
        public decimal? TotalPrice { get; set; }
        [Display(Name="Без НДС")]
        public bool WOVAT { get; set; }
        [Display(Name="Дата выпуска заказа (Заполняется в СХ)")]
        public DateTime? POIssueDate { get; set; }

        public List<DbModels.DataContext.Repositories.TORepository.TOItemViewModel> Items { get; set; }
        public List<DbModels.DataContext.Repositories.TORepository.MatItemViewModel> MatItems { get; set; }
    }
}