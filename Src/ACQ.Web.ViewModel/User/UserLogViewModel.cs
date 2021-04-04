using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.User
{
   public class UserLogViewModel
    {
        public int LogId { get; set; }
        public string UserEmail { get; set; }
        public string IPAddress { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> Restime { get; set; }
        public string IsActive { get; set; }
        public string Message { get; set; }
    }

    
}
