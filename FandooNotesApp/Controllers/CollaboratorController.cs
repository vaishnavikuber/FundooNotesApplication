using CommonLayer.Models;
using System.Threading.Tasks;
using System;
using ManagerLayer.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net;
using FandooNotesApp.Helpers;
using Microsoft.Extensions.Logging;

namespace FandooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorManager manager;
        private readonly FundooDBContext context;
        private readonly IBus bus;
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<CollaboratorController> _logger;
        public CollaboratorController(ICollaboratorManager manager,FundooDBContext context, IBus bus, IDistributedCache distributedCache, ILogger<CollaboratorController> _logger)
        {
            this.manager = manager;
            this.context = context;
            this.bus = bus;
            this.distributedCache = distributedCache;
            this._logger = _logger;
        }

        [Authorize]
        [HttpPost]
        [Route("addcollaborator")]
        public async Task<IActionResult> AddCollaborator(AddCollaboratorModel model)
        {

            try
            {
                
                    string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                    string email = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
                    Collaborator addCollaborator = manager.AddCollaborator(model,Convert.ToInt32(userId));
                    SendCollaboratorMail send = new SendCollaboratorMail();
                    send.SendEmail(addCollaborator.Email, addCollaborator.NotesID,email);
                    Uri uri = new Uri("rabbitmq://localhost/FundooNotesEmailQueue");
                    var endPoint = await bus.GetSendEndpoint(uri);
                    await endPoint.Send(addCollaborator);
                    _logger.LogInformation(endPoint.ToString());    
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent successfully", Data = endPoint.ToString() });

                

            }
            catch (AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Email is not registered in application" });

            }
        }

        [HttpGet]
        [Route("redisgetallcollaborator")]
        public async Task<IActionResult> GetAllCollaboratorsUsingRedisCache()
        {
            var cacheKey = "CollaboratorsList";
            string serializedLabelList;
            var labelList = new List<Collaborator>();
            var redisLabelList = await distributedCache.GetAsync(cacheKey);
            if (redisLabelList != null)
            {
                serializedLabelList = Encoding.UTF8.GetString(redisLabelList);
                labelList = JsonConvert.DeserializeObject<List<Collaborator>>(serializedLabelList);
            }
            else
            {
                labelList = context.Collaborators.ToList();
                serializedLabelList = JsonConvert.SerializeObject(labelList);
                redisLabelList = Encoding.UTF8.GetBytes(serializedLabelList);
                var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                await distributedCache.SetAsync(cacheKey, redisLabelList, options);
            }
            return Ok(labelList);
        }

        [Authorize]
        [HttpGet("getcollaborators")]
        public IActionResult GetCollaborators()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.GetAllCollaborators(Convert.ToInt32(userId));
                _logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<List<Collaborator>> { Success = true, Message = "fetch successfull", Data = result });
            }
            catch (AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<List<Collaborator>> { Success = false, Message = "failed to fetch" });
            }

        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteCollaborators(int notesId, string email)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var result = manager.DeleteCollaborator(email, notesId, Convert.ToInt32(userId));
                _logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "delete successfull", Data = result });

            }
            catch (AppException ex)
            {
               _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to fetch" });

            }




        }

    }
}
