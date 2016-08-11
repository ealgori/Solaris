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

        #region MailTemplate
        string mailTemplate = @"<html> 

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
  </tr>
 ";

        #endregion


        /// <summary>
        /// выбираем заявки без позиций, узнаем почту их создателя и запиливаем их ему
        /// по стурктуре филиала узнаем руководителя филиала, узнаем его почту и запиливаем все заявки
        /// без позиций ему
        /// </summary>
        /// <returns></returns>
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
                        var builder = new StringBuilder();
                        foreach (var avr in avrGr)
                        {
                            builder.AppendLine(string.Format(rowTemplate
                                , avr.AVRId
                                , avr.TaskSubcontractorNumber
                                , avr.Subregion
                                , avr.Subcontractor
                                , avr.TotalAmount
                                , avr.Source
                                , avr.SourceNo
                                , avr.AddingsInfo
                                , avr.WorkStart.HasValue ? avr.WorkStart.Value.ToShortDateString() : ""
                                , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString() : ""
                                , avr.CreatedBy
                                ));
                        }
                        var message = string.Format(mailTemplate,builder.ToString());

                        //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrGr.Select(s => s.AVRId))}";
                        TaskParameters.EmailHandlerParams.Add(new List<string> { avrGr.Key }, addtest ? testRecipints : null, "Заявки без состава работ", true, message, null);
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
                        var builder = new StringBuilder();
                        foreach (var avr in avrs.SelectMany(s => s))
                        {
                            builder.AppendLine(string.Format(rowTemplate
                              , avr.AVRId
                               , avr.TaskSubcontractorNumber
                                , avr.Subregion
                                , avr.Subcontractor
                                , avr.TotalAmount
                                , avr.Source
                                , avr.SourceNo
                                , avr.AddingsInfo
                                , avr.WorkStart.HasValue ? avr.WorkStart.Value.ToShortDateString() : ""
                                , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString() : ""
                                , avr.CreatedBy
                                )
                                
                                );
                        }
                        var message = string.Format(mailTemplate, builder.ToString());

                        //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrs.SelectMany(s => s).Select(s => s.AVRId))}";

                        TaskParameters.EmailHandlerParams.Add(rukFilsList, addtest?testRecipints:null, $"Заявки без состава работ ({filial.Name})", true, message, null);
                    }

                }









            }
            return true;
        }
    }
}
