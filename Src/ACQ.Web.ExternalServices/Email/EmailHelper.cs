//using ACQ.Web.ViewModel.IndexPage;
using ACQ.Web.ViewModel.User;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ACQ.Web.ExternalServices.Email
{
    public class EmailHelper
    {
    //    public static BadgeRegistrationViewModel SendLinkBadgeRegistration(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {
    //        BadgeRegistrationViewModel usOutputViewModel = new BadgeRegistrationViewModel();
    //        try
    //        {
    //            string Body = EmailHelper.SendLinkBadgeRegistrationBody(InputModel, mailPath);
    //            EmailHelper.SendEmail(InputModel.Email, Body);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return usOutputViewModel;
    //    }
    //    private static string SendLinkBadgeRegistrationBody(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {

    //        return mailPath
    //            .Replace("{Url}", InputModel.SendLink);
    //    }

    //    public static BadgeRegistrationViewModel SendBadgePassess(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {
    //        BadgeRegistrationViewModel usOutputViewModel = new BadgeRegistrationViewModel();
    //        try
    //        {
    //            string Body = EmailHelper.SendBadgePassessBody(InputModel, mailPath);
    //            EmailHelper.SendEmail(InputModel.Email, Body);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return usOutputViewModel;
    //    }
    //    private static string SendBadgePassessBody(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {

    //        return mailPath;
                
    //    }
    //    public static BadgeRegistrationViewModel DownloadBadgePassess(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {
    //        BadgeRegistrationViewModel usOutputViewModel = new BadgeRegistrationViewModel();
    //        try
    //        {
    //            string Body = EmailHelper.DownloadBadgePassessBody(InputModel, mailPath);
    //            EmailHelper.SendEmail(InputModel.Email, Body);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return usOutputViewModel;
    //    }
    //    private static string DownloadBadgePassessBody(BadgeRegistrationViewModel InputModel, string mailPath)
    //    {

    //        return mailPath
    //        .Replace("{Url}", InputModel.SendLink);
    //    }

    //    public static EventPageViewModel SendEventUsernamaandpassword(EventPageViewModel InputModel, string mailPath)
    //    {
    //        EventPageViewModel usOutputViewModel = new EventPageViewModel();
    //        try
    //        {
    //            string Body = EmailHelper.SendEventUsernamaandpasswordBody(InputModel, mailPath);
    //            EmailHelper.SendEmail(InputModel.Email, Body);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return usOutputViewModel;
    //    }
    //    private static string SendEventUsernamaandpasswordBody(EventPageViewModel InputModel, string mailPath)
    //    {

    //        return mailPath
    //            .Replace("{Url}", InputModel.UrlPath)
    //            .Replace("{UserName}", InputModel.UserName)
    //            .Replace("{Password}", InputModel.Password);
    //    }
    //    public static NodalCompanyViewModel SendNodalUsernamaandpassword(NodalCompanyViewModel InputModel, string mailPath)
    //    {
    //        NodalCompanyViewModel usOutputViewModel = new NodalCompanyViewModel();
    //        try
    //        {
    //            string Body = EmailHelper.SendNodalUsernamaandpasswordBody(InputModel, mailPath);
    //            EmailHelper.SendEmail(InputModel.Email, Body);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return usOutputViewModel;
    //    }
    //    private static string SendNodalUsernamaandpasswordBody(NodalCompanyViewModel InputModel, string mailPath)
    //    {

    //        return mailPath
    //            .Replace("{Url}", InputModel.UrlPathMails)
    //            .Replace("{UserName}", InputModel.UserName)
    //            .Replace("{Password}", InputModel.Password);
    //    }

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
                message.Subject = "OTP";
                message.Body = Body.ToString();
                message.IsBodyHtml = true;
                smtpClient.EnableSsl = true;
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


        //public static EventPageViewModel SendSpaceBookingsEmailOTP(EventPageViewModel InputModel, string mailPath)
        //{
        //    EventPageViewModel usOutputViewModel = new EventPageViewModel();
        //    try
        //    {
        //        string Body = EmailHelper.SendEventUsernamaandpasswordBody(InputModel, mailPath);
        //        EmailHelper.SendEmail(InputModel.Email, Body);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return usOutputViewModel;
        //}
        //public static EventPageViewModel SendForgatPasswordOTP(EventPageViewModel InputModel, string mailPath)
        //{
        //    EventPageViewModel usOutputViewModel = new EventPageViewModel();
        //    try
        //    {
        //        string Body = EmailHelper.SendEventUsernamaandpasswordBody(InputModel, mailPath);
        //        EmailHelper.SendEmail(InputModel.Email, Body);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return usOutputViewModel;
        //}

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

    }
}
