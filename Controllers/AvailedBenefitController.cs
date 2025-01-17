using System.Web.Http;
using BenefitPortalServices.Services;
using System.Collections.Generic;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/auth")]
    public class AvailedBenefitController : ApiController
    {
        private readonly AvailedBenefitService _service;

        public AvailedBenefitController()
        {
            _service = new AvailedBenefitService();
        }

        [HttpPost]
        [Route("AvailedBenefit")]
        public IHttpActionResult AvailedBenefit([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            string benefitIdString = payload.benefitId;

            // Convert username to int
            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid SAP Id format");
            }
            if (!int.TryParse(benefitIdString.ToString(), out int benefitId))
            {
                return BadRequest("Invalid benefit Id format");
            }

            List<Availedbenefit>availedbenefit = _service.AvailedBenefit(employeeId, benefitId);
            if (availedbenefit != null && availedbenefit.Count > 0)
            {
                return Ok(new { status = true, benefits = availedbenefit });
            }
            else
            {
                return Ok(new { status = false, message = "No benefits found or eligible for this employee." });
            }
        }
        [HttpPost]
        [Route("Avail")]
        public IHttpActionResult Avail([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            

            // Convert username to int
            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid SAP Id format");
            }
           
            List<Availedbenefit> availedbenefit = _service.Avail(employeeId);
            if (availedbenefit != null && availedbenefit.Count > 0)
            {
                return Ok(new { status = true, benefits = availedbenefit });
            }
            else
            {
                return Ok(new { status = false, message = "No benefits found or eligible for this employee." });
            }
        }
    }
}
