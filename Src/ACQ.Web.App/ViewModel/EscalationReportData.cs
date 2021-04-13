using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class EscalationReportData
    {
        public int Id { get; set; }
        public string MSG_TYPE { get; set; }
        public Nullable<int> aon_id { get; set; }
        public string Service_Lead_Service { get; set; }
        public string item_description { get; set; }
        public string Categorisation { get; set; }
        public string dateofaccordAON { get; set; }
        public string msg { get; set; }
        public string L1_Officer_Name { get; set; }
        public string L1_Officer_Phone { get; set; }
        public string L1_Officer_Email { get; set; }
        public string L2_Officer_Name { get; set; }
        public string L2_Officer_Phone { get; set; }
        public string L2_Officer_Email { get; set; }
        public string L3_Officer_Name { get; set; }
        public string L3_Officer_Phone { get; set; }
        public string L3_Officer_Email { get; set; }
        public string L4_Officer_Name { get; set; }
        public string L4_Officer_Phone { get; set; }
        public string L4_Officer_Email { get; set; }
        public string L5_ADGAcq_Name { get; set; }
        public string L5_ADGAcq_Phone { get; set; }
        public string L5_ADGAcq_Email { get; set; }
        public string L6_JS_AM_Name { get; set; }
        public string L6_JS_AM_Phone { get; set; }
        public string L6_JS_AM_Email { get; set; }
        public string L7_FM_Name { get; set; }
        public string L7_FM_Phone { get; set; }
        public string L7_FM_Email { get; set; }
        public string L8_DG_Acq_Name { get; set; }
        public string L8_DG_Acq_Phone { get; set; }
        public string L8_DG_Acq_Email { get; set; }
        public string L9_AS_FA_Name { get; set; }
        public string L9_AS_FA_Phone { get; set; }
        public string L9_AS_FA_Email { get; set; }
        public Nullable<System.DateTime> date_of_alert { get; set; }
        public Nullable<bool> isModified { get; set; }
        public string Modified_by { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> AlertSentOn { get; set; }
        public Nullable<bool> isAlertSent { get; set; }

    }

    public class EscalationDraftMessage
    {
        public int Id { get; set; }
        public string TaskId { get; set; }
        public string DraftMessage_L1 { get; set; }
        public string DraftMessage_L2 { get; set; }
        public string DraftMessage_L3 { get; set; }
        public string DraftMessage_L4 { get; set; }
    }

    public class datesearch
    {
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public List<EscalationReportData> data { get; set; }
        public bool Status { get; set; }
    }
}