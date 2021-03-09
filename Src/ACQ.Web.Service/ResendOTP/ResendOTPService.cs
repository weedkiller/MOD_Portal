using ACQ.Web.Core.Constant;
using ACQ.Web.Data;
using ACQ.Web.ViewModel.ResendOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.Service.ResendOTP
{
   public class ResendOTPService: IResendOTP
    {
        #region Global Variable
        private readonly Gip_AeroIndia2021Entities _entities;
        public ResendOTPService()
        {
            _entities = new Gip_AeroIndia2021Entities();
        }
        #endregion


        public ResendOTPViewModel ResendOTP(ResendOTPViewModel oTPViewModel)
        {
             if (oTPViewModel.PageName == ResendOTPConstant.resendOTPexhibitorEmail)
            {
                var _userexists = _entities.tbl_mst_Company.Where(x => x.Nodal_Email == oTPViewModel.emailID).FirstOrDefault();
                if (_userexists != null)
                {
                    oTPViewModel.Comp_refNO = _userexists.Comp_ReferenceNo;
                }

                var _ExhibitorExists = _entities.tbl_trn_OTP.Where(x => x.EmailID == oTPViewModel.emailID && x.OTPCategory == "Exhibitor").FirstOrDefault();
                if (_ExhibitorExists != null)
                {
                    oTPViewModel.emailOTP = _ExhibitorExists.EmailOTP;
                }

                oTPViewModel.Message = ResendOTPConstant.resendOTPexhibitorEmail;
            }
            return oTPViewModel;
        }
    }
}
