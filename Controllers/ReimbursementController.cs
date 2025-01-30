using System.Web.Http;
using BenefitPortalServices.Services;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/reimbursement")]
    public class ReimbursementController : ApiController
    {
        private readonly ReimbursementService _service;

        public ReimbursementController()
        {
            _service = new ReimbursementService();
        }

        [HttpPost]
        [Route("AddReimbursement")]
        public IHttpActionResult AddReimbursement([FromBody] Reimbursement reimbursement)
        {
            if (_service.AddReimbursement(reimbursement))
            {
                return Ok("Reimbursement request submitted successfully.");
            }
            return BadRequest("Failed to submit reimbursement request.");
        }

        [HttpGet]
        [Route("GetReimbursements/{employeeId}")]
        public IHttpActionResult GetReimbursements(int employeeId)
        {
            var reimbursements = _service.GetReimbursementsByEmployeeId(employeeId);
            if (reimbursements != null && reimbursements.Count > 0)
            {
                return Ok(reimbursements);
            }
            //if (reimbursements.Count > 0)
            //{
            //    return BadRequest("no bill found");
            //}
            return BadRequest("no bill found");
            //return NotFound();
        }
    }
}
