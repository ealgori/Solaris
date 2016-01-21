//using Redemption;

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TaskManager.Handlers.EmailHandlers.Abstract;
//using TaskManager.TaskParamModels;

//namespace TaskManager.Handlers.EmailHandlers.Models
//{
//    public class EmailHandlerRev2 : AEmailHandler
//    {
//        public EmailHandlerRev2(TaskParameters taskParameters)
//            : base(taskParameters)
//        {

//        }
//        public override bool SendMails()
//        {

//            var ri = new RedemptionInreract.Rev2.RI();
//            var mb = ri.GetMailBoxFor("technical.box.for.solaris@ericsson.com");
//           // RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");
//            if (TaskParameters.EmailHandlerParams != null && TaskParameters.EmailHandlerParams.EmailParams.Count > 0)
//            {

//                foreach (var param in TaskParameters.EmailHandlerParams.EmailParams)
//                {
//                    try
//                    {
//                        #region Код для RDOMail
//                        //RDOMail mail = new RDOMail
//                        //{
//                        //    Subject = param.Subject,
//                        //};
//                        //if (param.Recipients == null || param.Recipients.Count() == 0)
//                        //{
//                        //    throw new Exception("Не указаны получатели письма:" + param.Name);
//                        //}
//                        //var recipients = param.Recipients.Where(r => IsValidEmail(r));
//                        //if (recipients.Count() == 0)
//                        //{
//                        //    throw new Exception("Ни одного корретного получателя письма:" + param.Name);
//                        //}
//                        //foreach (var recip in param.Recipients)
//                        //{
//                        //    mail.Recipients.Add(recip);

//                        //}
//                        //if ((param.CCRecipients!=null&& param.CCRecipients.Count>0))
//                        //{
//                        //    foreach (var ccrecip in param.CCRecipients)
//                        //    {
//                        //        if (IsValidEmail(ccrecip))
//                        //        {
//                        //            var cc = mail.Recipients.Add(ccrecip);
//                        //            cc.Type = 2;
//                        //        }

//                        //    }
//                        //}
//                        //if ((param.BCCRecipients!=null)&& (param.BCCRecipients.Count>0))
//                        //{
//                        //    foreach (var bccrecip in param.BCCRecipients)
//                        //    {
//                        //        if (IsValidEmail(bccrecip))
//                        //        {
//                        //            var bcc = mail.Recipients.Add(bccrecip);
//                        //            bcc.Type = 3;
//                        //        }

//                        //    }
//                        //}
//                        //mail.HTMLBody = param.HtmlBody;


                       


//                        //var attachments = GetAttachments(param);

//                        //if (attachments == null || attachments.Count == 0)
//                        //{
//                        //    if (!param.AllowWithoutAttachments)
//                        //    {
//                        //        throw new Exception("Нет ни одного файла для отправки " + param.Name);
//                        //    }
//                        //}
//                        //else
//                        //{
//                        //    foreach (var attach in param.FilePaths)
//                        //    {
//                        //        if (File.Exists(attach))
//                        //        {
//                        //            mail.Attachments.Add(attach, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, 1, Path.GetFileName(attach));
//                        //        }
//                        //    }
//                        //}
//                        //mail.Body = param.HtmlBody;
//                        ////Отправляем
//                        //mb.SendMail(mail);
//                        #endregion
                       
//                            AutoMail mail = new AutoMail
//                            {
//                                Subject = param.Subject,
//                            };
//                            if (param.Recipients == null || param.Recipients.Count() == 0)
//                            {
//                                throw new Exception("Не указаны получатели письма:" + param.Name);
//                            }
//                            var recipients = param.Recipients.Where(r => IsValidEmail(r));
//                            if (recipients.Count() == 0)
//                            {
//                                throw new Exception("Ни одного корретного получателя письма:" + param.Name);
//                            }
//                            mail.Email = string.Join(";", recipients.Distinct());
//                            if (param.CCRecipients != null && param.CCRecipients.Count > 0)
//                            {
//                                mail.CCEmail = string.Join(";", param.CCRecipients.Where(r => IsValidEmail(r)));
//                            }
//                            if (param.BCCRecipients != null && param.BCCRecipients.Count > 0)
//                            {
//                                mail.BCCEmail = string.Join(";", param.BCCRecipients.Where(r => IsValidEmail(r)));
//                            }


//                            var attachments = GetAttachments(param);

//                            if (attachments == null || attachments.Count == 0)
//                            {
//                                if (!param.AllowWithoutAttachments)
//                                {
//                                    throw new Exception("Нет ни одного файла для отправки " + param.Name);
//                                }
//                            }
//                            else
//                            {
//                                foreach (var attach in attachments)
//                                {
//                                    mail.Attachments.Add(attach);
//                                }
                                
//                            }
//                            mail.Body = param.HtmlBody;
//                            //Отправляем
//                            mb.SendMail(mail);


                       

//                    }
//                    catch (Exception exc)
//                    {
//                        TaskParameters.TaskLogger.LogError(param.Name + " " + exc.Message);

//                    }
//                }

//            }
//            return true;
//        }

//        bool IsValidEmail(string email)
//        {
//            try
//            {
//                var addr = new System.Net.Mail.MailAddress(email);
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
//}
