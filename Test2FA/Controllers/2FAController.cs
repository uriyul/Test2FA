using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test2FA.Logic;
using Test2FA.Model;

namespace Test2FA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFAController : ControllerBase
    {
        private readonly Handle2FA handle2FA;
        public TwoFAController(Handle2FA handle2FA)
        {
            this.handle2FA = handle2FA;
        }

        [HttpPost("login")]
        public IActionResult Get2FAStatus([FromBody] LoginModel model)
        {
            var rc = handle2FA.Login(model.Username, model.Password, model.TwoFACode);

            if (rc == null)
            {
                return Unauthorized(rc);
            }

            return Ok(rc);
        }

        /// <summary>
        /// This endpoint first authenticates the user using username and password.
        /// Then it creates a string for a QR code for the authenticator app
        /// Then it marks the user as a 2FA user, so he will not be able to sign in witout the authenticator code.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>A string for Authenticator QR code</returns>
        [HttpPost("2faqr")]
        public ActionResult<string> Generate2FAQR(string username, string password)
        {
            var qr = handle2FA.Generate2FAQR(username, password);

            if (qr == null)
            {
                return Unauthorized(qr);
            }

            return Ok(qr);
        }
    }
}
