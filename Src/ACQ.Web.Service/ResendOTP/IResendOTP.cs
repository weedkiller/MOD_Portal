using ACQ.Web.ViewModel.ResendOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.Service.ResendOTP
{
   public interface IResendOTP
    {
        ResendOTPViewModel ResendOTP(ResendOTPViewModel oTPViewModel);
    }
}
