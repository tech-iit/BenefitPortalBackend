using System.Web.Http;
using BenefitPortalServices.Services;


namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/admin/reimbursement")]
    public class AdminReimbursementController : ApiController
    {
        private readonly AdminReimbursementService _service;

        public AdminReimbursementController()
        {
            _service = new AdminReimbursementService();
        }

        // API to get all reimbursements
        [HttpGet]
        [Route("GetAllReimbursements")]
        public IHttpActionResult GetAllReimbursements()
        {
            var reimbursements = _service.GetAllReimbursements();
            if (reimbursements != null && reimbursements.Count > 0)
            {
                return Ok(reimbursements);
            }
            return NotFound();
        }

        // API to update reimbursement status by EmployeeId and ReimbursementId
        [HttpPost]
        [Route("UpdateReimbursementStatus")]
        public IHttpActionResult UpdateReimbursementStatus([FromBody] dynamic payload)
        {
            string reimbursementIdString=payload.reimbursementId;
            string status=payload.status;
            if (!int.TryParse(reimbursementIdString.ToString(), out int reimbursementId))
            {
                return BadRequest("Invalid sap id format");
            }
            bool result = _service.UpdateReimbursementStatus(reimbursementId,status);
            if (result)
            {
                return Ok("Reimbursement status updated successfully.");
            }
            return BadRequest("Failed to update reimbursement status.");
        }
    }
}
