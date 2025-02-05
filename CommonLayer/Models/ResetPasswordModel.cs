using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="password required")]
        [PasswordPropertyText]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
