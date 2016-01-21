using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using DbModels.DomainModels.SAT;

namespace TaskManager.Handlers.TaskHandlers.Models.SAT
{
    public class ActToSHHandler : ATaskHandler
    {
        public ActToSHHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
         
            
            // сначала найдем акты с пересекающимися айтемами, в этой сессии.
            var crossedItemActs = TaskParameters.Context.SATActs
                .Where(a => !a.UploadedToSHDate.HasValue)
                .SelectMany(s => s.SATActItems.Where(i => i is SATActService))
                .GroupBy(g => g.ShId)
                .Where(g => g.Count() > 1).ToList();
            // для этих актов в сате надо проставить ошибку


            if (crossedItemActs.Count() > 0)
            {
                foreach (var acts in crossedItemActs)
                {
                    var actIds = acts.Select(a => a.SATAct.Id ).ToList();

                    var unuploadableActs = TaskParameters.Context.SATActs.Join(actIds, sa => sa.Id, a => a, (sa, a) =>  new{ sa, a });
                    string actNames = string.Join(",", unuploadableActs.Select(a => a.sa.ActName));
                    foreach (var ua in unuploadableActs)
                    {
                        ua.sa.UploadedToSHDate = DateTime.Now;
                        ua.sa.UploadToSHComment = string.Format("Акты для загрузки содержат пересекающиеся элементы и не будут загружены в сх:{0}", actNames);
                    }

                }
            }
            TaskParameters.Context.SaveChanges();

            var actForUpload = TaskParameters.Context.SATActs.Where(a => !a.UploadedToSHDate.HasValue&& a.Id!=-12).ToList();
            foreach (var act in actForUpload)
            {
                bool checkSucced = true;
                var temporaryServiceList = new List<string>();
                var temporaryDeleteActList = new List<string>();
                var temporaryMaterialList = new List<string>();

                // пробегаемся по всем айтемам акта. сначала по сервисам
                foreach (var serv in act.SATActItems.Where(i => i is SATActService).ToList())
                {
                    // находим в сх этот айтем и проверяем, подвязан ли он
                    var shItem = TaskParameters.Context.ShTOItems.Find(serv.ShId);
                    if (shItem != null)
                    {
                        // если он подвязан, надо првоерить, заморожен ли акт
                        var shAct = TaskParameters.Context.ShActs.Find(shItem.ActId);
                        if (shAct != null)
                        {
                            // если он заможрожен, надо проставить ошибку в САТ
                            if (shAct.GetActLink)
                            {
                                act.UploadedToSHDate = DateTime.Now;
                                act.UploadToSHComment = string.Format("У создаваемого акта {0} если элемент {1} который содержится в другом замороженном акте:{2}. Загрузка невозможна", act.ActName, shItem.TOItem, shItem.ActId);
                                checkSucced = false;
                                break;
                            }
                            else
                            {
                                // если же он не заморожен, его надо удалить
                                temporaryDeleteActList.Add(shAct.Act);
                               
                            }
                        }
                        // и перепривезать айтем
                        temporaryServiceList.Add(shItem.TOItem);
                       
                    }
                }
                if (!checkSucced)
                {
                    continue;
                }
                else
                {
                    List<CreateActImportModel> createdActsImport = new List<CreateActImportModel>();
                    List<UpdateActItemImportModel> updateActItemModel = new List<UpdateActItemImportModel>();
                    List<UpdateActItemImportModel> updateActMatModel = new List<UpdateActItemImportModel>();
                    List<DeleteActImportModel> deleteActImportModel = new List<DeleteActImportModel>();
                   
                    // обновляемые айтемы
                    updateActItemModel.AddRange(
                        temporaryServiceList.Select(s => new UpdateActItemImportModel() { ActId = act.ActName, ItemId = s })

                        );
                    deleteActImportModel.AddRange(
                        temporaryDeleteActList.Select(s => new DeleteActImportModel() { ActName = s })
                        );

                    var materials = act.SATActItems.Where(i => i is SATActMaterial).ToList();
                    updateActMatModel.AddRange(
                        materials.Select(m => new UpdateActItemImportModel() { ActId = act.ActName, ItemId = m.ShId })
                        );
                    decimal matTotal = materials.Sum(m => m.Quantity * m.Price);

                    var serv = act.SATActItems.Where(i => i is SATActService).ToList();
                    decimal servTotal = serv.Sum(s => s.Quantity * s.Price);

                    decimal total = matTotal + servTotal;

                    // если все успешно, то надо создать акт, и перегнать все временные данные в данные для прогрузки
                    createdActsImport.Add(new CreateActImportModel()
                    {
                        TO = act.TO,
                        ActId = act.ActName,
                        EndDate = act.EndDate,
                        StartDate = act.StartDate,
                        MatTotalAmmount = matTotal,
                        ServTotalAmmount = servTotal,
                        TotalAmmount = total
                    });
                    
                    TaskParameters.ImportHandlerParams = new ImportHandlerParams();

                    ImportParams iParam3 = new ImportParams();
                    iParam3.Objects = new System.Collections.ArrayList(deleteActImportModel);
                    iParam3.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName4;
                    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam3);


                    ImportParams iParam = new ImportParams();
                    iParam.Objects = new System.Collections.ArrayList(createdActsImport);
                    iParam.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1;
                    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam);

                    ImportParams iParam1 = new ImportParams();
                    iParam1.Objects = new System.Collections.ArrayList(updateActItemModel);
                    iParam1.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2;
                    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam1);

                    ImportParams iParam2 = new ImportParams();
                    iParam2.Objects = new System.Collections.ArrayList(updateActMatModel);
                    iParam2.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3;
                    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam2);

                    var importHandler = new ImportHandlers.ImportHandler(TaskParameters);
                    // var satAct = TaskParameters.Context.SATActs.Find(act.ActName);
                    act.UploadedToSHDate = DateTime.Now;
                    if (importHandler.Import())
                    {
                        act.UploadedToSH = true;
                    }
                    else
                    {
                        act.UploadedToSH = false;
                    }
                    TaskParameters.Context.SaveChanges();

                }



            }














            //    createdActsImport.Add(new CreateActImportModel() {
            //    ActId = act.ActName,
            //    StartDate = act.StartDate,
            //    EndDate = act.EndDate,
            //    TO = act.TO


            //    });
            //    var actServices = act.SATActItems.Where(a => a is SATActService);
            //    if (actServices.Count() > 0)
            //    {
            //        updateActItemModel.AddRange(
            //            actServices.Select(s => new UpdateActItemImportModel() { ActId = act.ActName, ItemId = s.ShId })
            //            );
            //    }
            //    var actMaterials = act.SATActItems.Where(a => a is SATActMaterial);
            //    if (actMaterials.Count() > 0)
            //    {
            //        updateActMatModel.AddRange(
            //            actMaterials.Select(m => new UpdateActItemImportModel() { ActId = act.ActName, ItemId = m.ShId })
            //            );
            //    }

            //    TaskParameters.ImportHandlerParams = new ImportHandlerParams();

            //    ImportParams iParam = new ImportParams();
            //    iParam.Objects = new System.Collections.ArrayList(createdActsImport);
            //    iParam.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1;
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam);

            //    ImportParams iParam1 = new ImportParams();
            //    iParam1.Objects = new System.Collections.ArrayList(updateActItemModel);
            //    iParam1.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2;
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam1);

            //    ImportParams iParam2 = new ImportParams();
            //    iParam2.Objects = new System.Collections.ArrayList(updateActMatModel);
            //    iParam2.ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3;
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(iParam2);

            //    var importHandler = new ImportHandlers.ImportHandler(TaskParameters);
            //   // var satAct = TaskParameters.Context.SATActs.Find(act.ActName);
            //    act.UploadedToSHDate = DateTime.Now;
            //    if (importHandler.Import())
            //    {
            //        act.UploadedToSH = true;
            //    }
            //    else
            //    {
            //        act.UploadedToSH = false;
            //    }
            //    TaskParameters.Context.SaveChanges();

            // }
            return true;

        }

        private class CreateActImportModel
        {
            public string ActId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string SendInformation { get; set; }
            public string SendStatus { get; set; }
            public string ActLink { get; set; }
            public string TO { get; set; }
            public decimal ServTotalAmmount { get; set; }
            public decimal MatTotalAmmount { get; set; }
            public decimal TotalAmmount { get; set; }
        }

        private class UpdateActItemImportModel
        {
            public string ItemId { get; set; }
            public string ActId { get; set; }
        }

        public class DeleteActImportModel
        {
            public string ActName { get; set; }
        }




    }
}
