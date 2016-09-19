using MailProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DomainModels.ShClone;
using EpplusInteract;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class EmptyAVRDistrHandler : ATaskHandler
    {

        private string _destination;

        public EmptyAVRDistrHandler(TaskParameters taskParams, string destination) : base(taskParams)
        {
            this._destination = destination;
        }


        #region MailTemplate
        string mailTemplate = @"<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:w=""urn:schemas-microsoft-com:office:word"" xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml"" xmlns=""http://www.w3.org/TR/REC-html40""><head><meta http-equiv=Content-Type content=""text/html; charset=utf-8"">

<style>
table, th, td {{
   border: 1px solid black;
}}
</style>
На данный момент в SH заведены следующие заявки без состава работ:

<table>
  <thead>
 <tr>
<th>№ Заявки</th>
<th>№ Заявки подрядчика</th>
<th>Филиал</th> 
<th>Подрядчик</th>
<th>Стоимость работ</th>
<th>Основание</th>
<th>Номер тикета</th>
<th>Что нужно сделать</th>
<th>Дата начала выполнения работ</th>
<th>Дата окончания выполенния работ</th>
<th>Создал</th>
<th>Ответственный</th>
  </tr>
  </thead>
  <tbody>
  {0}
  </tbody>
</table>

</div>

</body>

</html>";

        string rowTemplate = @" 
  <tr>
     <td>{0}</td>
<td>{1}</td>
<td>{2}</td>
<td>{3}</td>
<td>{4}</td>
<td>{5}</td>
<td>{6}</td>
<td>{7}</td>
<td>{8}</td>
<td>{9}</td>
<td>{10}</td>
<td>{11}</td>
  </tr>
 ";

        #endregion



        public override bool Handle()
        {
            bool addtest = true;
            var testRecipints = new List<string> {
                DistributionConstants.EalgoriEmail
                , DistributionConstants.EgorovEmail
            };

            var startDate = new DateTime(2016, 6, 22);
            var emptyAVRs = TaskParameters.Context.ShAVRs
                .Where(c => c.ObjectCreateDate > startDate)
                .Where(c => c.Items.Count == 0)
                .Where(a => string.IsNullOrEmpty(a.RefAVR))
               // .GroupBy(g => g.Subregion);
              // .Where(a => a.Subregion == "VC MS Ural Izhevsk")
            ;

            // считываем файл с ответственнымими
            // пробегаем по каждой и выводим ответственного и мэнеджера. и отв и менеджеров бьем по ; чтоб получились отдельне записи
            // группируем по ответсвтенным и рассылаем
            // группируем по менеджерам и рассылаем

            if (!File.Exists(TaskParameters.DbTask.TemplatePath))
            {
                return false;
            }
            var emailList = new List<DistrItem>();
            var sendToResponsible = new List<SendToResp>();

            var wsObjs = EpplusSimpleUniReport.ReadFile(TaskParameters.DbTask.TemplatePath, "DRT", 2);
            foreach (var avr in emptyAVRs)
            {
                // фильтр по региону
                var filtered = wsObjs.Where(r => r.Column1 == avr.Subregion).ToList();
                // фильтр по типу
                if (!string.IsNullOrEmpty(avr.AVRType3))
                {
                    filtered = filtered.Where(a => a.Column3 == avr.AVRType3).ToList();
                }
                else
                {
                    filtered = filtered.Where(r => string.IsNullOrEmpty(r.Column3)).ToList();
                }

                //фильтр по подрядчику
                if (!string.IsNullOrEmpty(avr.Subcontractor))
                {
                    filtered = filtered.Where(a => a.Column2 == avr.Subcontractor).ToList();
                }
                else
                {
                    filtered = filtered.Where(r => string.IsNullOrEmpty(r.Column2)).ToList();
                }

                var rows = filtered;
                if (rows.Count > 0)
                {
                    var responsibleStr = string.Join(";", rows.Select(s => s.Column4)).ToLower();
                    var responsibles =
                         responsibleStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                    var responsible = string.Join(";", responsibles);

                    switch (_destination)
                    {
                        case "responsibles":
                            {
                              
                                if (!avr.SendToResp.HasValue && responsibles.Any())
                                {

                                    foreach (var resp in responsibles)
                                    {
                                        if (!emailList.Any(l => l.Email == resp && l.ShAvRs.AVRId == avr.AVRId))
                                        {
                                            emailList.Add(new DistrItem { ShAvRs = avr, Email = resp, Responsible = responsible });
                                            sendToResponsible.Add(new SendToResp() { AVR =  avr.AVRId, SendToResponsible = DateTime.Now});
                                        }
                                    }
                                }
                                break;
                            }
                        case "managers":
                        {

                                var managerStr = string.Join(";", rows.Select(s => s.Column5));
                                var managers = managerStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                                if (managers.Any())
                                {
                                    var manager = String.Join(";", managers);
                                    foreach (var manag in managers)
                                    {
                                        if (!emailList.Any(l => l.Email == manag && l.ShAvRs.AVRId == avr.AVRId))
                                        {
                                            emailList.Add(new DistrItem { ShAvRs = avr, Email = manag, Responsible = responsible });
                                        }
                                    }
                                }
                                break;
                        }

                    }


                 
                }
            }


            var distrGroup = emailList.GroupBy(g => new { g.Email, g.ShAvRs.Subregion }).ToList();
            foreach (var group in distrGroup)
            {
                var builder = new StringBuilder();
                foreach (var avr in group.Select(s => s))
                {
                    builder.AppendLine(string.Format(rowTemplate
                      , avr.ShAvRs.AVRId
                       , avr.ShAvRs.TaskSubcontractorNumber
                        , avr.ShAvRs.Subregion
                        , avr.ShAvRs.Subcontractor
                        , avr.ShAvRs.TotalAmount
                        , avr.ShAvRs.Source
                        , avr.ShAvRs.SourceNo
                        , avr.ShAvRs.AddingsInfo
                        , avr.ShAvRs.WorkStart.HasValue ? avr.ShAvRs.WorkStart.Value.ToShortDateString() : ""
                        , avr.ShAvRs.WorkEnd.HasValue ? avr.ShAvRs.WorkEnd.Value.ToShortDateString() : ""
                        , avr.ShAvRs.CreatedBy
                        , avr.Responsible
                        )

                        );
                }
                var message = string.Format(mailTemplate, builder.ToString());
                TaskParameters.EmailHandlerParams.Add(new List<string> { group.Key.Email }, addtest ? testRecipints : null, $"Заявки без состава работ ({group.Key.Subregion})", true, message, null);

            }





            //RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");








            //var filials = TaskParameters.Context.ShFilialStruct.ToList();
            //foreach (var filial in filials)
            //{
            //    var avrs = emptyAVRs.Where(a => a.Key == filial.Name).ToList();
            //    if (avrs.Count > 0)
            //    {
            //        var avrsGrByCreated = avrs.SelectMany(s => s).GroupBy(g => g.CreatedByEmail);
            //        foreach (var avrGr in avrsGrByCreated)
            //        {
            //            var builder = new StringBuilder();
            //            foreach (var avr in avrGr)
            //            {
            //                builder.AppendLine(string.Format(rowTemplate
            //                    , avr.AVRId
            //                    , avr.TaskSubcontractorNumber
            //                    , avr.Subregion
            //                    , avr.Subcontractor
            //                    , avr.TotalAmount
            //                    , avr.Source
            //                    , avr.SourceNo
            //                    , avr.AddingsInfo
            //                    , avr.WorkStart.HasValue ? avr.WorkStart.Value.ToShortDateString() : ""
            //                    , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString() : ""
            //                    , avr.CreatedBy
            //                    ));
            //            }
            //            var message = string.Format(mailTemplate,builder.ToString());

            //            //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrGr.Select(s => s.AVRId))}";
            //            TaskParameters.EmailHandlerParams.Add(new List<string> { avrGr.Key }, addtest ? testRecipints : null, "Заявки без состава работ", true, message, null);
            //        }


            //        // сводна рук филиала
            //        var rukFils = filial.RukFills.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //        // здесь будем осбирать валидный список рукфилов
            //        List<string> rukFilsList = new List<string>();
            //        if (rukFils.Count > 0)
            //        {
            //            foreach (var ruk in rukFils)
            //            {
            //                if (CommonFunctions.StaticHelpers.IsValidEmail(ruk))
            //                {
            //                    rukFilsList.Add(ruk);
            //                }
            //                else
            //                {
            //                    var email = interactor.GetUserEmail(ruk);
            //                    if (!string.IsNullOrEmpty(email) && CommonFunctions.StaticHelpers.IsValidEmail(email))
            //                    {
            //                        rukFilsList.Add(email);
            //                    }
            //                }
            //            }
            //        }
            //        if (rukFilsList.Count > 0)
            //        {
            //            var builder = new StringBuilder();
            //            foreach (var avr in avrs.SelectMany(s => s))
            //            {
            //                builder.AppendLine(string.Format(rowTemplate
            //                  , avr.AVRId
            //                   , avr.TaskSubcontractorNumber
            //                    , avr.Subregion
            //                    , avr.Subcontractor
            //                    , avr.TotalAmount
            //                    , avr.Source
            //                    , avr.SourceNo
            //                    , avr.AddingsInfo
            //                    , avr.WorkStart.HasValue ? avr.WorkStart.Value.ToShortDateString() : ""
            //                    , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString() : ""
            //                    , avr.CreatedBy
            //                    )

            //                    );
            //            }
            //            var message = string.Format(mailTemplate, builder.ToString());

            //            //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrs.SelectMany(s => s).Select(s => s.AVRId))}";

            //            TaskParameters.EmailHandlerParams.Add(rukFilsList, addtest?testRecipints:null, $"Заявки без состава работ ({filial.Name})", true, message, null);
            //        }

            //    }








            if (sendToResponsible.Any())
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(sendToResponsible) });
            }

            return true;
        }


        public class DistrItem
        {
            public ShAVRs ShAvRs { get; set; }
            public string Email { get; set; }
            public string Responsible { get; set; }

        }

        public class SendToResp
        {
            public string AVR { get; set; }
            public DateTime? SendToResponsible { get; set; }
        }
    }
}
