using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.ShClone
{
    public class ShAddAgreement
    {
        [Key]
        public string AddAgreement { get; set; }
        public bool SendAddAgreement { get; set; }
        public string ErrorCreationComment { get; set; }
    }
}
