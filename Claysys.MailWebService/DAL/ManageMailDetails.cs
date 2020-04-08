using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Claysys.MailWebService.DAL
{
    public class ManageMailDetails
    {
        public static bool IsUserInAD(string mailId)
        {
            string domain = ConfigurationManager.AppSettings["Domain"]; 

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain))
            {
                // define a "query-by-example" principal - 
                UserPrincipal qbeUser = new UserPrincipal(context);
                qbeUser.EmailAddress = mailId;

                // create your principal searcher passing in the QBE principal    
                PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

                // find all matches
                foreach (var found in srch.FindAll())
                {
                    if (((System.DirectoryServices.AccountManagement.UserPrincipal)found) != null && ((System.DirectoryServices.AccountManagement.UserPrincipal)found).EmailAddress == mailId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void CallFormAsAnAPI(string from, string to, string subject, string body)
        {
            #region Get from application settings
            string apiURL = ConfigurationManager.AppSettings["ApiURL"];//0//1
            string auth = ConfigurationManager.AppSettings["Auth"];//2//3
            string fromAddressControl = ConfigurationManager.AppSettings["FromAddressControl"];//4//5
            string toAddressControl = ConfigurationManager.AppSettings["ToAddressControl"];//6//7
            string mailBodyControl = ConfigurationManager.AppSettings["MailBodyControl"];//8//9
            string mailSubjectControl = ConfigurationManager.AppSettings["mailSubjectControl"];//8//9
            string triggerControl = ConfigurationManager.AppSettings["TriggerControl"];//10//11
            string formID = ConfigurationManager.AppSettings["FormID"];//12//13
            string tenantId = ConfigurationManager.AppSettings["TenantId"]; //14//15
            #endregion

            //WebRequest request = WebRequest.Create(apiURL);
            // request.Method = "POST";
            string PostData = @"
                 <APIRequest xmlns=""http://schemas.datacontract.org/2004/07/ClaySys.AppForms.Model.Models"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""> 
                  <APIDataList>
                    <APIData>
                      <ControlName>{0}</ControlName>
                      <Value>{1}</Value>
                    </APIData>
                     <APIData>
                      <ControlName>{2}</ControlName>
                      <Value>{3}</Value>
                    </APIData>
                     <APIData>
                      <ControlName>{4}</ControlName>
                      <Value>{5}</Value>
                    </APIData>
                    <APIData>
                      <ControlName>{9}</ControlName>
                      <Value>{10}</Value>
                    </APIData>
                       <APIData>
                      <ControlName>{6}</ControlName>
                      <Value></Value>
                    </APIData>
                  </APIDataList>
                  <ExecuteFormLoadRule>false</ExecuteFormLoadRule>
                  <FormId>{7}</FormId>
                  <FormName></FormName>
                  <TenantId>{8}</TenantId>
                </APIRequest>";

            PostData = string.Format(PostData, fromAddressControl, from, toAddressControl, to, mailBodyControl, body, triggerControl, formID, tenantId, mailSubjectControl, subject);

            //request.ContentType = "application/xml";
            //request.Headers["Origin"] = apiURL;
            ////request.Headers["Content-Type"] = "application/xml";

            string userName = auth.Split(':')[0];
            string passWord = auth.Split(':')[1];
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + passWord));
            //request.Headers.Add("Authorization", "Basic " + encoded);

            //WebResponse response = request.GetResponse();

            var client = new RestClient(apiURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Origin", apiURL);
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Authorization", "Basic " + encoded);
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Cookie", "ASP.NET_SessionId=ncm3izryh4l4mpnynxfetzu0");
            request.AddParameter("application/xml,application/xml", PostData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


        }

        public static void CreateCommand(string from, string to, string subject, string body, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand("InsertMailDetails", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@FromAddress";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = from;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@ToAddress";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = to;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@subject";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = subject;
                command.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@body";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = body;
                command.Parameters.Add(parameter);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                command.Dispose();
                connection.Close();
            }
        }
    }
}