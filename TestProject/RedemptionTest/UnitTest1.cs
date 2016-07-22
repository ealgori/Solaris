using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Instrumentation;
using Redemption;
using System.IO;
using RedemptionInteract;
using System.Web;
using System.Net;

namespace TestProject.RedemptionTest
{
    [TestClass]
    public class UnitTest1
    {

        public static string ealgoriEmail = "aleksey.gorin@ericsson.com";
        public static string ealgoriEmail2 = "a762b1b5-3271-4b81-8aef-9cfabf31cec4@ericsson.com";
        public static string ealgori = "ealgori";
        public static string ealgori2 = @"ericsson\ealgori";
        public static string userName = "Aleksey Gorin";

        public static string oldAutoDiscover = @"C:\Users\ealgori\AppData\Local\Microsoft\Outlook\b7e128fc6da9614daadcced8787ff412 - Autodiscover.xml";
        public static string newAutoDiscover = @"C:\Users\ealgori\AppData\Local\Microsoft\Outlook\9425a46cbd7ac747afb11f26f1854908 - Autodiscover.xml";

        public static  RDOSession GetSession()
        {
          
            var rSession = new RDOSession();
           // rSession.CacheAutodiscoverXml("aleksey.gorin@ericsson.com", File.ReadAllText(oldAutoDiscover));
            rSession.SkipAutodiscoverLookupInAD = true;
            return rSession;
        }

        public static void TryLogin(string userName, string server)
        {
            WebRequest request = WebRequest.Create("https://www.microsoft.com/sv-se/");
            var res = request.GetResponse();
            var text = res.ToString();
            var session = GetSession();

            // session.LogonHostedExchangeMailbox(ealgoriEmail, userName, "G0rin2003");
            //session.LogonHostedExchangeMailbox(ealgoriEmail2, ealgoriEmail, "G0rin2003");
            session.Logon();
            try
            {
                var folder = session.GetDefaultFolder(rdoDefaultFolders.olFolderInbox);
                var rStore = session.GetSharedMailbox("vimpelcom.admin.02@ericsson.com");
            }
            catch (Exception exc)
            {

                throw;
            }
        }


     
        //[TestMethod]
        //public void TestMethod7()
        //{
        //    TryLogin(ealgoriEmail, MailOptions.ExchangeServerName);
        //}
        //   [TestMethod]
        //public void TestMethod1()
        //{
        //    TryLogin("Aleksey Gorin", MailOptions.OldExchangeServerName);

        //}
        //[TestMethod]
        //public void TestMethod2()
        //{
        //    TryLogin(ealgoriEmail, MailOptions.OldExchangeServerName);
        //}
        ////[TestMethod]
        ////public void TestMethod3()
        ////{
        ////    TryLogin(ealgoriEmail2, MailOptions.OldExchangeServerName);
        ////}
        //[TestMethod]
        //public void TestMethod4()
        //{
        //    TryLogin(ealgori, MailOptions.OldExchangeServerName);
        //}
        ////[TestMethod]
        ////public void TestMethod5()
        ////{
        ////    TryLogin(ealgori2, MailOptions.OldExchangeServerName);
        ////}
        ////[TestMethod]
        ////public void TestMethod6()
        ////{
        ////    TryLogin("Aleksey Gorin", MailOptions.ExchangeServerName);
        ////}
        ////[TestMethod]
        ////public void TestMethod8()
        ////{
        //    TryLogin(ealgoriEmail2, MailOptions.ExchangeServerName);
        //}
        //[TestMethod]
        //public void TestMethod9()
        //{
        //    TryLogin(ealgori, MailOptions.ExchangeServerName);
        //}
        //[TestMethod]
        //public void TestMethod10()
        //{
        //    TryLogin(ealgori2, MailOptions.ExchangeServerName);
        //}
        //[TestMethod]
        //public void TestMethod11()
        //{
        //    TryLogin("Aleksey Gorin", MailOptions.ExchangeServerName);
        //}
       

    }
}
