using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.Models;
using FandooNotesApp.Helpers;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

namespace FandooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelManager manager;
        private readonly IDistributedCache distributedCache;
        private readonly FundooDBContext context;
        private readonly ILogger<LabelController> _logger;
        public LabelController(ILabelManager manager, FundooDBContext context, IDistributedCache distributedCache, ILogger<LabelController> _logger)
        {
            this.manager = manager;
            this.context = context;
            this.distributedCache = distributedCache;
            this._logger = _logger;
        }

        [Authorize]
        [HttpPost]
        [Route("addlabel")]
        public IActionResult AddLabel(LabelModel model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                var label = manager.AddLabel(model, Convert.ToInt32(userId));
                _logger.LogInformation(label.ToString());
                return Ok(new ResponseModel<Label> { Success = true, Message = "label added successfully", Data = label });
            }
            catch(AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Label> { Success = false, Message = "failed to add label" });
            }
           
           
        }

        [Authorize]
        [HttpGet("getalllabels")]
        public IActionResult GetLabels()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                List<Label> labels = manager.GetAllLabels(Convert.ToInt32(userId));
                _logger.LogInformation(labels.ToString());
                return Ok(new ResponseModel<List<Label>> { Success = true, Message = "fatch successful", Data = labels });
            }
            catch (AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<List<Label>> { Success = true, Message = "failed to fetch" });
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult Delete(int labelId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                bool result = manager.DeleteLabel(Convert.ToInt32(userId), labelId);
                _logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Deleted successful", Data = result });

            }
            catch (AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to delete" });
            }
        }

        [Authorize]
        [HttpDelete("updatelabel")]
        public IActionResult Update(int labelId,string labelName)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
                bool result = manager.UpdateLabel(Convert.ToInt32(userId), labelId, labelName);
                _logger.LogInformation(result.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Updated successful", Data = result });
            }
            catch (AppException ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "failed to Update" });
            }
        }

        [HttpGet]
        [Route("redisgetallusers")]
        public async Task<IActionResult> GetAllUsersUsingRedisCache()
        {
            var cacheKey = "UsersList";
            string serializedLabelList;
            var labelList = new List<Label>();
            var redisLabelList = await distributedCache.GetAsync(cacheKey);
            if (redisLabelList != null)
            {
                serializedLabelList = Encoding.UTF8.GetString(redisLabelList);
                labelList = JsonConvert.DeserializeObject<List<Label>>(serializedLabelList);
            }
            else
            {
                labelList = context.Labels.ToList();
                serializedLabelList = JsonConvert.SerializeObject(labelList);
                redisLabelList = Encoding.UTF8.GetBytes(serializedLabelList);
                var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                await distributedCache.SetAsync(cacheKey, redisLabelList, options);
            }
            return Ok(labelList);
        }

    }
}
