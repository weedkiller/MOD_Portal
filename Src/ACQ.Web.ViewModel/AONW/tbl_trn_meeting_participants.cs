using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public  class tbl_trn_MeetingParticipants
    {
        public int Id { get; set; }
        public Nullable<int> MeetingID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string UserName { get; set; }
        public string UserDesignation { get; set; }
        public string MemberType { get; set; }
        public string Status { get; set; }
    }
}
