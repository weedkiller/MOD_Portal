using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACQ.Web.Data;
using ACQ.Web.ExternalServices.GeneratedPassword;
using ACQ.Web.ViewModel.User;

namespace ACQ.Web.Service.User
{
    public class UserService : IUser
    {
        #region Global variable and settings

        private readonly Gip_AeroIndia2021Entities _entities;
        public UserService()
        {
            _entities = new Gip_AeroIndia2021Entities();
        }


        #endregion
        public LoginViewModel LoginUser(LoginViewModel model)
        {
            LoginViewModel loginView = new LoginViewModel();
            try
            {
                var _isUserExdfxists = _entities.tbl_mst_User.Where(x => x.UserName == model.UserName && x.IsActive == "N").FirstOrDefault();
                if (_isUserExdfxists != null)
                {
                    model.Salt = _isUserExdfxists.Salt;
                }

                byte[] passwordAndSalt = System.Text.Encoding.UTF8.GetBytes(model.Password + model.Salt);
                byte[] hashPass = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSalt);
                string hashCode = Convert.ToBase64String(hashPass);

                var _isUserLog = _entities.tbl_mst_User.Where(x => x.UserName == model.UserName && x.Password == hashCode && x.IsActive == "N").FirstOrDefault();
                if (_isUserLog != null)
                {
                    loginView.UserName = _isUserLog.UserName;
                    loginView.Comp_ReferenceNo = _isUserLog.Comp_ReferenceNo;
                    if (_isUserLog.ExpiryDate<=System.DateTime.Now)
                    {
                        _isUserLog.LoginCount = 0;
                        _isUserLog.ExpiryDate = System.DateTime.Now;
                        _entities.SaveChanges();
                        loginView.Message = "LoginSuccess";
                    }
                    else
                    {
                        loginView.Message = "Blocked";
                    }
                  
                }
                else
                {
                    var _isUserExists = _entities.tbl_mst_User.Where(x => x.UserName == model.UserName || x.Password == model.Password && x.IsActive == "Y").FirstOrDefault();
                    if (_isUserExists != null && _isUserExists.LoginCount <= 4)
                    {
                        _isUserExists.LoginCount = _isUserExists.LoginCount + 1;
                        _entities.SaveChanges();
                        loginView.Message = "LoginFailed";
                    }
                    else if (_isUserExists != null)
                    {
                        _isUserExists.ExpiryDate = System.DateTime.Now.AddHours(1);
                        _entities.SaveChanges();
                        loginView.Message = "Blocked";
                    }
                    else
                    {
                        loginView.Message = "LoginFailed";
                    }
                }
                return loginView;
            }

            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public ForgetPasswordViewModel ForgetPassword(ForgetPasswordViewModel modelinput)
        {
            try
            {
                var IsEmailExist = _entities.tbl_mst_User.Where(x => x.UserName == modelinput.UserName && x.IsActive == "Y" && x.IsFirst == true).FirstOrDefault();
                {
                    if (IsEmailExist == null)
                    {
                        modelinput.Message = "EmailDoesnotexist";
                    }
                    else
                    {
                        modelinput.UserID = IsEmailExist.UserID;
                        modelinput.UserName = IsEmailExist.UserName;
                        modelinput.Password = IsEmailExist.Password;

                        string GenPwd = "aero@" + GeneratedHashKey.GeneratePassword(5);
                        string GetSalt = GeneratedHashKey.CreateSalt(10);
                        string hashString = GeneratedHashKey.GenarateHash(GenPwd, GetSalt);
                        modelinput.TempRefNo = GenPwd;
                        modelinput.NewPassword = hashString;
                        modelinput.Salt = GetSalt;
                        modelinput.Category = IsEmailExist.Category;
                        modelinput.IsActive = IsEmailExist.IsActive;
                        modelinput.Message = "emailexist";
                        #region Responsible for update new password
                        var IsPasswordExist = _entities.tbl_mst_User.Where(x => x.UserID == modelinput.UserID && x.Password == modelinput.Password).FirstOrDefault();
                        if (IsPasswordExist != null)
                        {
                            IsPasswordExist.Password = modelinput.NewPassword;
                            IsPasswordExist.Salt = modelinput.Salt;
                            IsPasswordExist.TempRefNo = modelinput.TempRefNo;
                            _entities.SaveChanges();
                            modelinput.Message = "PasswordChangeSuccessfully";
                        }
                        else
                        {
                            modelinput.Message = "PasswordNotChange";
                        }
                        #endregion
                    }
                }
                return modelinput;
            }
            catch (Exception)
            { }
            return modelinput;
        }

        public ChangePasswordViewModel ChangePassword(string Comp_Reference, ChangePasswordViewModel modelinput)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            try
            {
                var IsPasswordExist = _entities.tbl_mst_User.Where(x => x.Comp_ReferenceNo == Comp_Reference && x.TempRefNo == modelinput.Password).FirstOrDefault();
                 if (IsPasswordExist != null)
                {
                    string GenPwd = modelinput.NewPassword;
                    string GetSalt = GeneratedHashKey.CreateSalt(10);
                    string hashString = GeneratedHashKey.GenarateHash(GenPwd, GetSalt);
                    IsPasswordExist.TempRefNo = GenPwd;
                    IsPasswordExist.Salt = GetSalt;
                    IsPasswordExist.Password = hashString;
                    IsPasswordExist.IsFirst = true;
                    _entities.SaveChanges();
                    model.EmailID = IsPasswordExist.UserName;
                    model.Message = "PasswordChangeSuccessfully";
                }
                else
                {
                    model.Message = "PasswordNotChange";
                }
                return model;
            }
            catch (Exception)
            {
            }
            return model;
        }

        public ChangePasswordViewModel GetChangePassword(string Comp_Reference)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            
                var IsPasswordExist = _entities.tbl_mst_User.Where(x => x.Comp_ReferenceNo == Comp_Reference).FirstOrDefault();
                if (IsPasswordExist != null)
                {
                    model.Password = IsPasswordExist.TempRefNo;
                }
                return model;
        }
    }
    }

