using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.Models;
using FandooNotesApp.Helpers;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Migrations;

namespace FandooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserManager manager;
        private readonly IBus bus;
        private readonly FundooDBContext context;
        private readonly ILogger<UsersController> _logger;
        private readonly IDistributedCache distributedCache;

        public UsersController(IUserManager manager, IBus bus, FundooDBContext context, ILogger<UsersController> _logger, IDistributedCache distributedCache)
        {
            this.manager = manager;
            this.bus = bus;
            this.context = context;
            this._logger = _logger;
            this.distributedCache = distributedCache;
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


        [HttpPost("Register")]
        public IActionResult RegisterUser(RegisterModel userReg)
        {
            try
            {
                var ifEmailExist = manager.IsEmailExist(userReg.Email);
                if (ifEmailExist)
                {
                    _logger.LogWarning("The EmailId Already Exist");
                    return Ok(new { success = false, message = "The EmailId Already Exist" });
                }
                var resUser = manager.Registration(userReg);
                if (resUser != null)
                {
                    _logger.LogInformation("Registeration Successfull");
                    return Created("User Added Successfully", new { success = true, data = resUser });
                }
                else
                {
                    _logger.LogError("Registeration Unsuccessfull");
                    return BadRequest(new { success = false, message = "Something Went Wrong" });
                }
            }
            catch (AppException ex)
            {
                _logger.LogCritical(ex, " Exception Thrown...");
                return NotFound(new { success = false, message = ex.Message });
            }
        }





        [HttpPost]
        [Route("Login")]
        public IActionResult UserLogin(LoginModel model)
        {
            //var result = manager.Login(model);
            //if (result != null)
            //{
            //    return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            //}
            //return BadRequest(new ResponseModel<string> { Success = false, Message = "Login Failed" });

            try
            {
                var result = manager.Login(model);
                //_logger.LogInformation(result);
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = result });
            }
            catch(AppException ex)
            {
                _logger.LogWarning(ex.ToString());
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Login Failed" });
            }
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
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email provided is not registered" });

            }
            catch (AppException ex)
            {
                throw ex;

            }
        }

        [Authorize]
        [HttpPut]
        [Route("resetpassword")]

        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
                bool result = manager.ResetPassword(email, model);
                _logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Password reset successfull", Data = result });
            }
           catch(AppException ex)
            {
                _logger.LogWarning(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to reset password" });
            }
            
            
        }

        [HttpGet]
        [Route("redisgetallusers")]
        public async Task<IActionResult> GetAllUsersUsingRedisCache()
        {
            var cacheKey = "UsersList";
            string serializedUsersList;
            var usersList = new List<Users>();
            var redisUsersList = await distributedCache.GetAsync(cacheKey);
            if (redisUsersList != null)
            {
                serializedUsersList = Encoding.UTF8.GetString(redisUsersList);
                usersList = JsonConvert.DeserializeObject<List<Users>>(serializedUsersList);
            }
            else
            {
                usersList = context.Users.ToList();
                serializedUsersList = JsonConvert.SerializeObject(usersList);
                redisUsersList = Encoding.UTF8.GetBytes(serializedUsersList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                await distributedCache.SetAsync(cacheKey, redisUsersList, options);
            }
            return Ok(usersList);
        }




        //review

        //[HttpGet]
        //[Route("getallusers")]
        //public IActionResult GetAllUsers()
        //{
        //    List<Users> usersList = new List<Users>();
        //    var users = context.Users.Select(x => x);
        //    if (users != null)
        //    {
        //        foreach (Users user in users)
        //        {
        //            usersList.Add(user);
        //        }
        //        int count = usersList.Count;
        //        return Ok(new ResponseModel<List<Users>> { Success = true, Message = "Here are the users", Data = usersList });
        //    }
        //    return BadRequest(new ResponseModel<List<Users>> { Success = false, Message = "Failed to fetct Users" });
        //}

        //[HttpGet]
        //[Route("getuserbyid")]
        //public IActionResult GetUserById(int userId)
        //{
        //    var user = context.Users.FirstOrDefault(x => x.UserID == userId);

        //    if (user != null)
        //    {
        //        return Ok(new ResponseModel<Users> { Success = true, Message = "fetch successfull", Data = user });
        //    }
        //    return BadRequest(new ResponseModel<Users> { Success = false, Message = "failed to fatch data" });
        //}

        //[HttpGet]
        //[Route("namestartswith")]
        //public IActionResult NameStartsWith()
        //{

        //    var users = context.Users.Where(x => x.FirstName.StartsWith(@"A")).ToList();
        //    if (users != null)
        //    {

        //        return Ok(new ResponseModel<List<Users>> { Success = true, Message = "The names starts with a:", Data = users });
        //    }
        //    return BadRequest(new ResponseModel<List<Users>> { Success = false, Message = "failed to fetch" });
        //}

        //[HttpGet("orderby")]
        //public IActionResult OrderByName()
        //{
        //    List<Users> usersList = new List<Users>();
        //    var ascUsers = context.Users.OrderBy(x => x.FirstName).ToList();
        //    var descUsers = context.Users.OrderByDescending(x => x.FirstName).ToList();

        //    if (ascUsers != null && descUsers != null)
        //    {
        //        return Ok(new ResponseModel<List<Users>> { Success = true, Message = "The names with ascending", Data = ascUsers });
        //    }
        //    return BadRequest(new ResponseModel<List<Users>> { Success = false, Message = "failed to fetch" });

        //}


        //[HttpGet]
        //[Route("averageAge")]
        //public IActionResult AverageAge()
        //{
        //    double users = context.Users.Select(x =>DateTime.Now.Year -x.DateOfBirth.Year).Average();
        //    if (users != 0)
        //    {
        //        return Ok(new ResponseModel<double> { Success = true, Message = "successfully fetched", Data = users });
        //    }
        //    return BadRequest(new ResponseModel<double> { Success = false, Message = "failed to calculate average" });

        //}

        //[HttpGet]
        //[Route("oldestandyoungest")]
        //public IActionResult OldestAndYoungest()
        //{

        //    var youngUsers = DateTime.Now.Year - context.Users.Select(x => x).Max(x=>x.DateOfBirth).Year;
        //    var OldestUsers = DateTime.Now.Year - context.Users.Select(x => x).Min(x => x.DateOfBirth).Year;

        //    if (youngUsers != null && OldestUsers!=null)
        //    {
        //        return Ok(new ResponseModel<string> { Success = true, Message = "successfull", Data ="youngest user:"+youngUsers+"  oldest user:"+OldestUsers  });
        //    }
        //    return BadRequest(new ResponseModel<string> { Success = false, Message = "failed" });


        //}



    }
}
