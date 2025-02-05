using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class ForgetPassword
    {
        [Required(ErrorMessage ="id is required")]
        public int UserID { get; set; }
        [Required(ErrorMessage ="email required")]
        [EmailAddress(ErrorMessage ="enter valid email")]
        public string Email { get; set; }
        public string Token { get; set; }

    }
}
