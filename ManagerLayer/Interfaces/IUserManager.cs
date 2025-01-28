using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interfaces
{
    public interface IUserManager
    {
        public Users Registration(RegisterModel model);
        public bool IsEmailExist(string email);
        public string Login(LoginModel model);

    }
}
