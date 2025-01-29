using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;


using static System.Net.Mime.MediaTypeNames;

namespace RepositoryLayer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly FundooDBContext context;
        private readonly IConfiguration configuration;

        public UserRepository(FundooDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
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
                string token = GenerateToken(result.UserID, result.Email);
                return token;
            }
            return null;
        }

        private string GenerateToken(int UserID,string Email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("UserID",UserID.ToString()),
                new Claim("Email",Email)
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        public ForgetPassword ForgetPasswordMethod(string email)
        {
            Users users = context.Users.FirstOrDefault(x => x.Email == email);
            if (users != null)
            {
                ForgetPassword forgetPassword = new ForgetPassword();
                forgetPassword.UserID = users.UserID;
                forgetPassword.Email=users.Email;
                forgetPassword.Token = GenerateToken(users.UserID, users.Email);
                return forgetPassword;
            }
            else
            {
                throw new Exception("User not Exist for requested email!!");

            }
        }

        public bool ResetPassword(string email,ResetPasswordModel model)
        {
            Users users = context.Users.FirstOrDefault(x => x.Email == email);
            if(users != null)
            {
                if(model.Password!= model.ConfirmPassword)
                {
                    return false;
                }
                else
                {
                   
                    users.Password = PasswordEncrypt( model.ConfirmPassword);
                    context.Users.Update(users);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }



    }
}
