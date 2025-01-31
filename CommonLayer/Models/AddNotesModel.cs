using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class AddNotesModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
       
        public DateTime CreatedAt { get; set; }
        
    }
}
