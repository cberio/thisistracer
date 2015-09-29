using System;
using System.Configuration;

namespace thisistracer.Util {
    public static class Configure {

        public static string GetAppConfigure(string arg) {
            if (!String.IsNullOrEmpty(arg) && !String.IsNullOrWhiteSpace(arg))
                return ConfigurationManager.AppSettings[arg];
            else
                throw new ArgumentNullException("Argument is empty or null");
        }
    }
}
