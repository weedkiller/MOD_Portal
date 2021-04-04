//using ACQ.Web.ViewModel.IndexPage;
using ACQ.Web.ViewModel.AONW;
using ACQ.Web.ViewModel.User;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ACQ.Web.ExternalServices.Email
{
    public class EmailHelper
    {
        private static void SendEmail(string Email, string Body)
        {
            try
            {
               
                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();
                MailAddress mailAddress = new MailAddress(ConfigurationManager.AppSettings["FromAddress"].ToString(), ConfigurationManager.AppSettings["FromName"].ToString());
                smtpClient.Host = ConfigurationManager.AppSettings["smtpAddress"].ToString();
                message.From = mailAddress;
                message.To.Add(Email);
                message.Priority = MailPriority.High;
                message.Subject = "MoD (ACQUISITION) DASHBOARD";
                message.Body = Body.ToString();
                message.IsBodyHtml = true;
                smtpClient.EnableSsl = false;
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
                smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential(ConfigurationManager.AppSettings["MailUserID"].ToString(), ConfigurationManager.AppSettings["MailPassword"].ToString());
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                string msg = ex.StackTrace;
            }
        }


        //change password 
        public static ChangePasswordViewModel SendPwdDetails(ChangePasswordViewModel InputModel, string emaid, string mailPath)
        {
            ChangePasswordViewModel usOutputViewModel = new ChangePasswordViewModel();
            try
            {
                string Body = EmailHelper.SendOTpPopulateBody(InputModel, emaid, mailPath);
                EmailHelper.SendEmail(emaid, Body);
            }
            catch (Exception)
            {
            }
            return usOutputViewModel;
        }
        private static string SendOTpPopulateBody(ChangePasswordViewModel InputModel, string emaid, string mailPath)
        {
            return mailPath.Replace("{Name}", InputModel.UserName)
           .Replace("{Email}", InputModel.UserName).Replace("{tokenid}", InputModel.TokenId);
        }
        public static string SendChangePasswordMail(string UserMail, string newpassord, string mailPath)
        {
            ForgetPasswordViewModel usOutputViewModel = new ForgetPasswordViewModel();
            try
            {
                string Body = EmailHelper.SendChangePasswordOTpPopulateBody(newpassord, mailPath);
                EmailHelper.SendEmail(UserMail, Body);
            }
            catch (Exception)
            {
            }
            return "success";
        }
        private static string SendChangePasswordOTpPopulateBody(string newpassord, string mailPath)
        {
            return mailPath.Replace("{password}", newpassord);
        }
        public static LoginViewModel SendOTPDetails(LoginViewModel InputModel, string emailOTP, string mailPath)
        {
            LoginViewModel usOutputViewModel = new LoginViewModel();
            try
            {
                string Body = EmailHelper.SendOTpPopulateBody(InputModel, emailOTP, mailPath);
                EmailHelper.SendEmail(InputModel.ExternalEmailID, Body);
               

            }
            catch (Exception)
            {
            }
            return usOutputViewModel;
        }
        private static string SendOTpPopulateBody(LoginViewModel InputModel, string emailOTP, string mailPath)
        {
            return mailPath.Replace("{Name}", InputModel.InternalEmailID)
           //.Replace("{Email}",InputModel.Comp_ContactEmail).Replace("{Phone}",InputModel.Comp_MobileNo)
           .Replace("{subject}", InputModel.UserName).Replace("{otp}", emailOTP);
        }

        public static acqmstmemberSendMailViewModel SendAllDetails(string email, string mailPath)
        {
            acqmstmemberSendMailViewModel usOutputViewModel = new acqmstmemberSendMailViewModel();
            try
            {
                string Body = EmailHelper.SendOTpPopulateBody(email, mailPath);
                EmailHelper.SendEmail(email, Body);

            }
            catch (Exception)
            {
            }
            return usOutputViewModel;
        }
        public static void SendToParticipants(string email, string mailPath)
        {
            
            try
            {
               
                EmailHelper.SendEmail(email, mailPath);

            }
            catch (Exception)
            {
            }
            
        }
        private static string SendOTpPopulateBody(string email, string mailPath)
        {
            return mailPath.Replace("{Name}", email)
           //.Replace("{Email}",InputModel.Comp_ContactEmail).Replace("{Phone}",InputModel.Comp_MobileNo)
           .Replace("{subject}", email).Replace("{otp}", email);
        }

    }
}
