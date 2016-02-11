using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.SAT
{
    /// <summary>
    /// Запросы для создания их в сх, после опрайсовки Катей
    /// </summary>
    public class VCRequestToCreate
    {
        public int Id { get; set; }
        public string AVRId { get; set; }
        public string VCRequestNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public string UserName { get; set; }

    }
}
