using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.ExternalServices.SecurityAudit
{
   public class uploadDoc
    {
        private string GetIconPath(string fileExtention)
        {
            string Iconpath = "/Images";
            string ext = fileExtention.ToLower();
            switch (ext)
            {
                case ".txt":
                    Iconpath += "/txt.png";
                    break;
                case ".doc":
                case ".docx":
                    Iconpath += "/word.png";
                    break;
                case ".xls":
                case ".xlsx":
                    Iconpath += "/xls.png";
                    break;
                case ".pdf":
                    Iconpath += "/pdf.png";
                    break;
                case ".rar":
                    Iconpath += "/rar.png";
                    break;
                case ".zip":
                case ".7z":
                    Iconpath += "/zip.png";
                    break;
                default:
                    break;
            }
            return Iconpath;
        }
    }
}
