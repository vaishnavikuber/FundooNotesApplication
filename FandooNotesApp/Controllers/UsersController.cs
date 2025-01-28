using CommonLayer.Models;
using ManagerLayer.Interfaces;
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

        public UsersController(IUserManager manager)
        {
            this.manager = manager;
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



    }
}
