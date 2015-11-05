using Microsoft.AspNet.Identity;
using System;
using System.Configuration;

namespace thisistracer.Util {
    public static class Utils {

        public static string GetAppConfigure(string arg) {
            if (!String.IsNullOrEmpty(arg) && !String.IsNullOrWhiteSpace(arg))
                return ConfigurationManager.AppSettings[arg];
            else
                throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name + "Argument is empty or null");
        }

        public static string GetUniqueFileName(System.Security.Principal.IPrincipal User, string fileName) {
            if (User == null)
                throw new ArgumentException(System.Reflection.MethodBase.GetCurrentMethod().Name + "User Argumnet is null");

            return string.Format(User.Identity.GetUserId() + "/image_{0}{1}",
                DateTime.Now.ToString("yyyyMMddhhmmssms"), System.IO.Path.GetExtension(fileName));
        }
    }
}
