using Microsoft.AspNetCore.Identity;
namespace Test2FA.Model
{

    public class ApplicationUser : IdentityUser
    {
        private string authenticatorKey;

        public void SetAuthenticatorKey(string key) => authenticatorKey = key;
        public string GetAuthenticatorKey() => authenticatorKey;
    }
}
