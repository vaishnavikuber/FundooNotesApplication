using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]{2,}$",ErrorMessage ="name should contain minimum 3 characters and starts with capital letter")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage ="email is required")]
        [EmailAddress(ErrorMessage ="provide valid email")]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
