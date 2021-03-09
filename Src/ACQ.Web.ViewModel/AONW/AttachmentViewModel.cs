using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public class AttachmentViewModel
    {
        public long AttachmentID { get; set; }
        public Nullable<int> aon_id { get; set; }
        public string AttachmentFileName { get; set; }
        public string Path { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> RecDate { get; set; }
        public Nullable<int> RefId { get; set; }
    }
}
