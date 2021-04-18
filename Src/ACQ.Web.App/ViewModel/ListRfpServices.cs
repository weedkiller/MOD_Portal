using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACQ.Web.App.ViewModel
{
    public class ListRfpServices
    {

        public int aon_id { get; set; }
        public string item_description { get; set; }
        public string Categorisation { get; set; }
        public string Service_Lead_Service { get; set; }
        public AttachmentData attachment { get; set; }
    }

    public class AttachmentData
    {
        public long AttachmentID { get; set; }
        public string AttachmentFileName { get; set; }
        public string Path { get; set; }
        public Nullable<System.DateTime> RecDate { get; set; }
    }

    public class ApiResponseRfp
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ListRfpServices data { get; set; }
    }
}