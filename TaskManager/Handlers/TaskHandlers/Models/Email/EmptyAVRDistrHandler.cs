using MailProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class EmptyAVRDistrHandler : ATaskHandler
    {
        public EmptyAVRDistrHandler(TaskParameters taskParams) : base(taskParams)
        {
        
        }

        public override bool Handle()
        {
            var startDate = new DateTime(2016, 6, 1);
            var emptyAVRs = TaskParameters.Context.ShAVRs
                .Where(c => c.ObjectCreateDate > startDate)
                .Where(c => c.Items.Count == 0)
                .GroupBy(g => g.Subregion);
            ;
            RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");
            var filials = TaskParameters.Context.ShFilialStruct.ToList();
            foreach (var filial in filials)
            {
                var avrs = emptyAVRs.Where(a => a.Key == filial.Name).ToList();
                if (avrs.Count > 0)
                {
                    var avrsGrByCreated = avrs.SelectMany(s => s).GroupBy(g => g.CreatedByEmail);
                    foreach (var avrGr in avrsGrByCreated)
                    {
                        string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrGr.Select(s => s.AVRId))}";
                        TaskParameters.EmailHandlerParams.Add(new List<string> { avrGr.Key }, null, "Заявки без состава работ", true, body, null);
                    }


                    // сводна рук филиала
                    var rukFils = filial.RukFills.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    // здесь будем осбирать валидный список рукфилов
                    List<string> rukFilsList = new List<string>();
                    if (rukFils.Count > 0)
                    {
                        foreach (var ruk in rukFils)
                        {
                            if (CommonFunctions.StaticHelpers.IsValidEmail(ruk))
                            {
                                rukFilsList.Add(ruk);
                            }
                            else
                            {
                                var email = interactor.GetUserEmail(ruk);
                                if (!string.IsNullOrEmpty(email) && CommonFunctions.StaticHelpers.IsValidEmail(email))
                                {
                                    rukFilsList.Add(email);
                                }
                            }
                        }
                    }
                    if (rukFilsList.Count > 0)
                    {
                        string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrs.SelectMany(s => s).Select(s => s.AVRId))}";
                        TaskParameters.EmailHandlerParams.Add(rukFilsList, null, $"Заявки без состава работ ({filial.Name})", true, body, null);
                    }

                }









            }
            return true;
        }
    }
}
