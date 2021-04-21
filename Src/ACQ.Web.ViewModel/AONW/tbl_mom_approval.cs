using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public class tbl_mom_approval
    {
        public int Id { get; set; }
        public Nullable<int> meeting_Id { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<bool> isDeleted { get; set; }
    }
}
