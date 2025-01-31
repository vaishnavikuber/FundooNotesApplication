using System;
using System.Collections.Generic;
using System.Linq;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Migrations;

namespace FandooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {

        private readonly INotesManager manager;
        private readonly FundooDBContext context;

        public NotesController(INotesManager manager, FundooDBContext context)
        {
            this.manager = manager;
            this.context = context;
        }

        [Authorize]
        [HttpPost]
        [Route("addnotes")]
        public IActionResult AddNotes(AddNotesModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
            var notes = manager.AddNotes(model, Convert.ToInt32(userId));
            if (notes != null)
            {
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Notes added successfully", Data = notes });
            }
            return BadRequest(new ResponseModel<Notes> { Success = false, Message = "Failed to add notes" });
        }

        [Authorize]
        [HttpGet]
        [Route("getallnotes")]
        public IActionResult GetAllNotes()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
            var notes = manager.GetAllNotes(Convert.ToInt32(userId));
            if (notes != null)
            {
                return Ok(new ResponseModel<List<Notes>> { Success = true, Message = "Notes  are :", Data=notes });
            }
            return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "Failed to get notes" });

        }
       
        
        [Authorize]
        [HttpPut]
        [Route("editnotes")]
        
        public IActionResult UpdateNotes(int notesId,EditNotesModel model)
        {
            
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
            bool result = manager.EditNotes(model,notesId, Convert.ToInt32(userId));
            if (result)
            {               
                return Ok(new ResponseModel<bool> { Success = true, Message = "Notes updated successfully" });
            }
            return BadRequest(new ResponseModel<bool> { Success=false,Message="Failed to update notes"});

        }

        [Authorize]
        [HttpDelete]
        [Route("deletenotes")]
        public IActionResult Delete(int notesId)
        {
            bool result = manager.DeleteNotes(notesId);
            if (result)
            {
                return Ok(new ResponseModel<bool> { Success = true, Message = "Notes deleted successfully" });
            }
            return BadRequest(new ResponseModel<bool> { Success = false, Message = "Failed to delete notes" });

        }

    }
}
