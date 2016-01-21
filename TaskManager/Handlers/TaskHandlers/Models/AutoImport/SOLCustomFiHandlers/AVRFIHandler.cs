using AutoImport.Rev3.FileImportHandlers;
using DbModels.DataContext;
using SHInteract.Handlers.Solaris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomFiHandlers
{
    public class AVRFIHandler:IFileImportHandler
    {
        public HandlerResult Handle(global::Models.AutoMail amail)
        {
            HandlerResult result = new HandlerResult();
            var parts = amail.Subject.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() <2)
            {
                result.Success = false;
                result.ErrorsList.Add("В теме письма должны быть указаны  номер ТО и сайт. А так же, по желанию, плановая дата.");
                return result;

            }
            using (Context context = new Context())
            {

                var po = parts[1];
                var shAvr = context.ShAVRs.FirstOrDefault(a=>a.PurchaseOrderNumber == po);
                if (shAvr == null)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("PO не существует:{0}", po));
                    return result;

                }


             

                if (amail.Attachments.Count != 1)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("Возможен импорт только 1 файлов."));
                    return result;
                }
                result.InfoList.Add(string.Format("AVR:{0}",shAvr));
                string impResult = AVRFileUploaderSol.Handle(amail.Attachments.Select(f => f.FilePath).FirstOrDefault(), shAvr.AVRId);
                result.InfoList.Add(impResult);
                result.Success = true;


            }
            return result;
        }
    }
}
