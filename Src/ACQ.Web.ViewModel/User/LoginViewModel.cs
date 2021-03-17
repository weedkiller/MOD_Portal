//using ACQ.Web.ViewModel.ResultOutPut;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.User
{
    public class LoginViewModel //: BaseViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Emailotp { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)] 
        public string InternalEmailID { get; set; }
        public string ExternalEmailID { get; set; }

        [Required(ErrorMessage = "The password is required")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{5,}", ErrorMessage = "Your password must be at least 5 characters long and contain at least 1 letter and 1 number")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Phone { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public Nullable<int> ReportingTo { get; set; }
        public string Designation { get; set; }
        public string RankUser { get; set; }
        public string LoginAllowed { get; set; }
        public Nullable<int> SectionID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ErrorMsg { get; set; }
        public Nullable<int> MemberID { get; set; }
        public string deptt_description { get; set; }

        public Nullable<int> SrNo { get; set; }
        public string IsActive { get; set; }
        public string Message { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public long UserID { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Enter Confirmation New Password")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirmation Password Must Match.")]
        public string ConfirmPassword { get; set; }
        public string Comp_ReferenceNo { get; set; }
        public string EmailID { get; set; }
        public bool IsFirst { get; set; }
        public string Category { get; set; }
        [DataType(DataType.Password)]
        public string TempRefNo { get; set; }
        public string Salt { get; set; }
        public string IsActive { get; set; }
    }

}
