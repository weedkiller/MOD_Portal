using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ExternalServices.Email
{
    public static class OTPHelper
    {
        public static string GenerateMobileOTP()
        {
            string otp = string.Empty;
            try
            {
                string numbers = "1234567890";
                string characters = numbers;
                int length = int.Parse("6");
                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (otp.IndexOf(character) != -1);
                    otp += character;
                }
                return otp;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return otp;
        }
        public static string GenerateEmailOTP()
        {
            string otp = string.Empty;
            try
            {
                //  string numbers = "1234567890";
                string numbers = "1234567890";

                string characters = numbers;
                int length = int.Parse("6");
                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (otp.IndexOf(character) != -1);
                    otp += character;
                }
                return otp;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return otp;
        }
      
    }
}
