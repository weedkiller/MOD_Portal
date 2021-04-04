using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.User
{
    public class resetpwdViewModel
    {
        public int ResetPwd { get; set; }
        public string UserName { get; set; }
        public string mTokenId { get; set; }
        public Nullable<System.DateTime> RecDate { get; set; }
        public string IsActive { get; set; }
        public string Message { get; set; }

    }
}
