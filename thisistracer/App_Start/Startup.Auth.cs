﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using thisistracer.Models;
using thisistracer.Util;
using Microsoft.Owin.Security.Facebook;

namespace thisistracer {
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            
            //app.UseFacebookAuthentication(
            //   appId: Configure.GetAppConfigure("FaceBookAppId"),
            //   appSecret: Configure.GetAppConfigure("FaceBookAppSecret")
            //);

            // use Options
            var fb = new FacebookAuthenticationOptions();
            fb.Scope.Add("email");
            fb.Scope.Add("user_about_me");
            fb.Scope.Add("user_friends");
            //fb.Scope.Add("user_photos");
            fb.AppId = Util.Utils.GetAppConfigure("FaceBookAppId");
            fb.AppSecret = Util.Utils.GetAppConfigure("FaceBookAppSecret");
            fb.Provider = new FacebookAuthenticationProvider() {
                OnAuthenticated = async context => {
                    // Get the access token from FB and store it in the database and
                    // use Facebook C# SDK to get more information about the user
                    context.Identity.AddClaim(
                        new System.Security.Claims.Claim("FacebookAccessToekn", context.AccessToken)
                    );
                }
            };
            fb.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(fb);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions() {
                ClientId = Util.Utils.GetAppConfigure("GoogleClientId"),
                ClientSecret = Util.Utils.GetAppConfigure("GoogleClientSecret")
            });
        }
    }
}