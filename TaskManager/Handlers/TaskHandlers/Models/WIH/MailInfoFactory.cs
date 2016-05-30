using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIHInteract;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public static  class MailInfoFactory
    {
        private static WIHMailInformation GetDefaultInfo()
        {
            WIHMailInformation mailInf = new WIHMailInformation();
            mailInf.MailBoxSigmun = "ESOLARIS";
            mailInf.Project = "MS-SOLARIS";
            mailInf.Subject = "GR WIH Request";
            mailInf.Email = "technical.box.for.solaris@ericsson.com";
            mailInf.ResponsibleTeam = "ROD Sofia";
            mailInf.SystemComponent = "Other";
            mailInf.CertificationCode = "L2302RODSofia_AO";

            return mailInf;
        }

        public static WIHMailInformation GetGRInfo(string internalMailType, string filePath)
        {
            var info = GetDefaultInfo();
            info.InternalMailType = internalMailType;
            info.FilePath = filePath;
            return info;

        }






    }
}
