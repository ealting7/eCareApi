using eCareApi.Entities;
using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FaxPoolController : ControllerBase
    {
        private readonly IFaxPool _faxPoolInterface;

        public FaxPoolController(IFaxPool faxPoolInterface)
        {
            _faxPoolInterface = faxPoolInterface ?? throw new ArgumentNullException(nameof(faxPoolInterface));
        }

        [HttpGet("queues/")]
        public IActionResult GetFaxQueues()
        {
            var queues = _faxPoolInterface.GetFaxQueues();            

            if (queues == null)
            {
                return NotFound();
            }

            var returnQueues = new List<FaxPoolQueue>();

            foreach (var que in queues)
            {
                returnQueues.Add(new FaxPoolQueue
                {
                    QueueId = que.id,
                    QueueName = que.queue_name,
                    ListOrder = que.listorder,
                    ParentQueueId = que.parent_id
                });
            }


            return Ok(returnQueues);
        }

        [HttpGet("queues/{id}")]
        public IActionResult GetFaxPoolFaxes(int id)
        {
            var faxes = _faxPoolInterface.GetFaxPoolFaxes(id);            

            if (faxes == null)
            {
                return NotFound();
            }

            var returnFaxes = new List<Fax>();

            foreach (var fx in faxes)
            {
                returnFaxes.Add(new Fax
                {
                    FaxId = fx.FaxId,
                    CreateDate = fx.CreateDate,
                    displayCreatedDate = (fx.CreateDate.HasValue) ? fx.CreateDate.Value.ToShortDateString() + " " + fx.CreateDate.Value.ToShortTimeString() : "",
                    AssignedToUserId = fx.AssignedToUserId,
                    FaxImage = fx.FaxImage,
                    FaxQueueName = fx.FaxQueueName
                });
            }


            return Ok(returnFaxes);
        }

        [HttpGet("um/queues/{id}")]
        public IActionResult getUmFaxPoolFaxes(int id)
        {
            var faxes = _faxPoolInterface.getUmFaxPoolFaxes(id);

            if (faxes == null)
            {
                return NotFound();
            }

            var returnFaxes = new List<Fax>();

            foreach (var fx in faxes)
            {
                returnFaxes.Add(new Fax
                {
                    FaxId = fx.FaxId,
                    CreateDate = fx.CreateDate,
                    displayCreatedDate = (fx.CreateDate.HasValue) ? fx.CreateDate.Value.ToShortDateString() + " " + fx.CreateDate.Value.ToShortTimeString() : "",
                    AssignedToUserId = fx.AssignedToUserId,
                    FaxQueueName = fx.FaxQueueName
                });
            }


            return Ok(returnFaxes);
        }

        [HttpGet("{id}")]
        public IActionResult GetFax(int id)
        {
            var fax = _faxPoolInterface.GetFax(id);

            if (fax == null)
            {
                return NotFound();
            }

            var returnFax = new Fax();

            returnFax.FaxId = fax.id;
            returnFax.AssignedToUserId = fax.assigned_to_user_id;
            returnFax.FaxImage = fax.fax_image;
            returnFax.FaxImageBase64 = Convert.ToBase64String(fax.fax_image);
            returnFax.CreateDate = fax.fax_creationtime;

            return Ok(returnFax);
        }



        [HttpPost("dbms/update/faxpool/reffaxes")]
        public IActionResult updateFaxPoolUm(Fax faxpoolFax)
        {

            var referralFaxex = _faxPoolInterface.updateFaxPoolUm(faxpoolFax);

            if (referralFaxex == null)
            {
                return NoContent();
            }

            Utilization referral = new Utilization();
            referral.faxes = referralFaxex;

            return Ok(referral);
        }



        [HttpPost("dbms/upload/faxpool/reffaxes")]
        public IActionResult uploadFaxPoolUm(Fax faxpoolFax)
        {

            var referralFaxex = _faxPoolInterface.uploadFaxPoolUm(faxpoolFax);

            if (referralFaxex == null)
            {
                return NoContent();
            }

            Utilization referral = new Utilization();
            referral.faxes = referralFaxex;

            return Ok(referral);
        }


        [HttpPost("dbms/remove/faxpool/reffaxes")]
        public IActionResult removeFaxPoolUm(Fax faxpoolFax)
        {

            var referralFaxex = _faxPoolInterface.removeFaxPoolUm(faxpoolFax);

            if (referralFaxex == null)
            {
                return NoContent();
            }

            Utilization referral = new Utilization();
            referral.faxes = referralFaxex;

            return Ok(referral);
        }



        [HttpPatch("{id}")]
        public IActionResult UpdateAssignTo(int id, [FromBody]InboundFax updatedFax)
        {
            if (updatedFax != null)
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                InboundFax oldFax = new InboundFax();
                oldFax = _faxPoolInterface.getFaxForPatchAssignTo(id);

                if (oldFax != null && (oldFax.id == updatedFax.id))
                {
                    oldFax.assigned_by_user_id = updatedFax.assigned_by_user_id;
                    oldFax.assigned_to_user_date = updatedFax.assigned_to_user_date;
                    
                    //_faxPoolInterface.UpdateAssignTo(id, oldFax);

                    return Ok(oldFax);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
