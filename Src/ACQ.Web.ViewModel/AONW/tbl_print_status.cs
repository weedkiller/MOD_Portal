using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
    public class tbl_print_history
    {
        public int id { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<int> meetingid { get; set; }
        public Nullable<int> aon_id { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
