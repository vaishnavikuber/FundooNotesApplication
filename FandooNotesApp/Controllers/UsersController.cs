using System;
using System.Linq;
using System.Threading.Tasks;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace FandooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserManager manager;
        private readonly IBus bus;

        public UsersController(IUserManager manager, IBus bus)
        {
            this.manager = manager;
            this.bus = bus;
        }

        [HttpPost]
        [Route("Reg")]
        public IActionResult Register(RegisterModel model)
        {
            var checkMail = manager.IsEmailExist(model.Email);
            if (checkMail)
            {
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Email already exists!", Data = checkMail });
            }
            else
            {
                var result = manager.Registration(model);
                if (result != null)
                {
                    return Ok(new ResponseModel<Users> { Success = true, Message = "Registration Successful", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<Users> { Success = true, Message = "Registration Failed" });
                }
            }
           
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult UserLogin(LoginModel model)
        {
            var result = manager.Login(model);
            if (result != null)
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            }
            return BadRequest(new ResponseModel<string> { Success = false, Message = "Login Failed" });
        }

       
        [HttpGet]
        [Route("forgetpassword")]
        public async Task<IActionResult> ForgetPasswordM(string email)
        {

            try
            {
                if (manager.IsEmailExist(email))
                {
                   
                    ForgetPassword forgetpassword = manager.ForgetPasswordMethod(email);
                    SendMail send = new SendMail();
                    send.SendEmail(forgetpassword.Email, forgetpassword.Token);
                    Uri uri = new Uri("rabbitmq://localhost/FundooNotesEmailQueue");
                    var endPoint = await bus.GetSendEndpoint(uri);
                    await endPoint.Send(forgetpassword);
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent successfully", Data = endPoint.ToString() });

                }
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email provided is not regestered" });

            }
            catch (Exception ex)
            {
                throw ex;
               
            }
        }

        [Authorize]
        [HttpPut]
        [Route("resetpassword")]

        public IActionResult ResetPassword( ResetPasswordModel model)
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
            bool result = manager.ResetPassword(email, model);
            if (result)
            {
                return Ok(new ResponseModel<bool> { Success = true, Message = "Password reset successfull",Data=result });
            }
            return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to reset password" });
        } 


       



    }
}
