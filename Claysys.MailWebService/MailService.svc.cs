using System;
using System.Configuration;
namespace Claysys.MailWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class MailService : IMailService
    {
        public bool InsertMailBody(string from, string to, string subject, string body)
        {
            try
            {
                if (Claysys.MailWebService.DAL.ManageMailDetails.IsUserInAD(from))
                {
                    Claysys.MailWebService.DAL.ManageMailDetails.CallFormAsAnAPI(from, to, subject, body);
                }
                //CreateCommand(from, to, subject, body, CoreConnectionStringManager.CRMConnectionString);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string GetTargetMailId()
        {
            string targetApplicationSetting = ConfigurationManager.AppSettings["TargetMailID"];
            if (!string.IsNullOrEmpty(targetApplicationSetting))
            {
                return targetApplicationSetting;
            }
            else
            {
                return string.Empty;
            }
        }




    }
}



