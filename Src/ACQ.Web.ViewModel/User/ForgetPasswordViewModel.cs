//using ACQ.Web.ViewModel.ResultOutPut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.User
{
   public class ForgetPasswordViewModel 
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Salt { get; set; }
        public string Comp_ReferenceNo { get; set; }
        public bool IsFirst { get; set; }
        public string Category { get; set; }
        public string IsActive { get; set; }
        public Nullable<int> LoginCount { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string TempRefNo { get; set; }
        public Nullable<System.DateTime> RecInsTime { get; set; }
    }
}
