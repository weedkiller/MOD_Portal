using ACQ.Web.ViewModel.EFile;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACQ.Web.ViewModel.AONW
{

    public class SAVESOCVIEWMODEL
    {
        public List<SAVESOCVIEWMODEL> SOCVIEW { get; set; }
        public List<AttachmentViewModel> FileDetail { get; set; }
        public string aon_id { get; set; }
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
        public string EFileNo { get; set; }
        public string SoC_Type { get; set; }
        public string Path { get; set; }
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
        private string _ErrorMsg { get; set; }
        public string ErrorMsg
        {
            get { return _ErrorMsg; }
            set { _ErrorMsg = value; }
        }

    }

    public class SAVESOCVIEWMODELBluk
    {
        public string aon_id { get; set; }
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
        public string EFileNo { get; set; }
        public string ErrorMsg { get; set; }
        public string SoC_Type { get; set; }
        public string UniqueID { get; set; }
        public string Path { get; set; }
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

    public class SAVESOCVIEWMODELBlukPDF
    {
        public string item_description { get; set; }
        public string id { get; set; }
        public string service_leadservice { get; set; }
        public string categorisation { get; set; }
        public string introduction { get; set; }
        public string mission_needs { get; set; }
        public string how_currently { get; set; }
        public string deficiency_capability { get; set; }
        public string changes_doctr { get; set; }
        public string matrl_soln { get; set; }
        public string capability_inducted { get; set; }
        public string additional_capability { get; set; }
        public string associated_induction { get; set; }
        public string equp_replaced { get; set; }
        public string detailed_justfn { get; set; }
        public string UniqueID { get; set; }
        public string SoC_Type { get; set; }
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