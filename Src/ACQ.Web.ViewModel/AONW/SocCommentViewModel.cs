﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ViewModel.AONW
{
   public class SocCommentViewModel
    {
        public int SocCommentID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> SoCId { get; set; }
        public string IsActive { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Created_by { get; set; }
        public Nullable<System.DateTime> Created_on { get; set; }
        public Nullable<int> Deleted_by { get; set; }
        public Nullable<System.DateTime> Deleted_on { get; set; }
        
    }
}
