using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
  public  class acqmstmemberSendMailViewModel
    {
        public List<acqmstmemberSendMailViewModel> SOCMailVIEW { get; set; }
        public int MemberID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> SrNo { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string meeting_type { get; set; }
        public Nullable<System.DateTime> from_date { get; set; }
        public Nullable<System.DateTime> till_date { get; set; }
        public string member_type { get; set; }
        public string designation { get; set; }
        public string item_desc { get; set; }
        public string service { get; set; }
        public string Email { get; set; }

        public virtual tbl_tbl_User tbl_tbl_User
        {
            get; set;
        }
    }
}
