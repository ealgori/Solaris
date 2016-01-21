using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.SAT
{
    public class TOToSHHandler : ATaskHandler
    {
        public TOToSHHandler(TaskParameters taskParameters) : base(taskParameters) { }


        public override bool Handle()
        {
            TORepository repository = new TORepository(TaskParameters.Context);
            var toList = repository.GetLastSATTOList().Where(t=>!t.UploadedToSh&&string.IsNullOrEmpty(t.ShComment));
            foreach (var to in toList)
            {
                var vidTOTotalAmmount = new List<VidTOTotalAmount>();
                vidTOTotalAmmount.Add(new VidTOTotalAmount(){
                 TO = to.TO,
                  Activity = to.Activity,
                   TotalPrice = to.Total,
                    User = to.CreateUserName,
                     StoimostMaterialov = to.TotalMaterials,
                     StoimostRabot = to.TotalServices
                });

                var items = to.SATTOItems.Where(i=>i.Type=="Service").Select(i=>new VidTOItemsUpd(){
                 ItemId= i.TOItemId,
                  PLDescription= i.Description,
                   PLItemId = i.PriceListRevisionItem.Id,
                   PLItemRevisionID = i.PriceListRevision.Id,
                   // 23.05.2014 почему то раньше грузили прайс. Правильно прогружать цену за единицу 
                   PLPrice = i.PricePerItem,
                    Quantity = i.Quantity
                }).ToList();

                var itemsMat = to.SATTOItems.Where(i => i.Type == "Material"&& i.PriceListRevisionItem!=null).Select(i => new TOMatItemUpd()
                {
                    ItemId = i.MatTOItemId,
                    Description = i.Description,
                    PLItemId = i.PriceListRevisionItem.Id,
                    PLItemRevisionID = i.PriceListRevision.Id,
                    Price = i.PricePerItem,
                    Unit = i.Unit

                }).ToList();
                // придется отказаться от встроенного функционала импорта и частично имплементить его здесь.
           //    TaskParameters param = new TaskParameters();
             //  param.Context = TaskParameters.Context;
            //   param.DbTask = TaskParameters.DbTask;
                TaskParameters.ImportHandlerParams = new ImportHandlerParams();
               
               ImportParams iParam = new ImportParams();
               iParam.Objects = new System.Collections.ArrayList(vidTOTotalAmmount);
               iParam.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1;
               TaskParameters.ImportHandlerParams.ImportParams.Add(iParam);

               ImportParams iParam1 = new ImportParams();
               iParam1.Objects = new System.Collections.ArrayList(items);
               iParam1.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2;
               TaskParameters.ImportHandlerParams.ImportParams.Add(iParam1);

               ImportParams iParam2 = new ImportParams();
               iParam2.Objects = new System.Collections.ArrayList(itemsMat);
               iParam2.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3;
               TaskParameters.ImportHandlerParams.ImportParams.Add(iParam2);

                var importHandler = new ImportHandlers.ImportHandler(TaskParameters);
                var satTO = TaskParameters.Context.SATTOs.Find(to.Id);
                satTO.ShUploadDate = DateTime.Now;
                if (importHandler.Import())
                {
                    satTO.ShComment = "success";
                    satTO.UploadedToSh=true;

                }
                else
                {
                    satTO.ShComment = "failed";
                }
                TaskParameters.Context.SaveChanges();
                
            }
            return true;
        }

        public class VidTOTotalAmount
        {
            public string TO { get; set; }
            public decimal TotalPrice { get; set; }
            public string User { get; set; }
            public string Activity { get; set; }
            public decimal StoimostMaterialov { get; set; }
            public decimal StoimostRabot { get; set; }
        }

        public class VidTOItemsUpd
        {
            public string ItemId { get; set; }
            public decimal PLPrice { get; set; }
            public int PLItemId { get; set; }
            public string PLDescription { get; set; }
            public int PLItemRevisionID { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class TOMatItemUpd
        {
            public string ItemId { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public decimal Price { get; set; }
            public int PLItemId { get; set; }
            public int PLItemRevisionID { get; set; }
        }
    }
}
