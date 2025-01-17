using System.Web.Http;
using BenefitPortalServices.Services;

namespace BenefitPortalServices.Controllers
{
    [RoutePrefix("api/sub")]
    public class SubscribeController : ApiController
    {
        private readonly SubscribeService _service;

        public SubscribeController()
        {
            _service = new SubscribeService();
        }

        [HttpPost]
        [Route("Subscribe")]
        public IHttpActionResult Subscribe([FromBody] dynamic payload)
        {
            string emailIdString = payload.emailId;
            var x = _service.Subscribe(emailIdString);
            if (x)
            {
                return Ok(new {status=true});
            }
            return BadRequest("error subscribing this email");
        }

        [HttpPost]
        [Route("CheckSubscription")]
        public IHttpActionResult CheckSubscription([FromBody] dynamic payload)
        {
            string emailId = payload.emailId;
            var x = _service.CheckSubscription(emailId);
            if (x)
            {
                return Ok(new { check = true });
            }else if (!x)
            {
                return Ok(new {check=false});
            }
            return BadRequest("error fetching status");
        }

        [HttpPost]
        [Route("UnSubscribe")]
        public IHttpActionResult UnSubscribe([FromBody] dynamic payload)
        {
            string emailIdString = payload.emailId;
            var x = _service.UnSubscribe(emailIdString);
            if (x)
            {
                return Ok(new { status = true });
            }
            return BadRequest("error subscribing this email");
        }
    }
}
