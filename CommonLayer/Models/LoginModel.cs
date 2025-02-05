using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "provide valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="without password can't be logged in")]
        [PasswordPropertyText]
        public string Password { get; set; }

    }
}
