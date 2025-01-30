using System.Web.Http;
using BenefitPortalServices.Services;
using System.Collections.Generic;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/auth")]
    public class BenefitController : ApiController
    {
        private readonly UploadBenefit _service;
        public BenefitController()
        {
            _service = new UploadBenefit();
        }

        [HttpPost]
        [Route("AddBenefit")]
        public IHttpActionResult AddBenefit([FromBody] dynamic payload)
        {
            string adminIdString = payload.adminId;
            string benefitname = payload.name;
            string description = payload.description;
            string imagepath = payload.imagepath;
            string category = payload.category.ToString().ToLower();
            string eligibility = payload.eligibility;
            if (!int.TryParse(adminIdString.ToString(), out int adminId))
            {
                return BadRequest("Invalid admin id format");
            }
            bool status = _service.AddBenefit(adminId, benefitname, description, imagepath, category, eligibility);
            if (status == true)
                return Ok(new { status = true });
            else if (status == false)
                return Ok(new { status = false });
            return BadRequest("Error uploading ur benefit");
        }


        [HttpPost]
        [Route("ViewBenefit")]
        public IHttpActionResult ViewBenefit([FromBody] dynamic payload)
        {
            string employeeString = payload.employeeId;

            if (!int.TryParse(employeeString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid employee id format");
            }

            List<Benefit> benefits = _service.ViewBenefits(employeeId);

            if (benefits != null && benefits.Count > 0)
            {
                return Ok(new { status = true, benefits = benefits });
            }
            else
            {
                return Ok(new { status = false, message = "No benefits found or eligible for this employee." });
            }
        }

        [HttpPost]
        [Route("DeleteBenefit")]
        public IHttpActionResult DeleteBenefit([FromBody] dynamic payload)
        {
            string BenefitIdstring = payload.BenefitId;

            if (!int.TryParse(BenefitIdstring.ToString(), out int BenefitId))
            {
                return BadRequest("Invalid employee id format");
            }

            var status = _service.DeleteBenefit(BenefitId);

            if (status)
            {
                return Ok(new { status = true, message = "Benefit Deleted" });
            }
            else
            {
                return Ok(new { status = false, message = "Benefit Not deleted" });
            }
        }



    }
}
