using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.Models;
using FandooNotesApp.Helpers;
using GreenPipes.Caching;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<NotesController> logger;
        private readonly IDistributedCache distributedCache;

        public NotesController(INotesManager manager, FundooDBContext context, ILogger<NotesController> logger, IDistributedCache distributedCache)
        {
            this.manager = manager;
            this.context = context;
            this.logger = logger;
            this.distributedCache = distributedCache;
        }

        [Authorize]
        [HttpPost]
        [Route("addnotes")]
        public IActionResult AddNotes(AddNotesModel model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var notes = manager.AddNotes(model, Convert.ToInt32(userId));
                
                logger.LogInformation(notes.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Notes added successfully", Data = notes });
                
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "Failed to add notes" });

            }

        }

        [Authorize]
        [HttpGet]
        [Route("getallnotes")]
        public IActionResult GetAllNotes()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var notes = manager.GetAllNotes(Convert.ToInt32(userId));
                if (notes != null)
                {
                    logger.LogInformation(notes.ToString());
                    return Ok(new ResponseModel<List<Notes>> { Success = true, Message = "Notes  are :", Data = notes });
                }
                return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "Failed to get notes" });


            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "Failed to get notes" });

            }
        }
       
        
        [Authorize]
        [HttpPut]
        [Route("editnotes")]
        
        public IActionResult UpdateNotes(int notesId,EditNotesModel model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                bool result = manager.EditNotes(model, notesId, Convert.ToInt32(userId));
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Notes updated successfully" });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Failed to update notes" });
            }

        }

        [Authorize]
        [HttpDelete]
        [Route("deletenotes")]
        public IActionResult Delete(int notesId)
        {
            try
            {
                bool result = manager.DeleteNotes(notesId);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Notes deleted successfully" });
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Failed to delete notes" });
            }

        }

        [Authorize]
        [HttpPut("pinunpin")]
        public IActionResult Pin(int notesId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.PinUnPin(Convert.ToInt32(userId), notesId);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "successfull", Data = result });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "un-success" });
            }
        }

        [Authorize]
        [HttpPut("trashuntrash")]
        public IActionResult Trash(int notesId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.TrashUnTrash(Convert.ToInt32(userId), notesId);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "successfull", Data = result });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "un-success" });
            }
        }

        [Authorize]
        [HttpPut("archiveunarchive")]
        public IActionResult Archive(int notesId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.Archive(Convert.ToInt32(userId), notesId);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "successfull", Data = result });
            }
           catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "un-success" });

            }

        }

        [Authorize]
        [HttpPut("deleteforever")]
        public IActionResult DeleteForever(int notesId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                bool result = manager.DeleteForever(Convert.ToInt32(userId), notesId);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "delete successful", Data = result });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to delete" });
            }

        }

        [Authorize]
        [HttpPut("image")]
        public IActionResult Image(int notesId, IFormFile path)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.Image(Convert.ToInt32(userId), notesId, path);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "image insert successful", Data = result });

            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "failed to insert image" });
            }

        }

        [Authorize]
        [HttpPut("reminder")]
        public IActionResult Remainder(int notesId,DateTime datetime)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.Remainder(Convert.ToInt32(userId), notesId, datetime);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "remainder set successful", Data = result });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "failed to set remainder" });
            }

        }

        [Authorize]
        [HttpPut("color")]
        public IActionResult Color(int notesId,string color)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.Color(Convert.ToInt32(userId), notesId, color);
                logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "color set successful", Data = result });
            }
            catch(AppException ex)
            {
                logger.LogError(ex.Message);
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "failed to set color" });
            }
            

        }


        [HttpGet]
        [Route("redisgetallnotes")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string serializedNotesList;
            var notesList = new List<Notes>();
            var redisNotesList=await distributedCache.GetAsync(cacheKey);
            if (redisNotesList != null)
            {
                serializedNotesList = Encoding.UTF8.GetString(redisNotesList);
                notesList=JsonConvert.DeserializeObject<List<Notes>>(serializedNotesList);
            }
            else
            {
                notesList=context.Notes.ToList();
                serializedNotesList = JsonConvert.SerializeObject(notesList);
                redisNotesList=Encoding.UTF8.GetBytes(serializedNotesList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                await distributedCache.SetAsync(cacheKey, redisNotesList, options);
            }
            return Ok(notesList);
        }











        //review
        //[HttpGet("titleanddiscription")]
        //public IActionResult GetNotesByTitleAndDiscription(string title, string discription)
        //{
        //    var result=context.Notes.Select(x=>x).Where(x=>x.Title==title && x.Description==discription);
        //    if (result != null)
        //    {
        //        return Ok(new ResponseModel<List<Notes>> { Success = true, Message = "fetch successfull", Data = result.ToList() });
        //    }
        //    return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "Failed to fetch notes" });

        //}

        //[HttpGet("countofNotes")]
        //public IActionResult CountNotesOfUser(int userId)
        //{
        //    int count= context.Notes.Where(x => x.UserID == userId).Count();
        //    if (count != 0)
        //    {
        //        return Ok(new ResponseModel<int> { Success = true, Message = "fetch successfull", Data = count });

        //    }
        //    return BadRequest(new ResponseModel<int> { Success = false, Message = "Failed to fetch notes" });

        //}


    }
}
