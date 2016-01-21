using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.FileImportHandlers;
using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using SHInteract.Handlers.Solaris;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomFiHandlers
{
    public class TOIFIHandler : IFileImportHandler
    {
        public HandlerResult Handle(global::Models.AutoMail amail)
        {

            HandlerResult result = new HandlerResult();
            // для этого хэндлера в теме должно быть указано ТО, Сайт и Плановая дата, необязательная.
            var parts = amail.Subject.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() < 4)
            {
                result.Success = false;
                result.ErrorsList.Add("В теме письма должны быть указаны  номер ТО и сайт. А так же, по желанию, плановая дата.");
                return result;

            }
            using (Context context = new Context())
            {
                string _TO = parts[2];
                string _site = parts[3];
                string _planDate = string.Empty;
                if (parts.Count() > 4)
                {
                    _planDate = parts[4];
                }

                var shTO = context.ShTOes.Find(_TO);
                if (shTO == null)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("TO не существует:{0}", _TO));
                    return result;

                }


                var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == _site);
                if (shSite == null)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("Сайт не существует:{0}", _site));
                    return result;
                }
                DateTime? planDate = null;
                if (!string.IsNullOrEmpty(_planDate))
                {
                    DateTime pdate;
                    if (DateTime.TryParseExact(_planDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out pdate))
                    {
                        planDate = pdate;
                    }
                    else
                    {

                        if (DateTime.TryParseExact(_planDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out pdate))
                        {
                            planDate = pdate;
                        }
                        else
                        {

                            result.Success = false;
                            result.ErrorsList.Add(string.Format("Плановая дата задана некорректно:{0}", _planDate));
                            return result;
                        }
                    }
                }

                var shContacts = context.ShContacts.Where(c => c.EMailAddress.Contains(amail.Email));
                if (shContacts.Count()==0)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("Ваш Email адрес не зарегестрирова в списке контактов. Пожалуйста обратитесь к вашему администратору в Ericsson", amail.Email));
                    return result;
                }
                var contact = shContacts.FirstOrDefault(c => c.Contact == shTO.Subcontractor);
                if (contact==null)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("У Вас нет прав на занесение информации по TO:{0}. Пожалуйста, обратитесь к вашему администратору Ericsson, для получения доступа.", shTO.TO));
                    return result;
                }

                TORepository toRep = new TORepository(context);
                ShTOItem item = null;
                var toItemModels = toRep.GetTOItemModels(shTO.TO,true).Where(i => i.Site == shSite.Site);
                if (toItemModels.Count() == 0)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("У TO:{0} нет позиций по сайту:{1}", _TO, shSite.Site));
                    return result;
                }
                if (toItemModels.Count() > 1)
                {
                    if (!planDate.HasValue)
                    {
                        result.Success = false;
                        result.ErrorsList.Add(string.Format(@"Площадка ""{1}"" включена в состав TO:""{0}"" несколько раз, пожалуйста, укажите плановую дату выполнения работ на площадке.", _TO, shSite.Site));
                        return result;
                    }
                    else
                    {
                        toItemModels = toItemModels.Where(t => t.TOPlanDate == planDate);
                        if (toItemModels.Count() == 0)
                        {
                            result.Success = false;
                            result.ErrorsList.Add(string.Format("У TO:{0} нет  позиций по сайту:{1} c плановой датой:{2}, пожалуйста, обратитесь к Администратору в Е/// за корректировкой дат в системе.", _TO, shSite.Site, _planDate));
                            return result;
                        }
                        else
                        {
                            if (toItemModels.Count() > 1)
                            {
                                result.Success = false;
                                result.ErrorsList.Add(string.Format("У TO:{0} больше одной  позиции по сайту:{1} c плановой датой:{2}, пожалуйста, обратитесь к Администратору в Е/// за корректировкой дат в системе.", _TO, shSite.Site, _planDate));
                                return result;
                            }
                        }
                    }
                }

                item = context.ShTOItems.Find(toItemModels.FirstOrDefault().TOItem);

                if (item.WorkConfirmedByEricsson)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("Позиция по ТО:{0} площадки:{1} уже была принята ранее ({2} {3} {4}) . ", _TO, shSite.Site, item.WorkConfirmedByEricssonBy, item.WorkConfirmedByEricssonDate.HasValue?item.WorkConfirmedByEricssonDate.Value.ToString("dd.MM.yyyy"):"", item.TOItem, _planDate));
                    return result;
                }

                if (amail.Attachments.Count > 4)
                {
                    result.Success = false;
                    result.ErrorsList.Add(string.Format("Возможен импорт только 4 файлов."));
                    return result;
                }
                result.InfoList.Add(string.Format("TO:{0}; Site:{1}; PlanDate:{2}",shTO.TO, shSite.Site, planDate.HasValue?planDate.Value.ToString("dd.MM.yyyy"):"not specified"));
                string impResult = FileUploadSol.Handle(amail.Attachments.Select(f=>f.FilePath).ToList(), item.TOItem);
                result.InfoList.Add(impResult);
                result.Success = true;


            }
            return result;
           
        }
    }
}
