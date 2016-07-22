using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DomainModels.SAT;
using EDiadocApi.Models.AcceptModels;
using EDiadocApi.Models;

namespace ExcelParser.ExcelParser.TOAct
{
    public class XLSFormatActGen : IActGenerator
    {

        public XLSFormatActGen(string orgId, SATAct satAct, string workDescription, string siteAddress)
        {
            this.OrgId = orgId;
            this.SatAct = satAct;
            this.WorkDescription = workDescription;
            this.Address = siteAddress;

        }

        public string Address { get; private set; }
        public string OrgId { get; private set; }
        public SATAct SatAct { get; private set; }
        public string WorkDescription { get; private set; }

        public Tuple<string, Stream> Generate(List<SATActService> actServices, List<SATActMaterial> actMaterials)
        {
            var eDiadoc = new EDiadocApi.EDiadocApi();
            var org = eDiadoc.GetOrganizationList().FirstOrDefault();
            var acceptInfo = new AcceptanceInfo();
            acceptInfo.ActDate = SatAct.CreateDate;
            acceptInfo.DocDateTime = SatAct.CreateDate;
            acceptInfo.DocNumber = SatAct.ActName;
            acceptInfo.ActHeader = SatAct.WorkDescription;

            var workInfoList = new WorkInfoList(
               SatAct.WOVAT ? EDiadocApi.NDS.NDSStrategy.WithoutNDSStrategy : EDiadocApi.NDS.NDSStrategy.WithNDSStarategy);

            var works = actServices.Select(s =>
                new WorkInfo
                {
                    Description = s.Description,
                    Price = s.Price,
                    Quantity = s.Quantity,
                    Units = s.Unit
                }

            ).ToList();

            workInfoList.WorkInfos = works;

            var execInfo = new ExecutorInfo()
            {
                FirstName = "Кто-то",
                MiddleName = "Кто-то",
                LastName = "Кто-то",
                Position = "Кто-то",
                ExecDate = SatAct.CreateDate

            };

            var siteInfo = new SiteInfo()
            {
                SiteAddress = Address  
            };



            var xml = eDiadoc.GenerateAcceptanceXml(org, acceptInfo, workInfoList, execInfo, siteInfo);
            var stream = new MemoryStream();
            xml.Save(stream);
            return new Tuple<string,Stream>( $"Act{SatAct.ActName}.xml", stream);
                
        }
    }
}
