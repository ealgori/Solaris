using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.ShClone
{
    public class ShContact
    {
        [Key]
        public string Contact { get; set; }
        public string EMailAddress { get; set; }
        public string SubcFace { get; set; }
        public bool WithOutVAT { get; set; }
        public string ActUploadEmail { get; set; }
        public string ActFIO { get; set; }

    }
}
