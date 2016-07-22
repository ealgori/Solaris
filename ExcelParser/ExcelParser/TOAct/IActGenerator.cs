using DbModels.DomainModels.SAT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelParser.ExcelParser.TOAct
{
    interface IActGenerator
    {
        Tuple<string,Stream> Generate(List<SATActService> actServices, List<SATActMaterial> actMaterials);
    }
}
