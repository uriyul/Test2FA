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
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Missing username or password");
                return null;
            }
            
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

            if(string.IsNullOrEmpty(code))
            {
                Console.WriteLine("Missing Authenticator code");
                return null;
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

            string authenticatorUri = $"otpauth://totp/{Uri.EscapeDataString("My2FaTestApp")}:{Uri.EscapeDataString(user.Email)}?secret={user.AuthenticatorKey}&issuer={Uri.EscapeDataString("My2FaTestApp")}&digits=6&algorithm=SHA1&period=30";
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
                AuthenticatorKey = "JBSWY3DPEHPK3PXP",
                //TwoFactorEnabled = true

            };

            var result = userManager.CreateAsync(user, "Password1!").Result;
        }
        #endregion
    }
}
