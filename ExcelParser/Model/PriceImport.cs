using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelParser.Abstract;
using System.IO;

using DbModels;

using NPOI.SS.UserModel;
using System.Web;
using System.Globalization;
using DbModels.Models;
using CommonFunctions;
using DbModels.DomainModels.Solaris;


namespace ExcelParser.Model
{
    public class PriceImport : APriceImport
    {
        private List<string> SubContractorNameField =new List<string> {"Подрядчик:"};
        private List<string> SubContractorAdressField =new List<string> { "Юридический адрес подрядчика:"};
        private List<string> PriceListNumberField =new List<string> { "Номер Договора:"};
        private List<string> PriceListAdditionalNumberField=new List<string> {  "Номер Дополнительного соглашения:"};
        private List<string> PriceListSignDateField =new List<string> { "Дата подписания:"};
        private List<string> PriceListExpiryDateField = new List<string> {"Дата окончания действия Договора/Дополнительного соглашения:"};
        private List<string> SubContractorSAPNumberField =new List<string> { "SAP Vendor ID:"};
        private List<string> SubContractorSAPNameField =new List<string> { "SAP Vendor name:"};
        private List<string> PriceListVATField = new List<string> {"VAT"};
        //private string PriceListPaymentTermsField1 = "Условия платежа";
        private List<string> PriceListPaymentTermsField =new List<string> { "Условия оплаты","Условия платежа"};
        private List<string> RevisionItemNameField =new List<string> { "Перечень работ, услуг /Items (works and services)"};
        private List<string> RevisionItemUnitField = new List<string> {"Ед. изм. / Unit"};
        private List<string> RevisionItemPriceField =new List<string> { "Стоимость, руб. (без НДС) / Price in RUR (without VAT)"};
        private List<string> RevisionItemSiteField = new List<string> {"Номер объекта / Site ID"};
        private List<string> RevisionItemAddressField = new List<string> {"Адрес объекта / Address"};



        public PriceImport(HttpPostedFileBase fileBase,int projectId, ImportLogger.ImportLogger importLogger, bool comparable)
            : base(fileBase, importLogger,  projectId, comparable)
        {
            if (!MapHeaders(SubContractorNameField, SubContractorAdressField, PriceListNumberField, PriceListAdditionalNumberField, PriceListSignDateField,
                PriceListExpiryDateField, SubContractorSAPNumberField, SubContractorSAPNameField, PriceListVATField, PriceListPaymentTermsField,
                RevisionItemNameField, RevisionItemUnitField, RevisionItemPriceField, RevisionItemSiteField, RevisionItemAddressField))
            {
                MappedHeaders = null;
            }
        }

        public override SubContractor ProcessSubContractor()
        {

            var sapVendorIdCell = GetRightCell(MappedHeaders[SubContractorSAPNumberField]);
            // использую метод получния объекта а не стринг значения, потому что в эксель файле это значение с запятой, а нам нужно без.
            var sapNumberValue = NpoiInteract.GetCellObjectValueExt(sapVendorIdCell, typeof(decimal));
            if (sapNumberValue == null)
            {
                AddError("Не указан сабконтрактор сап ид");
                return null;
            }
            var sapNumber = sapNumberValue.ToString();
            if (string.IsNullOrEmpty(sapNumber))
            {
                AddError("Пустой вендор подрядчика.",sapVendorIdCell);
                return null;
            }
            SubContractor subContractor = Context.SubContractors.FirstOrDefault(sc => sc.SAPNumber == sapNumber);
            if (subContractor == null)
            {
                subContractor = new SubContractor();
                subContractor.SAPNumber = sapNumber;

                var subContractorNameCell = GetRightCell(MappedHeaders[SubContractorNameField]);
                subContractor.Name = NpoiInteract.GetCellValueExt(subContractorNameCell, typeof(string));

                var subContractorSAPNameCell = GetRightCell(MappedHeaders[SubContractorSAPNameField]);
                subContractor.SAPName = NpoiInteract.GetCellValueExt(subContractorSAPNameCell, typeof(string));

                var subContractorAdressCell = GetRightCell(MappedHeaders[SubContractorAdressField]);
                subContractor.Address = NpoiInteract.GetCellValueExt(subContractorAdressCell, typeof(string));
                //  = StaticHelpers.GetCellValueExt(subContractorSAPNameCell, typeof(string));
                subContractor.Project = Project;
                Context.SubContractors.Add(subContractor);
                AddMessage("Добавлен сабконтрактор: "+ subContractor.ToString());





            }
            return subContractor;
        }

        public override PriceList ProcessPriceList(SubContractor subContractor)
        {
            
            var priceListNumberCell = GetRightCell(MappedHeaders[PriceListNumberField]);
            string priceListNumber = NpoiInteract.GetCellValueExt(priceListNumberCell, typeof(string));
            if (string.IsNullOrEmpty(priceListNumber))
            {
                AddError("Для прайс листа не указан номер договора");
                return null;
            }
            var priceListAdditionalNumberCell = GetRightCell(MappedHeaders[PriceListAdditionalNumberField]);
            string priceListAdditionalNumber = NpoiInteract.GetCellValueExt(priceListAdditionalNumberCell, typeof(string));
            
            //var pl2 = Context.PriceLists.FirstOrDefault(pl => pl.PriceListNumber == priceListNumber && pl.PriceListAdditionalNumber == priceListAdditionalNumber);
            //var pl3 = Context.PriceLists.FirstOrDefault(pl => pl.PriceListNumber == priceListNumber && pl.SubContractor.Id == subContractor.Id);
            // 100% должен быть такой некомпарабл. Если он есть, уже обрабатываем логику
            var priceLists = Context.PriceLists.Where(pl => pl.PriceListNumber == priceListNumber && pl.SubContractor.Id == subContractor.Id);
            
            priceLists = priceLists.Where(pl => pl.PriceListAdditionalNumber == priceListAdditionalNumber);
           
           // var priceList = priceLists.FirstOrDefault();
            var parentPL = priceLists.FirstOrDefault(p => p.Comparable == false);
            if( Comparable&& parentPL==null)
            {
                AddError("Не существует родительского прайс-листа для Comparable прайслиста. Прогрузите сначала его.");
                return null;
            }
            var priceList = priceLists.FirstOrDefault(p=>p.Comparable==Comparable);
            if (priceList == null)
            {
                // если родителя нету, и мы грузим компарабл, то ошибка
               
                
                priceList = new PriceList();
                priceList.PriceListNumber = priceListNumber;
                priceList.PriceListAdditionalNumber = priceListAdditionalNumber;
                priceList.Project = Project;
                priceList.SubContractor = subContractor;
                priceList.Comparable = Comparable;

                if(Comparable)
                {
                    PriceListMap map = new PriceListMap();
                    map.PriceList = parentPL;
                    map.ComparablePriceList = priceList;
                    Context.PriceListMaps.Add(map);
                    AddWarning("Связь между родительским и Comparable прайслистом добавлена");
                }
                #region oldcode
                
                //var priceListPaymentTermsCell = GetRightCell(MappedHeaders[PriceListPaymentTermsField]);
                //priceList.PaymentTerms = StaticHelpers.GetCellValueExt(priceListPaymentTermsCell, typeof(string));

                //var priceListSignDateCell = GetRightCell(MappedHeaders[PriceListSignDateField]);
                //if (priceListSignDateCell != null)
                //{
                //    DateTime date;
                //    try
                //    {

                //        date = (DateTime)StaticHelpers.GetCellObjectValueExt(priceListSignDateCell, typeof(DateTime));
                //        priceList.SignDate = date;
                //    }
                //    catch (System.Exception ex)
                //    {


                //        string dateString = (StaticHelpers.GetCellValueExt(priceListSignDateCell, typeof(string)));
                       
                //        if (DateTime.TryParse(dateString, new CultureInfo("ru-RU"), DateTimeStyles.None, out date))
                //        {
                //            priceList.SignDate = date;
                //        }
                //        else
                //        {
                //            AddError("Не удалось распознать  дату начала действия прайс листа.");
                //            return null;
                //        }
                //    }
                    
                //}
                   
                //else
                //{
                //    AddError("Не указана дата начала действия прайс листа.");
                //    return null;
                //}

                //var priceListExpiryDateCell = GetRightCell(MappedHeaders[PriceListExpiryDateField]);
                //if (priceListExpiryDateCell != null)
                //{

                //    DateTime expDate;

                //    try
                //    {

                //        expDate = (DateTime)StaticHelpers.GetCellObjectValueExt(priceListExpiryDateCell, typeof(DateTime));
                //        priceList.ExpiryDate = expDate;
                //    }
                //    catch
                //    {
                //        string expDateString = (StaticHelpers.GetCellValueExt(priceListExpiryDateCell, typeof(string)));

                //        if (DateTime.TryParse(expDateString, new CultureInfo("ru-RU"), DateTimeStyles.None, out expDate))
                //        {
                //            priceList.ExpiryDate = expDate;
                //        }
                //        else
                //        {
                //            AddWarning("Не удалось распознать дату окончания действия прайс листа.");
                //        }
                //    }

                //    string dateStringExpiry = (StaticHelpers.GetCellValueExt(priceListExpiryDateCell, typeof(string)));
                //    DateTime dateExpry;
                //    if (DateTime.TryParse(dateStringExpiry, new CultureInfo("ru-RU"), DateTimeStyles.None, out dateExpry))
                //    {
                //        priceList.ExpiryDate = dateExpry;
                //    }
                //}
               

                //var priceListVATCell = GetRightCell(MappedHeaders[PriceListVATField]);
                //priceList.VAT = StaticHelpers.GetCellValueExt(priceListVATCell, typeof(string));
#endregion
                Context.PriceLists.Add(priceList);

                AddWarning("Добавлен новый прайслист:" + string.Format("{0}{1}:{2}", priceList.PriceListNumber, priceList.PriceListAdditionalNumber==null?"":"-"+priceList.PriceListAdditionalNumber, Comparable?"Comparable":""));

            }
            // Теперь может. Теперь это два разных прайслиста будет
            //if (priceList.Comparable != Comparable)
            //{
            //    AddError("Праслист для сравнения не может использоваться вместе с реальным прайслистом.");
            //    return null;
            //}

            return priceList;

        }

        public override PriceListRevision ProcessPriceListRevision(PriceList priceList)
        {
            PriceListRevision revision = new PriceListRevision();
            revision.PriceList = priceList;
            Context.PriceListRevisions.Add(revision);
            var priceListPaymentTermsCell = GetRightCell(MappedHeaders[PriceListPaymentTermsField]);
            revision.PaymentTerms = NpoiInteract.GetCellValueExt(priceListPaymentTermsCell, typeof(string));

            var priceListSignDateCell = GetRightCell(MappedHeaders[PriceListSignDateField]);
            if (priceListSignDateCell != null)
            {
                DateTime date;
                try
                {

                    date = (DateTime)NpoiInteract.GetCellObjectValueExt(priceListSignDateCell, typeof(DateTime));
                    revision.SignDate = date;
                }
                catch (System.Exception ex)
                {


                    string dateString = (NpoiInteract.GetCellValueExt(priceListSignDateCell, typeof(string)));

                    if (DateTime.TryParse(dateString, new CultureInfo("ru-RU"), DateTimeStyles.None, out date))
                    {
                        revision.SignDate = date;
                    }
                    else
                    {
                        AddError("Не удалось распознать  дату начала действия прайс листа.");
                        return null;
                    }
                }

            }

            else
            {
                AddError("Не указана дата начала действия прайс листа.");
                return null;
            }

            var priceListExpiryDateCell = GetRightCell(MappedHeaders[PriceListExpiryDateField]);
            if (priceListExpiryDateCell != null)
            {

                DateTime expDate;

                try
                {

                    expDate = (DateTime)NpoiInteract.GetCellObjectValueExt(priceListExpiryDateCell, typeof(DateTime));
                    revision.ExpiryDate = expDate;
                }
                catch
                {
                    string expDateString = (NpoiInteract.GetCellValueExt(priceListExpiryDateCell, typeof(string)));

                    if (DateTime.TryParse(expDateString, new CultureInfo("ru-RU"), DateTimeStyles.None, out expDate))
                    {
                        revision.ExpiryDate = expDate;
                    }
                    else
                    {
                        AddWarning("Не удалось распознать дату окончания действия прайс листа.");
                    }
                }

                string dateStringExpiry = (NpoiInteract.GetCellValueExt(priceListExpiryDateCell, typeof(string)));
                DateTime dateExpry;
                if (DateTime.TryParse(dateStringExpiry, new CultureInfo("ru-RU"), DateTimeStyles.None, out dateExpry))
                {
                    revision.ExpiryDate = dateExpry;
                }
            }


            var priceListVATCell = GetRightCell(MappedHeaders[PriceListVATField]);
            revision.VAT = NpoiInteract.GetCellValueExt(priceListVATCell, typeof(string));





            AddMessage(string.Format("Создана новая ревизия {0} прайслиста",Comparable?"Comparable":""));
            int importFileId = ImportLogger.GetCurrentImportFileId();
            var importFile = Context.ImportFiles.FirstOrDefault(ifi => ifi.Id == importFileId);
            revision.ImportFile = importFile;

            return revision;
        }

        public override bool ProcessPriceListRevisionItems(PriceListRevision priceListRevision, SubContractor subContractor)
        {
            List<SAPCode> _cachedSapCodes = new List<SAPCode>();
            bool success = true;
            var startRow = MappedHeaders[RevisionItemNameField].RowIndex;
            var sheet = MappedHeaders[RevisionItemNameField].Sheet;
            string currency = GetCurrency(NpoiInteract.GetCellValueExt(MappedHeaders[RevisionItemPriceField], typeof(string)));
            int codeAutoincrement = -1;
            int processedCount = 0;
            int createdSAPCodeCount = 0;
            int foundedSAPCodeCount = 0;
            if (sheet != null)
            {


                // Оббегаем все строки в них
                for (int rowNum = startRow + 1; rowNum < sheet.PhysicalNumberOfRows; rowNum++)
                {

                    IRow row = sheet.GetRow(rowNum);
                    if (row != null)
                    {
                        try
                        {

                            // получаем нужные ячейки из текущей строки

                            var itemNameCell = row.GetCell(MappedHeaders[RevisionItemNameField].ColumnIndex);
                            if (itemNameCell != null && !string.IsNullOrEmpty(NpoiInteract.GetCellValueExt(itemNameCell, typeof(string))))
                            {
                                var itemUnitCell = row.GetCell(MappedHeaders[RevisionItemUnitField].ColumnIndex);
                                var itemPriceCell = row.GetCell(MappedHeaders[RevisionItemPriceField].ColumnIndex);
                                var itemSiteCell = row.GetCell(MappedHeaders[RevisionItemSiteField].ColumnIndex);
                                var itemAddressCell = row.GetCell(MappedHeaders[RevisionItemSiteField].ColumnIndex);
                                // новый прайс айтем
                                PriceListRevisionItem item = new PriceListRevisionItem();
                                item.PriceListRevision = priceListRevision;
                                item.Name = NpoiInteract.GetCellValueExt(itemNameCell, typeof(string));
                                item.Unit = NpoiInteract.GetCellValueExt(itemUnitCell, typeof(string));
                                try
                                {
                                    // если цену не удалось распознать, то сворачиваемся
                                    item.Price = (decimal)NpoiInteract.GetCellObjectValueExt(itemPriceCell, typeof(decimal));
                                }
                                catch (System.Exception ex)
                                {
                                    AddError(string.Format("Не удалось распознать цену ({0}) для позиции в строке: {1}", NpoiInteract.GetCellObjectValueExt(itemPriceCell, typeof(string)), row.RowNum + 1));
                                    success = false;
                                }

                                item.Site = NpoiInteract.GetCellValueExt(itemSiteCell, typeof(string));
                                item.Address = NpoiInteract.GetCellValueExt(itemAddressCell, typeof(string));
                                // ищем подходящий сап код из присутствующих. вендор это сапи ид сабконтактора. он и есть. в дальнейшем поле вендор будет удалено из таблицы
                                var sapCode = Context.SAPCodes.FirstOrDefault(sc => sc.Vendor == subContractor.SAPNumber && sc.Description == item.Name);
                                if (sapCode != null)
                                {
                                    item.SAPCode = sapCode;
                                    //sapCode.ExistedInSAP = true;
                                    foundedSAPCodeCount++;
                                }
                                else
                                {
                                    if (_cachedSapCodes.FirstOrDefault(sc => sc.Description == item.Name)!=null)
                                    {
                                        AddError(string.Format("Дубль дискрипшн для айтема ({0}) для позиции в строке: {1}", item.Name, row.RowNum + 1));
                                        success = false;
                                    }
                                    else
                                    {
                                        // если такого сап кода нету, то создаем новый
                                        sapCode = new SAPCode();
                                        sapCode.Description = item.Name;
                                        sapCode.Vendor = subContractor.SAPNumber;
                                        if (codeAutoincrement < 0)
                                        {
                                            var lastSapCode = Context.SAPCodes.OrderByDescending(sc => sc.Code).FirstOrDefault();
                                            if (lastSapCode != null)
                                            {
                                                string lastCode = lastSapCode.Code;
                                                var parts = lastCode.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                                codeAutoincrement = Convert.ToInt32(parts.Last());
                                            }

                                        }
                                        codeAutoincrement++;
                                        sapCode.Code = GetCode(codeAutoincrement);
                                        createdSAPCodeCount++;
                                        AddWarning(string.Format("Создан сапкод для позиции:'{0}' в строке '{1}'; SapCode:{2}", item.Name, rowNum, sapCode.Code));
                                        Context.SAPCodes.Add(sapCode);
                                        _cachedSapCodes.Add(sapCode);
                                    }
                                }
                                
                                
                                item.SAPCode = sapCode;
                                Context.PriceListRevisionItems.Add(item);
                                processedCount++;
                            }
                          

                        }
                        catch (Exception exc)
                        {
                            AddError("Ошибка при чтении строки " + row + " " + exc.Message);
                            success = false;
                        }

                    }

                }
                if (createdSAPCodeCount > 0)
                {
                    AddWarning(string.Format("Создано сапкодов:{0}; найдено сапкодов:{1}", createdSAPCodeCount, foundedSAPCodeCount));
                }
                else
                {
                    AddMessage(string.Format("Создано сапкодов:{0}; найдено сапкодов:{1}", createdSAPCodeCount, foundedSAPCodeCount));
                }
               
                AddMessage("Обработано " + processedCount + " айтемов");
            }



            return success;

        }

        public bool Process(string userName)
        {
            List<string> allowedUsers = new List<string>() { @"ERICSSON\enikose", @"EEMEA\echeale", @"ERICSSON\erenkha", @"ERICSSON\enikvor", @"ERICSSON\ealgori", @"ERICSSON\echeale", @"ERICSSON\eekakos" };
            if (allowedUsers.Contains(userName))
            {
                if (WorkBook != null)
                {
                    if (MappedHeaders != null)
                    {
                        AddMessage("Обработка файла:" + FileName);
                        

                        var subcontractor = ProcessSubContractor();
                        if (subcontractor == null)
                            return false;
                    
                        var priceList = ProcessPriceList(subcontractor);
                        if (priceList == null)
                        {
                            return false;
                        }
                        var revision = ProcessPriceListRevision(priceList);
                        if (revision == null)
                            return false;
                        if (ProcessPriceListRevisionItems(revision, subcontractor))
                        {

                            ImportLogger.SetSuccessImportFile();
                            Context.SaveChanges();
                            AddMessage("ПРОГРУЖЕН");
                        }
                        else
                        {
                            AddWarning("НЕ ПРОГРУЖЕН");
                        }
                    }
                    else
                    {
                        AddWarning("НЕ ПРОГРУЖЕН");
                    }
                }
               

            }
            else
            {
                AddError("У вас не прав на прогрузку прайс листов");
                AddWarning("НЕ ПРОГРУЖЕН");
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringWithCurrency"></param>
        /// <returns></returns>
        private string GetCurrency(string stringWithCurrency)
        {
            if (stringWithCurrency.IndexOf("USD", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return "USD";
            }
            if (stringWithCurrency.IndexOf("ZCU", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return "ZCU";
            }
            if (stringWithCurrency.IndexOf("RUR", StringComparison.CurrentCultureIgnoreCase) < -1 && stringWithCurrency.IndexOf("RUB", StringComparison.CurrentCultureIgnoreCase) < -1)
            {
                AddError("Не удалось определить Currency. По умолчанию использован RUR");
            }
            return "RUR";

        }
        /// <summary>
        /// возвращает код в нужном формате.
        /// </summary>
        /// <param name="codeNum"></param>
        /// <returns></returns>
        private string GetCode(int codeNum)
        {
            return string.Format("ECR-SOLA-SER-{0}", (codeNum).ToString("00000"));
        }
    }
}
