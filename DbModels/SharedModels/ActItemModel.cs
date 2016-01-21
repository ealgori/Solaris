using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.SharedModels
{
    public class ActItemModel
    {
        public string Id { get; set; }
        public decimal? Quantity { get; set; }
        public bool Checked { get; set; }
    }
}
