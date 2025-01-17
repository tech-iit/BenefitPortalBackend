using System.Web.Http;
using BenefitPortalServices.Services;
using BenefitPortalServices.Models;
using System;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/ssd")]
    public class SSDController : ApiController
    {
        private readonly SSDService _service;

        public SSDController()
        {
            _service = new SSDService();
        }

        [HttpPost]
        [Route("RaiseSSD")]
        public IHttpActionResult RaiseSSD([FromBody] SSD ssd)
        {
            if (_service.RaiseSSD(ssd))
            {
                return Ok("SSD request raised successfully.");
            }
            return BadRequest("Failed to raise SSD request.");
        }

        [HttpPost]
        [Route("GetSSDHistoryByEmployeeId")]
        public IHttpActionResult GetSSDHistoryByEmployeeId([FromBody] dynamic payload)
        {
            string employeeIdString = payload.EmployeeId;

            if (!int.TryParse(employeeIdString.ToString(), out int EmployeeId))
            {
                return BadRequest("Invalid username format");
            }
            var ssdHistory = _service.GetSSDHistoryByEmployeeId(EmployeeId);
            if (ssdHistory != null && ssdHistory.Count > 0)
            {
                return Ok(ssdHistory);
            }
            return BadRequest("No SSD history found for the given employee.");
        }



        [HttpPost]
        [Route("GetAllSSDsForAdmin")]
        public IHttpActionResult GetAllSSDsForAdmin()
        {
            try
            {
                var allSsdRequests = _service.GetAllSsdRequests();
                if (allSsdRequests != null && allSsdRequests.Count > 0)
                {
                    return Ok(allSsdRequests);
                }
                return BadRequest("No SSD requests found.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // API to Update SSD Status (For Admin to update status)
        [HttpPost]
        [Route("UpdateSSDStatus")]
        public IHttpActionResult UpdateSSDStatus([FromBody] SSD ssdRequest)
        {
            if (ssdRequest == null || ssdRequest.SSDId <= 0 || string.IsNullOrEmpty(ssdRequest.Status))
            {
                return BadRequest("Invalid SSD or status.");
            }

            try
            {
                var result = _service.UpdateSsdStatus(ssdRequest.SSDId, ssdRequest.Status);
                if (result)
                {
                    return Ok("SSD status updated successfully.");
                }
                return BadRequest("Failed to update SSD status.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
