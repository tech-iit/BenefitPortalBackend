using System.Web.Http;
using BenefitPortalServices.Services;

namespace BenefitPortalServices.Controllers
{
    
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly LoginAuthorizationService _service;

        public AuthController()
        {
            _service = new LoginAuthorizationService();
        }

        [HttpPost]
        [Route("Authorize")]
        public IHttpActionResult Authorize([FromBody] dynamic payload)
        {
            string usernameString = payload.username;
            string password = payload.password;

            // Convert username to int
            if (!int.TryParse(usernameString.ToString(), out int username))
            {
                return BadRequest("Invalid username format");
            }

            var (userType,name,emailId) = _service.Authorize(username, password);
            if (userType == "Employee")
                return Ok(new { role = "Employee", name = name, emailId = emailId });
            else if (userType == "Admin")
                return Ok(new { role = "Admin", name = name, emailId = emailId });
            else if (userType == "Invalid")
            {
                return BadRequest("usertypeinvalid");
            }
            return BadRequest("Invalid username and password");
        }
        
       

    }

}
