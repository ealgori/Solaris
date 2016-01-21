using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
	public class ShSITE
	{
        [Key]
        public string Site { get; set; }
        public string SiteName { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public string City { get; set; }
        public string MacroRegion { get; set; }
        public DateTime? DataVvodaVeqspluatatciu { get; set; }
        public string TipAMS  { get; set; }
        public decimal? KolvoAMS { get; set; }
        public string  VidTOAMS  { get; set; }
        public string MarkaSKV  { get; set; }
        public decimal? KolvoSKV  { get; set; }
        public string  TipSKV  { get; set; }
        public string VidTOSKV  { get; set; }
        public string TippSKV  { get; set; }
        public decimal? KolvopSKV  { get; set; }
        public string TipMobilnoiGU  { get; set; }
        public string TipStatcionarnoiGU  { get; set; }
        public decimal? KolvoStacionarnihGU { get; set; }
        public decimal? KolvoMobilnihGU { get; set; }
        public string TipAUGPT  { get; set; }
        public decimal?  KolvoAUGPT  { get; set; }
        public int? Index { get; set; }







	}
}
