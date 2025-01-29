using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class UserManager : IUserManager
    {

        private readonly IUserRepository user;
        private readonly FundooDBContext context;

        public UserManager(IUserRepository user, FundooDBContext context)
        {
            this.user = user;
            this.context = context;
        }

        public Users Registration(RegisterModel model)
        {
            return user.Registration(model);
        }


        public bool IsEmailExist(string email)
        {
            var result = context.Users.FirstOrDefault(x => x.Email == email);
            if (result !=null )
            {
                return true;
            }
            return false;
        }

        public string Login(LoginModel model)
        {
            return user.Login(model);
        }

        public ForgetPassword ForgetPasswordMethod(string email)
        {
            return user.ForgetPasswordMethod(email);
        }

        public bool ResetPassword(string email, ResetPasswordModel model)
        {
            return user.ResetPassword(email, model);
        }
    }
}
