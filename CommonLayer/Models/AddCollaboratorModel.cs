using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class AddCollaboratorModel
    {
        [Required(ErrorMessage ="email required")]
        [EmailAddress(ErrorMessage ="enter valid email")]
        public string Email {  get; set; }
        [Required(ErrorMessage ="notes id is required")]
        public int NotesID { get; set; }
    }
}
