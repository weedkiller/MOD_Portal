using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.MasterRole
{
    public class tbl_Master_Role
    {
        public int Id { get; set; }
        public Nullable<int> UserID { get; set; }
        public string UserName { get; set; }
        public Nullable<int> FormMenuID { get; set; }
        public string FormName { get; set; }
        public string IsActive { get; set; }
        public Nullable<System.DateTime> RecTime { get; set; }
        public Nullable<int> CreatedBy { get; set; }
    }
}
