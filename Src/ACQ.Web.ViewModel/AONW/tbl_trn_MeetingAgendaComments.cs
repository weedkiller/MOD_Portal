using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public class tbl_trn_MeetingAgendaComments
    {
        public int ID { get; set; }
        public int MeetingAgendaID { get; set; }
        public int UserID { get; set; }
        public string ProposalComment { get; set; }
        public string BackgroundComment { get; set; }
        public string ApprovalSoughtComment { get; set; }
        public string DiscussionComment { get; set; }
        public string DecisionComment { get; set; }
        public Nullable<int> Locked { get; set; }
        public string AgendaName { get; set; }
        public string MemberName { get; set; }
        public string MemberDesg { get; set; }
    }
}
