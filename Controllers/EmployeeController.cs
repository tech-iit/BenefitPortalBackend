using System.Web.Http;
using BenefitPortalServices.Services;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/auth")]
    public class EmployeeController : ApiController
    {
        private readonly EmployeeService _service;
        public EmployeeController()
        {
            _service = new EmployeeService();
        }

        [HttpPost]
        [Route("AddEmployee")]
        public IHttpActionResult AddEmployee([FromBody] dynamic payload)
        {
            string employeeIdString = payload.employeeId;
            string password = payload.password;
            string name = payload.name;
            string emailId = payload.emailId;
            string contactNo = payload.contactNo;
            string band = payload.band;
            if (!int.TryParse(employeeIdString.ToString(), out int employeeId))
            {
                return BadRequest("Invalid employee id format");
            }
            bool status = _service.AddEmployee(employeeId, password, name, emailId,contactNo,band);
            if (status == true)
                return Ok(new { status = true });
            else if (status == false)
                return Ok(new { status = false });
            return BadRequest("Error uploading ur employee details");
        }
    }
}