using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class EscalationReportData
    {
        public long Pkey { get; set; }
        public int aon_id { get; set; }
        public string Service_Lead_Service { get; set; }
        public string item_description { get; set; }
        public Nullable<System.DateTime> Date_of_Accord_of_AoN { get; set; }
        public string Cost { get; set; }
        public string Categorisation { get; set; }
        public string IC_percentage { get; set; }
        public string Trials_Required { get; set; }
        public string TaskDescription { get; set; }
        public Nullable<System.DateTime> completed_on { get; set; }
        public Nullable<int> no_of_weeks { get; set; }
        public string dap_timeline { get; set; }
        public decimal TaskSlno { get; set; }
        public decimal CaseTaskSlno { get; set; }
        public string Responsible_Level1 { get; set; }
        public string Responsible_Level2 { get; set; }
        public string Responsible_Level3 { get; set; }
        public string Responsible_Level4 { get; set; }
        public int EscalationTime { get; set; }
        
    }
}