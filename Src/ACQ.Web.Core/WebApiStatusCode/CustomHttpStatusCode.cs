#region Using
using System;
using System.Net;
#endregion

namespace ACQ.Web.Core.WebApiStatusCode
{
    public static class CustomHttpStatusCode
    {
        public const string Return200 = "OK – Eyerything is working";
        public const string Return201 = "OK – New resource has been created";
        public const string Return204 = "OK – The resource was successfully deleted";
        public const string Return304 = "Not Modified – The client can use cached data";
        public const string Return400 = "Bad Request – The request was invalid or cannot be served.The exact error should be explained in the error payload.E.g. „The JSON is not valid“";
        public const string Return401 = "Unauthorized – The request requires an user authentication";
        public const string Return403 = "Forbidden – The server understood the request, but is refusing it or the access is not allowed.";
        public const string Return422 = "Unprocessable Entity – Should be used if the server cannot process the enitity, e.g. if an image cannot be formatted or mandatory fields are missing in the payload.";
        public const string Return500 = "Internal Server Error – API developers should avoid this error.If an error occurs in the global catch blog, the stracktrace should be logged and not returned as response.";
        public const string Return404 = "Not Found";
    }

    public static class PanVerifyCustomHttpStatusCode
    {
        public const string Return1 = "1";
        public const string Return2 = "2";
        public const string Return3 = "3";
        public const string Return4 = "4";
        public const string Return5 = "5";
        public const string Return6 = "6";
        public const string Return7 = "7";
        public const string Return8 = "8";
        public const string Return9 = "9";
        public const string Return10 = "10";
        public const string Return11 = "11";
        public const string Return12 = "12";

        public const string Return1Message = "Success";
        public const string Return2Message = "System Error";
        public const string Return3Message = "Authentication Failure";
        public const string Return4Message = "User not authorized";
        public const string Return5Message = "No PANs Entered";
        public const string Return6Message = "User validity has expired";
        public const string Return7Message = "Number of PANs exceeds the limit(5)";
        public const string Return8Message = "Not enough balance";
        public const string Return9Message = "Not an HTTPs request";
        public const string Return10Message = "POST method not used";
        public const string Return11Message = "SLAB_CHANGE_RUNNING";
        public const string Return12Message = "WRONG_VERSION_FOR_RESPONSE";

    }
    public static class Webapisecretkey
    {
        public const string secretkey = "trGa8uRafy76jfW7jVKfJ0RIF";

        public static string GetClientIPAddress()
        {
            string IPAddress = string.Empty;
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }
    }
}
