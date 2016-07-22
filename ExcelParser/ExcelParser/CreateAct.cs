using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpplusInteract;
using DbModels.DataContext;
using System.IO;
using DbModels.DataContext.Repositories;
using CommonFunctions.Extentions;
using CommonFunctions;
using ExcelParser.ExcelParser.TOAct;
using System.IO.Compression;
using Ionic.Zip;

namespace ExcelParser.EpplusInteract
{
    public static class CreateAct
    {
        private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\AKT.xlsm";

        public static byte[] CreateActFile(int ActId, bool createXML = false)
        {
         
            using (Context context = new Context())
            {


                ActRepository repository = new ActRepository(context);
                //var _act = repository.GetReadyToPrintActs().FirstOrDefault(t => t.Id == ActId);
                var act = context.SATActs.Find(ActId);
                if (act != null)
                {
                    //var act = context.SATActs.Find(ActId);
                    if (!act.UploadedToSH)
                        return null;

                    var actServices = repository.GetSATActServices(act);
                    var actMaterials = repository.GetSATActMaterials(act);
                    var shTO = context.ShTOes.Find(act.TO);

                    string subcFace = "please fill in SH";
                    var shSubcontractor = context.SubContractors.FirstOrDefault(s => s.Name == act.SubContractor || s.ShName == act.SubContractor);
                    if (shSubcontractor != null)
                    {
                        var shContact = context.ShContacts.FirstOrDefault(s => s.Contact == shSubcontractor.ShName);
                        if (shContact != null && !string.IsNullOrWhiteSpace(shContact.SubcFace))
                        {
                            subcFace = shContact.SubcFace;
                        }

                    }
                    string siteBranch;
                    string siteAddress;
                    var _firstItem = actServices.FirstOrDefault();
                    var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == _firstItem.Site);
                    var shFOL = context.ShFOLs.FirstOrDefault(s => s.FOL == _firstItem.FOL);
                    if (shSite != null)
                    {
                        siteBranch =  shSite.Branch;
                        siteAddress = shSite.Address;
                    }
                    else
                    {
                        if (shFOL != null)
                        {
                            siteBranch = shFOL.Branch;
                            siteAddress = $"{shFOL.StartPoint}-{shFOL.DestinationPoint}";
                        }
                        else
                            throw new Exception($"Позиция {_firstItem.Id} не привязана ни к сайту ни к фолу");
                    }




                    List <IActGenerator> actGens = new List<IActGenerator>();
                    actGens.Add(new OldFormatActGen(TemplatePath, act, shTO.WorkDescription, subcFace, siteBranch));
                    if(createXML)
                        actGens.Add(new XLSFormatActGen(shSubcontractor.ShName,act,shTO.WorkDescription,siteAddress));

                    var fileStreams = new List<Tuple<string,Stream>>();

                    foreach (var gen in actGens)
                    {
                        fileStreams.Add(gen.Generate(actServices, actMaterials));
                    }
                    var zipStream = new MemoryStream();
                    using (ZipFile zipFile = new ZipFile())
                    {
                        int i = 0;
                        foreach (var stream in fileStreams)
                        {
                            i++;
                            stream.Item2.Position = 0;
                            zipFile.AddEntry(stream.Item1,  stream.Item2);
                        }

                        //   zipFile.Save(@"C:\Temp\test\test.zip");
                        zipFile.Save(zipStream);
                        zipStream.Position = 0;
                        return zipStream.GetBuffer();
                    }










                }
        }
           return null;
       }

    

}
}
