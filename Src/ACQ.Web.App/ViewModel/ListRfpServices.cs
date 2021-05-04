using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class Service
    {
        public int SectionId { get; set; }
        public string Services { get; set; }
    }
    public class ListRfpServices
    {

        public int aon_id { get; set; }
        public string item_description { get; set; }
        public string Categorisation { get; set; }
        public string Service_Lead_Service { get; set; }
        public DateTime? SOCDate { get; set; }
        public string SOCCase { get; set; }
        public string SOCAname { get; set; }
        public string Delegation { get; set; }
        public AttachmentData attachment { get; set; }
    }

    public class VendorsType
    {
        public int Id { get; set; }
        public string VendorType { get; set; }
        public string ConnectedVendors { get; set; }
    }
    public class AttachmentData
    {
        public int Id { get; set; }
        public int aon_id { get; set; }
        public string UploadedDraftRFP { get; set; }
        public bool IsSent { get; set; }
        public int VendorType { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> Sentdate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }

    public class sharedRFP
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Share_RFP { get; set; }
        public int SOCID { get; set; }
        public string UploadedRFP { get; set; }
        public string Service { get; set; }
        public string Categorisation { get; set; }
        public string UserRole { get; set; }
        public Nullable<System.DateTime> CommentedDate { get; set; }
        public Nullable<bool> CommentSubmited { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public Nullable<System.DateTime> AcceptedDate { get; set; }
        public Nullable<System.DateTime> shareddate { get; set; }

        public List<circulationComment> Comments { get; set; }
    }


    public class collegiateRFP
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Commentkey { get; set; }
        public int CommentId { get; set; }
        public int Share_RFP { get; set; }
        public string Chapter { get; set; }
        public string Page { get; set; }
        public string Para { get; set; }
        public string CorrectionFor { get; set; }
        public string Suggestion { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public string Service { get; set; }
        public string Categorisation { get; set; }
        public string UserRole { get; set; }
        public string UploadedRFP { get; set; }
        public int SOCID { get; set; }
    }

    public class ApiResponseRfp
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ListRfpServices data { get; set; }
        public List<VendorsType> vendors { get; set; }
    }
    public class circulationComment
    {
        public int CommentId { get; set; }
        public int circulation_Id { get; set; }
        public string Chapter { get; set; }
        public string Page { get; set; }
        public string Para { get; set; }
        public string CorrectionFor { get; set; }
        public string Suggestion { get; set; }
        public DateTime saveddate { get; set; }
    }
    public class UploadComment
    {
        public HttpPostedFileBase MyFile { get; set; }
        public int CommentId { get; set; }
    }

    public class UserViewModel
    {
        
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
        public string Phone { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public string InternalEmailID { get; set; }
        public string ExternalEmailID { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public string IPAddress { get; set; }
        public string Designation { get; set; }
        public string Emailotp { get; set; }
        public string Pswd_Salt { get; set; }
        public string Flag { get; set; }
        public Nullable<int> MemberID { get; set; }
        public string deptt_description { get; set; }
        public int? SectionID { get; set; }

        public Nullable<int> SrNo { get; set; }
        public string IsActive { get; set; }
        public string Message { get; set; }
    }
}