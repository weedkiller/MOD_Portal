using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public class acq_soc_master
    {
        public int aon_id { get; set; }
        public string item_description { get; set; }
        public string DPP_DAP { get; set; }
        public string AoN_Accorded_By { get; set; }
        public Nullable<System.DateTime> SoDate { get; set; }
        public string Categorisation { get; set; }
        public string Service_Lead_Service { get; set; }
        public string Quantity { get; set; }
        public string Cost { get; set; }
        public string Tax_Duties { get; set; }
        public string Type_of_Acquisition { get; set; }
        public string Trials_Required { get; set; }
        public string Essential_parameters { get; set; }
        public string Any_other_aspect { get; set; }
        public string IC_percentage { get; set; }
        public string Option_clause_applicable { get; set; }
        public string Offset_applicable { get; set; }
        public string AMC_applicable { get; set; }
        public string AMCRemarks { get; set; }
        public string Warrenty_applicable { get; set; }
        public string Warrenty_Remarks { get; set; }
        public Nullable<decimal> AoN_validity { get; set; }
        public string AoN_validity_unit { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string SoCType { get; set; }
        public string SoCCase { get; set; }
        public Nullable<int> EFileId { get; set; }
        public string UniqueID { get; set; }
        public string SystemCase { get; set; }
        public string EPP { get; set; }
        public string SocAName { get; set; }
        public string SocADesignation { get; set; }
        public string SocAApprovalRef { get; set; }
        public string SocAApprovalDate { get; set; }
        public string SocSDName { get; set; }
        public string SocSDDesignation { get; set; }
        public string SocSDPhone { get; set; }
        public string SocSDDate { get; set; }
    }
}
