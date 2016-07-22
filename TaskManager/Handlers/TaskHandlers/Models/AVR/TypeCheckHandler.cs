using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using DbModels.DomainModels.ShClone;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class TypeCheckHandler : ATaskHandler
    {
        public TypeCheckHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        public override bool Handle()
        {
            bool test = false;
            string testAvr = "207945";
            string bidType04 = "04- Лимитированные работы (необходимо перевыставить Вымпелком)";
            string bidType00 = "00- Обычная (работы предусмотрены в контракте Вымпелком)";
            string bidChecked = "Checked";
            DateTime minCreateDate = new DateTime(2016, 4, 15);

            // берем все авр
            // выделяем те которые без лимитов - им надо просто статус о проверке.
            // выделяем которые с лимитами

            // выделяем те которые в рамках лимита
            // если тип авр не совпадает с 00 то проверяем checked
            // если уже был checked значит отправляем этот файл мне.
            // если не было, значит добавляем н прогрузку с новым типом и checked
            // если тип совпадает, то проверяем чекд и прогружаем при осутсвтии

            // выделяем те, которые за рамками лимита
            // те же действия
            var importModels1 = new List<TypeCheckClass>();
            var eMailModels = new Dictionary<string,List<EmailModel>>();
            var avr2016 = TaskParameters.Context.ShAVRs.Where(a =>
              !string.IsNullOrEmpty(a.Year)
              && a.ObjectCreateDate>minCreateDate
              ).ToList();
            if (test)
            {
                var contains = avr2016.Where(a=>a.AVRId== testAvr).ToList();
                avr2016 = TaskParameters.Context.ShAVRs.Where(a => a.AVRId == testAvr).ToList();

            }
            var avrWithfLimit = avr2016.Where(a => a.Items.Any(i => i.Limit != null)).ToList();
            var withoutLimit = avr2016.Except(avrWithfLimit).
                Where(i=>i.Items.Count>0). // для исключения авр без позиций.их проверять пока еще рано.
                ToList();
            var inLimitAVRS = avrWithfLimit.Where(a =>
                                a.Items.Where(i => i.Limit != null)
                                .All(i =>
                                    i.InLimit.HasValue
                                    && i.InLimit.Value == true)).ToList();
            if(test)
            {
                Func<ShAVRs, bool> equal = a => a.AVRId == testAvr;
                var withLim = avrWithfLimit.Where(equal);
                var woLim = withoutLimit.Where(equal);
                var inLim = inLimitAVRS.Where(equal);
            }


            foreach (var avr in inLimitAVRS)
            {

                if (avr.AVRType != bidType00)
                {
                    if (string.IsNullOrEmpty(avr.TypeCheck))
                    {
                        // 22.07.2016 - решили больше не менять типы авр
                        //importModels1.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType00, TypeCheck = bidChecked });
                    }
                    else
                    {
                        AddOrUpdateEmailDict(eMailModels, avr, $"Требует изменения на '{bidType00}'");
                       
                    }

                }
                else
                {
                    if(string.IsNullOrEmpty(avr.TypeCheck))
                    {
                        importModels1.Add(new TypeCheckClass { AVR = avr.AVRId, TypeCheck = bidChecked });
                    }
                }
            }
            var outOfLimit = avrWithfLimit.Except(inLimitAVRS).ToList();
            foreach (var avr in outOfLimit)
            {

                if (avr.AVRType != bidType04)
                {
                    if (string.IsNullOrEmpty(avr.TypeCheck))
                    {
                       // importModels1.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType04, TypeCheck = bidChecked });
                    }
                    else
                    {
                        AddOrUpdateEmailDict(eMailModels, avr, $"Требует изменения на '{bidType04}'");
                    }

                }
                else
                {
                    if (string.IsNullOrEmpty(avr.TypeCheck))
                    {
                        importModels1.Add(new TypeCheckClass { AVR = avr.AVRId,  TypeCheck = bidChecked });
                    }
                }
            }

            importModels1.AddRange(
                withoutLimit.Where(a => string.IsNullOrEmpty(a.TypeCheck))
                .Select(avr => 
                    new TypeCheckClass { AVR = avr.AVRId, TypeCheck = bidChecked }));

            if (!test)
            {
                if (importModels1.Count > 0)
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels1) });
            }
            if (eMailModels.Count > 0)
            {
                foreach (var kvp in eMailModels)
                {
                    var emailParam = new EmailParams(new List<string>() { kvp.Key }, "AVR type change missmatch");
                    emailParam.DataTables.Add("avr.xls", kvp.Value.ToDataTable());
                    emailParam.HtmlBody = "Тип этих АВР требует изменения.";
                    TaskParameters.EmailHandlerParams.EmailParams.Add(emailParam);
                }

              
            }
              

            return true;


        }


        private void AddOrUpdateEmailDict(Dictionary<string,List<EmailModel>> dict, ShAVRs avr, string message)
        {
            var emailModel = new EmailModel { AVR = avr.AVRId, Message = message };
            if (!string.IsNullOrEmpty(avr.CreatedByEmail))
            {
                if(dict.Keys.Contains(avr.CreatedByEmail))
                {
                    dict[avr.CreatedByEmail].Add(emailModel);
                }
                else
                {
                    dict.Add(avr.CreatedByEmail, new List<EmailModel> { emailModel });
                }
            }
            if (!string.IsNullOrEmpty(avr.RukOtdelaEmail))
            {
                if (dict.Keys.Contains(avr.RukOtdelaEmail))
                {
                    dict[avr.RukOtdelaEmail].Add(emailModel);
                }
                else
                {
                    dict.Add(avr.RukOtdelaEmail, new List<EmailModel> { emailModel });
                }
            }

        }





        public class TypeCheckClass
        {
            public string AVR { get; set; }
            public string TypeCheck { get; set; }
            public string AVRType { get; set; }
        }

        public class EmailModel
        {
            public string AVR { get; set; }
           
            public string Message { get; set; }
        }

    }
}
