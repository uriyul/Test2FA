using Microsoft.AspNetCore.Identity;
namespace Test2FA.Model
{

    public class ApplicationUser : IdentityUser
    {
        public string AuthenticatorKey { set; get; }
    }
}
