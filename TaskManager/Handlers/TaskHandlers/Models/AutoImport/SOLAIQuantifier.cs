using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.ImportQuantifiers;
using TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers;
using AutoImport.Rev3.ImportHandlers.Abstract;
using AutoImport.Rev3.DomainModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport
{
    public class SOLAIQuantifier : IImportQuantifier
    {

        public IAutoImportHandler Quantify(Attachment attachment, string project)
        {
           switch(attachment.TemplateName)
           {
               case"TOSubcImport":
                   {
                       return new TOImportHandler();
                       
                   }
               case "AutoTOItemAppove":
                    {
                        return new TOApproveHandler();
                    }
               case "Invoice_PstngPmntClearing":
                    {
                        return new InvoiceUpdateImportHandler();
                    }
               case "AISyberia":
                    {
                        return new AISyberiaHandler();
                    }
               case "Auto_AVRRecipients":
                    {
                        return new AVRRecipientsHandler();
                    }
               case "AddAgreement":
                    {
                        return new AgreementAIHandler();
                    }
               case "Auto_ActCreate":
                   {
                       return new ActCreateAutoimportHandler();
                   }
               case "AutoImportTOItemAppove":
                   {
                       return new TOItemApproveAiHandler();

                   }

           }
           return null;
        }
    }
}
