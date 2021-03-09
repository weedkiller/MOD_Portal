using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ACQ.Web.ViewModel.EFile
{
    public class Efile
    {
        public class SAVEEfile
        {
            public int EfileID { get; set; }
            public string EfileCode { get; set; }
            public string Subjects { get; set; }
            public string ReferenceNo { get; set; }
            public string EfileContentDetails { get; set; }
            public int aon_id { get; set; }

            public string aon_ids { get; set; }
            public string SOCfile1 { get; set; }
            public string SOCfile2 { get; set; }
            public string SOCfile3 { get; set; }
            public string Filepath { get; set; }

            public string ContentType { get; set; }


            public string item_description { get; set; }
            public string DPP_DAP { get; set; }
            public DateTime? SoDate { get; set; }
            [AllowHtml]
            public string EditorContent { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<int> Modifiedby { get; set; }
            public Nullable<System.DateTime> DeletedOn { get; set; }
            public Nullable<int> Isread { get; set; }
            public Nullable<Boolean> Efile_status { get; set; }
            public string Msg { get; set; }
            public List<SAVEEfile> SOCVIEW { get; set; }

            public List<EfileDetails> _Efiledetails { get; set; }

            public virtual ICollection<FileDetail> _FileDetail { get; set; }

            public List<Efile.SAVEEfile> SOCDesription { get; set; }

            public string XmlFileDetails { get; set; }
        }
        public class FileDetail
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            //public string Extension { get; set; }
            public string FilePath { get; set; }

            //public int SupportId { get; set; }
            // public virtual Support Support { get; set; }

        }
        //public class Support
        //{
        //    public DbSet<Support> Supports { get; set; }
        //}
        public class Efiles_routing
        {
            public int EfileRouteID { get; set; }
            public int EfileID { get; set; }
            public int UserId { get; set; }
            public int Efile_sequence { get; set; }
            public Nullable<Boolean> IsActive { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<int> Modifiedby { get; set; }
            public Nullable<System.DateTime> DeletedOn { get; set; }
            public string Msg { get; set; }
            public string Flag { get; set; }
        }
        public class Notefile
        {
            public int NoteFileId { get; set; }
            public int EfileID { get; set; }

            public string EfileCode { get; set; }

            public int UserId { get; set; }
            public string Filenames { get; set; }
            public string Filepath { get; set; }
            public string DownnloadFilepath { get; set; }
            public int Sequence { get; set; }
            public Nullable<Boolean> NoteFilestatus { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<int> Modifiedby { get; set; }
            public Nullable<System.DateTime> DeletedOn { get; set; }
            public string Msg { get; set; }
        }
        public class EfileDetails
        {
            public int EfileRouteID { get; set; }
            public int EfileID { get; set; }
            public int aon_id { get; set; }
            //public int UserId { get; set; }

            public string UserName { get; set; }
            public int Efile_sequence { get; set; }
            public string EfileContentDetails { get; set; }
            public string EditorContent { get; set; }
            public string Designation { get; set; }
            public string Subjects { get; set; }
            public string ReferenceNo { get; set; }
            public string EfileDate { get; set; }
            public int userID { get; set; }
        }
    }
    public static class EfileData
    {
        public static string CreateXML<T>(this ICollection<T> list)
        {
            var elementType = typeof(T);

            StringBuilder sbFinalString = new StringBuilder();
            sbFinalString.Append("<ROOT>");
            foreach (var item in list)
            {
                string lProperty = "";
                foreach (var propInfo in elementType.GetProperties())
                {
                    // propInfo.GetType();
                    // row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                    if (propInfo.GetValue(item) != null)
                        lProperty = lProperty + " " + propInfo.Name + " = '" + propInfo.GetValue(item, null).ToString() + "'";
                    else
                        lProperty = lProperty + " " + propInfo.Name + " = '" + DBNull.Value.ToString() + "'";
                }

                sbFinalString.Append("<ROW" + lProperty + ">");
                sbFinalString.Append("</ROW>");
            }
            sbFinalString.Append("</ROOT>");
            return sbFinalString.ToString();
        }
    }
    
}
