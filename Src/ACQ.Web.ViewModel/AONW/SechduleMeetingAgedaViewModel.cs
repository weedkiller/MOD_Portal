using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACQ.Web.ViewModel.AONW
{
    public class SechduleMeetingAgedaViewModel
    {
        public string meeting_id { get; set; }
        public string dac_dpb { get; set; }
        public Nullable<System.DateTime> meeting_date { get; set; }
        public Nullable<System.DateTime> Date_of_Issue_of_Minutes { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string Remarks { get; set; }
        public string Meeting_Number { get; set; }
        public string Meeting_Year { get; set; }
        public string Status { get; set; }
        [AllowHtml]
        public string officers_participated { get; set; }
        public List<string> officers_participated_list { get; set; }
        [AllowHtml]
        public string Distribution_List { get; set; }
        public string TypeofAgenda { get; set; }
        public string AgendaItem { get; set; }
        // public virtual acq_meeting_master acq_meeting_master { get; set; }
        public List<MeetingAgenda> TrnListMeeting { get; set; }
        public List<AONDescription> AonDescriptionddl { get; set; }
        public List<SechduleMeetingAgedaViewModel> ListofMeeting { get; set; }
        public List<MeetingParticipants> Participants { get; set; }
        public string Discussion { get; set; }
    }
   

    public class AONDescription
    {
        public long aon_id { get; set; }
        public String item_description { get; set; }
        public String SoCCase { get; set; }
    }
    public class MeetingAgenda
    {
        public long Pid { get; set; }
        public Nullable<int> meeting_id { get; set; }
        public Nullable<int> TypeofAgenda { get; set; }
        public string TypeofAgendaDescription { get; set; }
        public string AgendaItem { get; set; }
        public string Remark { get; set; }
        public string Previous_Meeting { get; set; }
        public string AgendaItem1 { get; set; }
        public string IsActive { get; set; }
        public string AONNumber { get; set; }
        public Nullable<System.DateTime> RecInsTime { get; set; }
        public List<AONDescription> AonDescriptionddl { get; set; }
        public string Version { get; set; }

        [AllowHtml]
        public string Proposal { get; set; }

        [AllowHtml]
        public string Background { get; set; }

        [AllowHtml]
        public string Approval_sought { get; set; }
        [AllowHtml]
        public string Discussion { get; set; }

        [AllowHtml]
        public string Decision { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> MeetingAgendaDate { get; set; }
        public string MeetingAgendaDateString { get; set; }
        public string ServiceLead { get; set; }
        public string Msg { get; set; }
        public string Comments { get; set; }
        public MeetingAgendaComment MeetingAgendaComment { get; set; }
        public List<MeetingAgendaComment> MeetingAgendaCommentList { get; set; }
    }
    public class MeetingParticipants
    {
        public int MemberID { get; set; }
        public int UserID { get; set; }
        public string meeting_type { get; set; }
        public string member_type { get; set; }
        public string designation { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
         
    }
    public class MeetingAgendaComment
    {
        public int ID { get; set; }
        public int MeetingAgendaID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string ProposalComment { get; set; }
        public string BackgroundComment { get; set; }
        public string ApprovalSoughtComment { get; set; }
        public string DiscussionComment { get; set; }
        public string DecisionComment { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<int> Locked {get;set;}
    }
   
}
