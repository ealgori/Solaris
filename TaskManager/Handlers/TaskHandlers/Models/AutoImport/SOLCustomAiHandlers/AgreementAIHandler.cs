using AutoImport.Rev3.ImportHandlers.Abstract;
using DbModels.DataContext;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;
using DbModels.DomainModels.ShClone;
namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    public class AgreementAIHandler : IAutoImportHandler
    {
        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {

            // случайное удаление эгримента
            // присылается файл со след позициями
            // 1 позиция = 1 эгримент. 1 позиция привязана ко второму эгрименту не замороженному.
            // 2 позиция = 2 эгримент. 2 позиция привязана к первому эгрименту не замороженному.
            // в итоге создается файл на удаление 1 и 2 эгримента.
            // забьем на это
            
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // ��������� ����������
            var rows = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 2);
            // ������� ������� �� ������ �����

            if (rows == null || rows.Count == 0)
            {
                hr.ErrorsList.Add("������ ������ � ������. ��������� ��� ������ � ����������.");
            }
            var items = rows.Select(r => new SetAddAgreementItem() { ItemId = r.Column1, AddAgreement = r.Column2 }).Where(r=>!string.IsNullOrEmpty(r.AddAgreement));

            List<DeleteAddAggrementModel> deleteModels = new List<DeleteAddAggrementModel>();
            List<AddAddAgreementModel> createModels = new List<AddAddAgreementModel>();
            List<SetAddAgreementItem> bindModels = new List<SetAddAgreementItem>();
            string deleteImportName = "auto_AddAgreemDel";
            string addImportName = "auto_AddAgreemIns";
            string setImportName = "auto_AddAgreemBind";
            using (Context context = new Context())
            {
                var groupByAgreement = items.GroupBy(g => g.AddAgreement);
                foreach (var group in groupByAgreement)
                {
                   
                    var agreem = context.ShAddAgreements.FirstOrDefault(a => a.AddAgreement == group.Key);
                    // ���� ��� - �������. 

                    if(agreem==null)
                    {
                        agreem = new ShAddAgreement(){ AddAgreement= group.Key};
                        if(!createModels.Any(c=>c.AddAgreement==group.Key))
                            createModels.Add(new AddAddAgreementModel() { AddAgreement = group.Key });
                    }
                    foreach (var item in group)
                    {
                        var shToItem = context.ShTOItems.FirstOrDefault(i => i.TOItem == item.ItemId);
                        // ���� ����� ����, ���� ��������� �� �������� �� �� ��� � ���������
                        if(shToItem!=null)
                        {
                            // ���� �� ��� ��������, ���� ���������, �� ��������� �� ����������� ��������
                            if (!string.IsNullOrEmpty(shToItem.AddAgreementId))
                            {
                                var addAgreem = context.ShAddAgreements.FirstOrDefault(a => a.AddAgreement == shToItem.AddAgreementId);
                                if(addAgreem!=null)
                                {
                                    if(!addAgreem.SendAddAgreement)
                                    {
                                        // ���� �� �� ���������, �� ������� ���
                                        if(addAgreem.AddAgreement!=agreem.AddAgreement)
                                            if(!deleteModels.Any(d=>d.AddAgreement==addAgreem.AddAgreement))
                                                deleteModels.Add(new DeleteAddAggrementModel() { AddAgreement = addAgreem.AddAgreement });
                                    }
                                    else
                                    {
                                        // �������� � ���, ��� ����� ���������� ���������, ��� ��� �� �������� � ������������� ���������
                                        hr.ErrorsList.Add(string.Format("������� {0} �� ����� ���� ����������� � ��������� {1}, ��� ��� ��� ��� ��������� � ������������ ��������� {2}", shToItem.TOItem, group.Key, addAgreem.AddAgreement));
                                        continue;
                                    }
                                }
                                
                            }
                            // �����������
                           
                            bindModels.Add(new SetAddAgreementItem() { ItemId = shToItem.TOItem, AddAgreement = agreem.AddAgreement });
                            
                        }
                        else
                        {
                            hr.ErrorsList.Add(string.Format("������� �� ���������� � �� {0}", item.ItemId));
                        }
                    }
                }
                var deleteFilePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName(deleteImportName, attachment.Id, ".xls"));
                var createFilePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName(addImportName, attachment.Id, ".xls"));
                var bindFilePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName(setImportName, attachment.Id, ".xls"));
                
                // ������� ����� ������� �����
                var delwb = NpoiInteract.GetNewWorkBook();
                var addwb = NpoiInteract.GetNewWorkBook();
                var bindwb = NpoiInteract.GetNewWorkBook();
                // ��������� � ��� ������ � ������� � ��� ����� ���
                NpoiInteract.FillReportData(deleteModels.ToDataTable(), "sheet1", delwb);
                NpoiInteract.FillReportData(createModels.ToDataTable(), "sheet1", addwb);
                NpoiInteract.FillReportData(bindModels.ToDataTable(), "sheet1", bindwb);
                // ��������� ��� ��� �� ���� ����������
                NpoiInteract.SaveReport(deleteFilePath, delwb);
                NpoiInteract.SaveReport(createFilePath, addwb);
                NpoiInteract.SaveReport(bindFilePath, bindwb);

                hr.FilesPaths.AddRange(new List<string> { deleteFilePath, createFilePath, bindFilePath });
            }


            hr.Success = true;
            return hr;
        }


        private class DeleteAddAggrementModel 
        {
            public string AddAgreement { get; set; }
           
        }
        public class AddAddAgreementModel
        {
            public string AddAgreement { get; set; }

        }

        public class SetAddAgreementItem
        {
            public string ItemId { get; set; }
            public string AddAgreement { get; set; }
            
        }

    }
}
