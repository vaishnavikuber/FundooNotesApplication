using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class LabelModel
    {
        [Required(ErrorMessage ="without name label cannot be created")]
        public string LabelName { get; set; }
        [Required(ErrorMessage ="notes id required")]
        public int NotesID { get; set; }
    }
}
