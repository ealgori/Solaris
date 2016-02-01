using AutoImport.Rev3.DataContext;
using AutoImport.Rev3.ImportHandlers.Abstract;
using DbModels.DataContext;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{

    /// <summary>
    /// Просмотр оплаченых счетов. Выгрузка из сапа, обрабтка макросом и отправка на автоимпорт. Ответственный :Дмитрий Егоров
    /// </summary>
    public class InvoiceUpdateImportHandler : IAutoImportHandler
    {
        public HandlerResult Handle(global::AutoImport.Rev3.DomainModels.Attachment attachment)
        {
            HandlerResult hr = new HandlerResult();

            string savePath = Path.Combine(global::AutoImport.Rev3.Constants.HandledFilesFolder, DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
            // экземпляр юнирепорта
            var rows = EpplusSimpleUniReport.ReadFile(attachment.FilePath, global::AutoImport.Rev3.Constants.DefaultSheetName, 6);
            // считали объекты из эксель файла

            if (rows == null || rows.Count == 0)
            {
                hr.ErrorsList.Add("Ошибка работы с файлом. Проверьте его формат и содержимое.");
            }
            List<ImportReferenceModel> models = new List<ImportReferenceModel>();

            using (Context context = new Context())
            {
                var allShInvoices = context.ShInvoices.AsNoTracking().ToList();
                foreach (var inv in allShInvoices)
                {
                    if(!string.IsNullOrEmpty(inv.FacturaNumber))
                    {
                        inv.FacturaNumber = inv.FacturaNumber.TrimStart(new Char[] { '-' }).TrimStart(new Char[] { '0' });
                    }
                    if (!string.IsNullOrEmpty(inv.InvoiceNumber))
                    {
                        inv.InvoiceNumber = inv.InvoiceNumber.TrimStart(new Char[] { '-' }).TrimStart(new Char[] { '0' });
                    }
                }
               


                foreach (var row in rows)
                {
                    var referrence = row.Column4;
                    #region referrence
                    referrence = referrence.TrimStart(new Char[] { '-' });
                    referrence = referrence.TrimStart(new Char[] { '0' });
                    #endregion


                    if (string.IsNullOrEmpty(referrence))
                        continue;


                    double ammount;
                    #region ammount
                    object _ammount = 0;
                    string strAmmount = row.Column7;
                    strAmmount = new string(strAmmount.Where(s => s != '.').ToArray());
                    strAmmount = strAmmount.Replace(",", ".");

                    if (!CommonFunctions.StaticHelpers.GetObjectByStringValue(strAmmount, typeof(double), out  _ammount))
                    {

                        continue;
                    }
                    else
                    {

                        ammount = Math.Floor(Math.Abs((double)_ammount));
                    }
                    #endregion
                    string account = row.Column2;
                    #region account
                    account = account.TrimStart(new Char[] { '-' });
                    account = account.TrimStart(new Char[] { '0' });
                    #endregion

                    var shInvoices = allShInvoices.Where(to => to.InvoiceNumber == (referrence) ||

                      to.FacturaNumber==(referrence)).ToList();
                    
                    
                    shInvoices = shInvoices.Where(i => i.TotalAmount.HasValue)
                        .Where(i => ((int)Math.Floor(i.TotalAmount.Value)) == ammount).ToList();
                    if (shInvoices.Count() != 0)
                    {
                        foreach (var shInvoice in shInvoices)
                        {

                            // 12 ,13,15
                            object date = new object();
                            DateTime? pstngDate = new DateTime?();
                            DateTime? pmntDate = new DateTime?();
                            DateTime? clearing = new DateTime?();



                            if (CommonFunctions.StaticHelpers.GetObjectByStringValue(row.Column12, typeof(DateTime?), out  date, "dd-MM-yyyy"))
                            {
                                pstngDate = (DateTime?)date;
                            }
                            if (CommonFunctions.StaticHelpers.GetObjectByStringValue(row.Column13, typeof(DateTime?), out  date, "dd-MM-yyyy"))
                            {
                                pmntDate = (DateTime?)date;
                            }
                            if (CommonFunctions.StaticHelpers.GetObjectByStringValue(row.Column15, typeof(DateTime?), out  date, "dd-MM-yyyy"))
                            {
                                clearing = (DateTime?)date;
                            }
                            if (!string.IsNullOrEmpty(shInvoice.TOId))
                            {
                                var shTO = context.ShTOes.FirstOrDefault(t => t.TO == shInvoice.TOId);
                                if (shTO != null)
                                {
                                    var shSubcontractor = context.SubContractors.FirstOrDefault(s => s.ShName == shTO.Subcontractor);
                                    if (shSubcontractor != null)
                                    {
                                        if (shSubcontractor.SAPNumber == account)
                                        {

                                            if (shInvoice.PmntDate != pmntDate || shInvoice.PstngDate != pstngDate || shInvoice.Clearing != clearing)
                                            {
                                                models.Add(new ImportReferenceModel() { Invoice = shInvoice.InvoiceId.ToString(), Clearing = clearing, PmntDate = pmntDate, PstngDate = pstngDate, Source = shTO.TO });
                                                continue;
                                            }
                                        }
                                    }

                                }
                            }
                            if (!string.IsNullOrEmpty(shInvoice.AVRid))
                            {
                                var shAvr = context.ShAVRs.FirstOrDefault(a => a.AVRId == shInvoice.AVRid);
                                if (shAvr != null)
                                {
                                    var shSubcontractor = context.SubContractors.FirstOrDefault(s => s.ShName == shAvr.Subcontractor);
                                    if (shSubcontractor != null)
                                    {
                                        if (shSubcontractor.SAPNumber == account)
                                        {

                                            if (shInvoice.PmntDate != pmntDate || shInvoice.PstngDate != pstngDate || shInvoice.Clearing != clearing)
                                            {
                                                models.Add(new ImportReferenceModel() { Invoice = shInvoice.InvoiceId.ToString(), Clearing = clearing, PmntDate = pmntDate, PstngDate = pstngDate, Source = shAvr.AVRId });
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                            
                           
                           }
                    }
                }
            }
             var dataTable = models.ToDataTable();
            // создаем новую рабочую книгу
            var wb = NpoiInteract.GetNewWorkBook();
            // встваляем в нее данные и создаем в ней новый шит
            NpoiInteract.FillReportData(dataTable, "sheet1", wb);
            // сохраняем это все по пути назначения
            var fileSavePath = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("Invoice_PstngPmntClearing", attachment.Id, ".xls"));
            NpoiInteract.SaveReport(fileSavePath, wb);





            hr.Success = true;
            // возвращаем коллекцию файлов для обработки
            hr.FilesPaths.Add(fileSavePath);




            return hr;
        }

        public class ImportReferenceModel
        {
            public string Invoice { get; set; }
            public DateTime? PstngDate { get; set; }
            public DateTime? PmntDate { get; set; }
            public DateTime? Clearing { get; set; }
            public string Source { get; set; }
        }


    }
}
