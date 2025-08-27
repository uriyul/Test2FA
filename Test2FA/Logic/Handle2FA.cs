using Microsoft.AspNetCore.Identity;
using System;
using Test2FA.Model;


namespace Test2FA.Logic
{
    public class Handle2FA
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly SignInManager<ApplicationUser> signInManager;
        public Handle2FA(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;

            CreateDummyUser();
        }

        public ApplicationUser Login(string username, string password, string code)
        {
            var rc = signInManager.PasswordSignInAsync(username, password, false, false).Result;


            if (rc.IsNotAllowed)
            {
                Console.WriteLine("User is not allowed to login");
                return null;
            }

            var user = userManager.FindByNameAsync(username).Result;
            if (user == null)
            {
                Console.WriteLine("Failed to find user");
                return null;
            }

            if (rc.Succeeded && !rc.RequiresTwoFactor)
            {
                Console.WriteLine("Login successful, no 2FA needed");
                return user;
            }


            var result = signInManager.TwoFactorAuthenticatorSignInAsync(code, false, false).Result;

            if (result.Succeeded)
            {
                Console.WriteLine("Login successful with 2FA");
                return user;
            }

            return null;
        }

        public string Generate2FAQR(string username, string password)
        {
            var user = Login(username, password, "");
            if (user == null)
            {
                return null;
            }

            string authenticatorUri = $"otpauth://totp/{Uri.EscapeDataString("My2FaTestApp")}:{Uri.EscapeDataString(user.Email)}?secret={user.TwoFASecret}&issuer={Uri.EscapeDataString("My2FaTestApp")}&digits=6";
            user.TwoFactorEnabled = true;

            return authenticatorUri;
        }

        #region private methods

        private void CreateDummyUser()
        {
            var user = new ApplicationUser
            {
                UserName = "admin@My2FaTestApp.com",
                Email = "admin@My2FaTestApp.com",
                TwoFASecret = "JBSWY3DPE",
                TwoFactorEnabled = true

                #endregion
            };
            user.SetAuthenticatorKey("JBSWY3DPE");

            var result = userManager.CreateAsync(user, "Password1!").Result;
        }
    }
}
