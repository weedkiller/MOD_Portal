using ACQ.Web.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACQ.Web.Service.User
{
    public interface IUser
    {
        LoginViewModel LoginUser(LoginViewModel model);
        ForgetPasswordViewModel ForgetPassword(ForgetPasswordViewModel modelinput);
        ChangePasswordViewModel ChangePassword(string Comp_Reference, ChangePasswordViewModel modelinput);
        ChangePasswordViewModel GetChangePassword(string Comp_Reference);
    }
}
