using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using AutoImport.Rev3;
using AutoImport.Rev3.FileImportHandlers;
using AutoImport.Rev3.FileImportQuantifiers;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport
{
    public class SOLAutoImport : ATaskHandler
    {
        public SOLAutoImport(TaskParameters taskParameters) : base(taskParameters) { }

        public override bool Handle()
        {
            //MainWorker.AutoImport("SOLARIS");
            // Передаем свои произвольные квантификаторы, которые умеют возвращать свои произвольные хэндлеры, которые умеют работать с контекстами
            List<string> mailMasks = null;
            if (DateTime.Now.Hour < 17 && DateTime.Now.Hour > 5)
                mailMasks = new List<string> {"autoimport"};

            //MainWorker.AutoImport(
            //    "SOLARIS"
            //    , new List<global::AutoImport.Rev3.ImportQuantifiers.IImportQuantifier>() { new SOLAIQuantifier() }
            //    , new List<IFileImportQuantifier> { new SOLFIQuantifier() }
            //    , mailMasks);

            MainWorker.AutoImport("SOLARIS", new List<global::AutoImport.Rev3.ImportQuantifiers.IImportQuantifier>() { new SOLAIQuantifier() }, new List<IFileImportQuantifier> { new SOLFIQuantifier() });
            return true;
        }
    }
}
