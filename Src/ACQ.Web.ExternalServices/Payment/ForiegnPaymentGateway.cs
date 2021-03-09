using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ExternalServices.Payment
{
    //public class ForiegnPaymentGateway
    //{
    //    public ForiegnPaymentGateway()
    //    {
    //        Config.Configure((Juspay.ExpressCheckout.Base.Config.Environment)Convert.ToInt32(ConfigurationManager.AppSettings["jusPayEnviornment"]), ConfigurationManager.AppSettings["MerchantId"], ConfigurationManager.AppSettings["apiKey"]);
    //    }
    //    public async Task<ECApiResponse> CreateOrder(Dictionary<string, string> dictonary)
    //    {
    //        Console.WriteLine(dictonary["order_id"]);
    //        //text = dictonary["order_id"];
    //        // Note:: Jusat APIs are all `async`.
    //        // in asp .NET, if you call Result to get the data from a Task<> object
    //        // in the same context / thread as the action handler it will result in a 
    //        // deadlock. To avoi this scanario, use async-await  
    //        ECApiResponse response = await Juspay.ExpressCheckout.Orders.CreateOrder(dictonary);
    //        return response;
    //    }
    //    public ECApiResponse GetStatus(string orderId)
    //    {
    //        ECApiResponse response = Juspay.ExpressCheckout.Orders.GetStatus(orderId).Result;
    //        return response;
    //    }

    //    public string DecryptForiegn(string input, string key)
    //    {
    //        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
    //        byte[] toEncryptArray = Convert.FromBase64String(input);
    //        RijndaelManaged rDel = new RijndaelManaged();
    //        rDel.Key = keyArray;
    //        rDel.Mode = CipherMode.ECB;
    //        rDel.Padding = PaddingMode.None;
    //        ICryptoTransform cTransform = rDel.CreateDecryptor();
    //        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
    //        return UTF8Encoding.UTF8.GetString(resultArray);
    //    }
    //}
}
