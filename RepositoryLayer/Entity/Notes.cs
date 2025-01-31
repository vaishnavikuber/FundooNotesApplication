﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entity
{
    public class Notes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotesID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Reminder {  get; set; }
        public string Color { get; set; }
        public  string Image { get; set; }
        public bool IsArchive { get; set; }
        public bool IsPin { get; set; }
        public bool IsTrash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [ForeignKey("NotesUser")]
        public int UserID { get; set; }
        [JsonIgnore]
        public virtual Users NotesUser { get; set; }
       
    }
}
