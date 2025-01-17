using System;
using System.Web.Http;
using BenefitPortalServices.Services;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/feedback")]
    public class FeedbackController : ApiController
    {
        private readonly FeedbackService _service;

        public FeedbackController()
        {
            _service = new FeedbackService();
        }

        [HttpPost]
        [Route("SubmitFeedback")]
        public IHttpActionResult SubmitFeedback([FromBody] Feedback feedback)
        {
            if (feedback == null || feedback.BenefitId <= 0 || feedback.EmployeeId <= 0 || string.IsNullOrEmpty(feedback.FeedbackText))
            {
                return BadRequest("Invalid feedback data.");
            }

            try
            {
                bool isBenefitExists = _service.CheckBenefitExists(feedback.BenefitId);
                if (!isBenefitExists)
                {
                    return BadRequest("Benefit does not exist.");
                }
                bool isEmployeeExists = _service.CheckEmployeeExists(feedback.EmployeeId);
                if (!isEmployeeExists)
                {
                    return BadRequest("Employee does not exist.");
                }

                var result = _service.InsertFeedback(feedback);
                if (result.Item2 == "1")
                {
                    return Ok("Feedback submitted successfully.");
                }
                else if (result.Item2 == "2")
                {
                    return BadRequest("Employee's band does not meet the eligibility criteria for this benefit.");
                }
                else
                {
                    return InternalServerError(new Exception(result.Item2));  
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error occurred: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }
        }
    }
}
