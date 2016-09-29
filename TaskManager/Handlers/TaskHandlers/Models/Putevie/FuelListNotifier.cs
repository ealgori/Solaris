using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Putevie
{
    public class FuelListNotifierHandler:ATaskHandler
    {
        public FuelListNotifierHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        private string mailText = @"<style>
pre {{
    font-size: 16px;
    font-family: 'Times New Roman', Times, serif;
}}
</style>
<pre>
responsibles:{2}; managers:{3}

Добрый день!

Прошу ответным письмом предоставить отчет по генерации за период {0} по городу {1}, для этого необходимо:      

1.       Заполнить отчет(включая страницу СЗ о расходе топлива)  в формате excel, название файла ФилиалММГГГГ.xls (где Филиал – филиал выполнения генерации, ММГГГГ – отчетные месяц и год)
2.       Распечатать отчет, добавить страницу с чеками ГСМ, на первой странице рядом с подписью ответственного написать фразу от руки «Отчет по генерации отправлен в бухгалтерию Эрикссон *указать число, месяц, год отправки и номер накладной*»
3.       Отсканировать путевой лист и лист с чеками ГСМ в формате pdf, название файла ФилиалММГГГГ.pdf (где Филиал– филиал выполнения генерации, ММГГГГ – отчетные месяц и год)
4.       Отправить на адрес technical.box.for.solaris@ericsson.com письмо с темой #генерация# вложив в него два файла: ФилиалММГГГГ.xls и ФилиалММГГГГ.pdf
5.       Отправить оригинал отчета по генерации в бумажном виде в бухгалтерию

Спасибо!

</pre>
";
        public override bool Handle()
        {
            var fListToSend = TaskParameters.Context.ShFuelLists.Where(f => f.Required == "Yes").ToList();
            foreach (var fList in fListToSend)
            {

                var responsibles = fList.Responsible?.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var managers = fList.Manager?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var cityParts = fList.Generator.Split(new string[] {"-"}, StringSplitOptions.RemoveEmptyEntries);
                if(cityParts.Length<2)
                    continue;
                var city = cityParts[1];

                var plParts = fList.FuelList.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if(plParts.Length<2)
                    continue;
                var plDate = $"{plParts[1].Substring(0,2)}.{plParts[1].Substring(2, 4)}";
            

                if (responsibles!=null&& (responsibles.Any()) ||(managers!=null && managers.Any()))
                    TaskParameters.EmailHandlerParams.Add(responsibles, managers, $"Отчет по генерации {city}", true
                        , string.Format(mailText, plDate, city, fList.Responsible??"None", fList.Manager??"None")
                        , null);
            }

            return true;
        }
    }
}
