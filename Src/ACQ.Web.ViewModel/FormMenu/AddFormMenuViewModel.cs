using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.FormMenu
{
    public class AddFormMenuViewModel
    {
        public int ID { get; set; }
        public string From_menu { get; set; }
        public string ActionName { get; set; }
        public string Controller { get; set; }
        public string FromName { get; set; }
        public string IsActive { get; set; }
        public Nullable<System.DateTime> RecTime { get; set; }
        public Nullable<int> MenuId { get; set; }
        public List<AddFormMenuViewModel> menuList { get; set; }
        public List<AddFormMenuViewModel> parentList { get; set; }
        public List<AddFormMenuViewModel> chidList { get; set; }
        public List<UserViewModel> UserList { get; set; }

        public List<roleViewModel> roleList { get; set; }
        public List<string> isChecked { get; set; }

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

       // public List<UserViewModel> UserList { get; set; }

    }

    public class roleViewModel
    {
        public bool isChecked { get; set; }
        public int Id { get; set; }
        public Nullable<int> UserID { get; set; }
        public string UserName { get; set; }
        public Nullable<int> FormMenuID { get; set; }
        public string FormName { get; set; }
        public string IsActive { get; set; }
        public Nullable<System.DateTime> RecTime { get; set; }
        
    }
}
