using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;


using static System.Net.Mime.MediaTypeNames;

namespace RepositoryLayer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly FundooDBContext context;

        public UserRepository(FundooDBContext context)
        {
            this.context = context;
        }

        public Users Registration(RegisterModel model)
        {
            Users users = new Users();
            users.FirstName=model.FirstName;
            users.LastName=model.LastName;
            users.DateOfBirth=model.DateOfBirth;
            users.Gender=model.Gender;          
            users.Email = model.Email;
            users.Password=PasswordEncrypt(model.Password);
            context.Users.Add(users);
            context.SaveChanges();
            return users;
        }

        

        public string PasswordEncrypt(string password)
        {

            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }


        }

        
        public string Login(LoginModel model)
        {
            
            var result=context.Users.FirstOrDefault(x=>x.Email == model.Email && x.Password== PasswordEncrypt( model.Password));
            if (result!=null)
            {
                return  "Login Successfull";
            }
            return null;
        }




    }
}
