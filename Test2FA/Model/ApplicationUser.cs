using Microsoft.AspNetCore.Identity;
namespace Test2FA.Model
{

    public class ApplicationUser : IdentityUser
    {
        public string TwoFASecret { get; set; }

        private string _authenticatorKey;

        public void SetAuthenticatorKey(string key) => _authenticatorKey = key;
        public string GetAuthenticatorKey() => _authenticatorKey;
    }
}
