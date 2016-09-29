using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.FileImportQuantifiers;
using TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomFiHandlers;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport
{
    public class SOLFIQuantifier : IFileImportQuantifier
    {
        public global::AutoImport.Rev3.FileImportHandlers.IFileImportHandler Quantify(string project, global::Models.AutoMail amail)
        {
            var parts = amail.Subject.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            // if (parts.Count() < 2)
            //    return null;
            // вторым параметром будет передавать тип атоимпорта
            string fiType = parts[1].ToUpper();
            string poType = parts[0].ToUpper();
            switch (fiType)
            {
                case "TOI":
                    {
                        return new TOIFIHandler();
                    }
                case "TOE":
                    {
                        return new TOItemFilesDownloadHandler();
                    }


            }
            switch
                (poType)
            {
                case "PO":
                    {
                        return new AVRFIHandler();
                    }
            }
            return null;

        }
    }
}
