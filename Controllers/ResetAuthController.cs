using System;
using System.Web.Http;
using BenefitPortalServices.Services;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/auth")]
    public class ResetAuthController : ApiController
    {
        private readonly ResetService _authService;

        public ResetAuthController()
        {
            _authService = new ResetService();
        }

        [HttpPost]
        [Route("send-otp")]
        public IHttpActionResult SendOtp([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid username format");
            }
            bool success = _authService.SendOtp(employeeId);
            if (!success)
                return BadRequest("Failed to send OTP. Check employee ID.");
            return Ok("OTP sent successfully.");
        }

        [HttpPost]
        [Route("verify-otp")]
        public IHttpActionResult VerifyOtp([FromBody] dynamic payload)
        {
            try
            {
                if (payload == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                string employeeIdString = payload.employeeId;
                string otpCodeString = payload.otpCode;

                if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
                {
                    return BadRequest("Invalid employee ID format");
                }

                if (!int.TryParse(otpCodeString.ToString(), out int otpCode))
                {
                    return BadRequest("Invalid OTP format");
                }

                bool isValid = _authService.VerifyOtp(employeeId, otpCode);

                if (!isValid)
                {
                   
                    return BadRequest("Invalid otp");
                }

                return Ok("OTP verified successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("reset-password")]
        public IHttpActionResult ResetPassword([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            string newPassword = payload.newPassword;
            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid username format");
            }
            bool success = _authService.ResetPassword(employeeId, newPassword);
            if (!success)
                return BadRequest("Failed to reset password.");
            return Ok("Password reset successfully.");
        }
    }
}


