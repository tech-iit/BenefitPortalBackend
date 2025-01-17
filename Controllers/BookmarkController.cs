using System.Web.Http;
using BenefitPortalServices.Services;
using System;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/auth")]
    public class BookmarkController : ApiController
    {
        private readonly BookmarkService _service;

        public BookmarkController()
        {
            _service = new BookmarkService();
        }

        [HttpPost]
        [Route("Bookmark")]
        public IHttpActionResult Bookmark([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            string benefitIdString = payload.benefitId;

            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid employee id format");
            }
            if (!int.TryParse(benefitIdString.ToString(), out int benefitId))
            {
                return BadRequest("Invalid benefit id format");
            }

            try
            {
                bool status = _service.Bookmark(employeeId, benefitId);
                return Ok(new { status, message = status ? "Bookmarked" : "Removed from bookmark" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

