using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;

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
            var eMailModels = new List<TypeCheckClass>();
            var avr2016 = TaskParameters.Context.ShAVRs.Where(a =>
              !string.IsNullOrEmpty(a.Year)
              && a.ObjectCreateDate>minCreateDate
              ).ToList();
            //if(test)
            //{
            //    avr2016 = TaskParameters.Context.ShAVRs.Where(a => a.AVRId == "200001").ToList();
            //}
            var avrWithfLimit = avr2016.Where(a => a.Items.Any(i => i.Limit != null)).ToList();
            var withoutLimit = avr2016.Except(avrWithfLimit).
                Where(i=>i.Items.Count>0). // для исключения авр без позиций.их проверять пока еще рано.
                ToList();
            var inLimitAVRS = avrWithfLimit.Where(a =>
                                a.Items.Where(i => i.Limit != null)
                                .All(i =>
                                    i.InLimit.HasValue
                                    && i.InLimit.Value == true)).ToList();
            foreach (var avr in inLimitAVRS)
            {

                if (avr.AVRType != bidType00)
                {
                    if (string.IsNullOrEmpty(avr.TypeCheck))
                    {
                        importModels1.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType00, TypeCheck = bidChecked });
                    }
                    else
                    {
                        eMailModels.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType00, TypeCheck = bidChecked });
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
                        importModels1.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType04, TypeCheck = bidChecked });
                    }
                    else
                    {
                        eMailModels.Add(new TypeCheckClass { AVR = avr.AVRId, AVRType = bidType04, TypeCheck = bidChecked });
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
                var emailParam = new EmailParams(new List<string>() { DistributionConstants.EalgoriEmail, "dmitriy.b.egorov@ericsson.com" }, "AVR type change missmatch");
                emailParam.DataTables.Add("avr.xls",eMailModels.ToDataTable());
                emailParam.HtmlBody = "Тип этих АВР требует изменения, однако их анализ уже был произведен.";
                TaskParameters.EmailHandlerParams.EmailParams.Add(emailParam);
            }
              

            return true;


        }





        public class TypeCheckClass
        {
            public string AVR { get; set; }
            public string TypeCheck { get; set; }
            public string AVRType { get; set; }
        }

    }
}
