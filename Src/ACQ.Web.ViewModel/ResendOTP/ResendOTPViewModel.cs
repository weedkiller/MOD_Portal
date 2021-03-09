//using ACQ.Web.ViewModel.ResultOutPut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.ResendOTP
{
  public  class ResendOTPViewModel 
    {
        public string emailOTP { get; set; }
        public string mobileOTP { get; set; }
        public string PageName { get; set; }

        public string Comp_refNO { get; set; }
        public string emailID { get; set; }
        public string MobileNo { get; set; }
        public string CountrName { get; set; }
    }
}
