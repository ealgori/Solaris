using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.DataContext;
using DbModels.DataContext;

namespace TaskManager.Service
{
    public static class WIHService
    {
        /// <summary>
        /// Функционал проверяет, можно ли слать новый запрос ТО. Нет ли раскорячившихся или не завершенных запросов
        /// </summary>
        /// <param name="TO"></param>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <param name="agreement">Номер эгримента. У вих реквеста появилось новое поле.</param>
        /// <returns></returns>
        public static  bool ReadySendTOWIHRequest(string TO, string type,  Context context, string agreement=null)
        {

            switch (type)
            {
                case WIHInteract.Constants.InternalMailTypeTOPOR:
                    {
                        return TORequestCanBeSended(TO,type, context);
                    }
                case WIHInteract.Constants.InternalMailTypeTOPORDel:
                    {
                        return TORequestCanBeSended(TO, type, context, agreement);
                    }
                case WIHInteract.Constants.InternalMailTypeTORecall:
                    {
                        var to = context.ShTOes.FirstOrDefault(t => t.TO == TO);
                        if (to == null)
                            return false;
                        else
                        {
                            if (string.IsNullOrEmpty(to.PONumber))
                            {
                                return false;
                            }
                            else
                            {
                                // если нужен рекол на агримент, то доблавляем сюда еще один параметр агримент
                                return TORequestCanBeSended(TO, WIHInteract.Constants.InternalMailTypeTORecall,context);
                            }
                                
                        }
                    }
            }



            return false;

        
        }

        //private static bool TORequestCanBeSended(string TO, string type, Context context)
        //{
        //    var toRequests = context.ShWIHRequests.Where(r => r.TOid == TO&& r.Type== type);
        //    //0 если нет таких запросов запросов, то отправляем
        //    if (toRequests.Count() == 0)
        //        return true;
        //    //1 если среди них есть хоть один комплитед, то досвидания
        //    if (toRequests.Any(t => t.CompletedByOD.HasValue))
        //    {
        //        return false;
        //    }
        //    //2 если есть запрос у которого нет комплитеда и реджекта, то досвиданься
        //    if (toRequests.Any(t => (!t.RejectedByOD.HasValue) && (!t.RejectedByOD.HasValue)))
        //    {
        //        return false;
        //    }
        //    //3 если есть реджекты с корректедом то наш клиент. условия выше отфильтруют негативные условия
        //    if (toRequests.Any(t => t.RejectedByOD.HasValue && t.CorrectionCompleted))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        private static bool TORequestCanBeSended(string TO, string type, Context context, string agreement=null)
        {
            var requests = context.ShWIHRequests.Where(r => r.TOid == TO ).ToList();
            if(string.IsNullOrEmpty(agreement))
                requests = requests.Where(r=>string.IsNullOrEmpty(r.AddAgreementId)).ToList();
            else
                requests = requests.Where(r=>r.AddAgreementId==agreement).ToList();

            var toRequest = requests.OrderByDescending(r => r.RequestSentToODdate).FirstOrDefault();
            //0 если нет таких запросов запросов, то отправляем
            if (toRequest==null)
                return true;
            //1 если среди них есть хоть один комплитед того же типа, то досвидания
            if (toRequest.CompletedByOD.HasValue&& toRequest.Type==type)
            {
                return false;
            }
            //2 если есть запрос у которого нет комплитеда и реджекта, то досвиданься
            if (!toRequest.RejectedByOD.HasValue && !toRequest.CompletedByOD.HasValue)
            {
                return false;
            }
            //3 если есть реджекты с корректедом то наш клиент. условия выше отфильтруют негативные условия
            if (toRequest.RejectedByOD.HasValue && toRequest.CorrectionCompleted&& toRequest.Type==type)
            {
                return true;
            }
            //если он комплитед, и не того же типа, что мы собираемся отправлять
            if (toRequest.CompletedByOD.HasValue && toRequest.Type != type)
            {
                return true;
            }
            return false;
        }


        public static bool TOHasCompletedRequest(string TO, string Type, Context context)
        {
            var requests = context.ShWIHRequests.Where(r => 
                r.TOid == TO 
                && string.IsNullOrEmpty(r.AddAgreementId)
                && r.Type == Type
                );
            if(requests.Any(r=>r.CompletedByOD.HasValue))
            {
                return true;
            }
            return false;
        }
    }
}
