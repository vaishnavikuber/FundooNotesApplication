using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRepository
    {
        public Users Registration(RegisterModel model);

        public string Login(LoginModel model);
        public ForgetPassword ForgetPasswordMethod(string email);
        public bool ResetPassword(string email, ResetPasswordModel model);
    }
}
