namespace B3RAP_Leecher_v2.Utils
{
    public class UpdatedUserAgents
    {
        private static readonly string[] UserAgents = new[] { "Mozilla/5.0 ({0}; WOW64; Trident/{1}; rv:{2}) like Gecko", "Mozilla/5.0 ({0}; {1}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{2} Safari/537.36 OPR/{3}", "Mozilla/5.0 ({0}; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36", "Mozilla/5.0 ({0}; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0", "Mozilla/5.0 ({0}; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362", "Mozilla/5.0 ({0}; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Brave Chrome/76.0.3809.132 Safari/537.36", "Mozilla/5.0 ({0}; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.34 Safari/537.36 Edg/78.0.276.11", "Mozilla/5.0 (Linux; U; {0}; SM-J710F Build/M1AJQ; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/77.0.3865.73 Mobile Safari/537.36 OPR/44.1.2254.143214" };

        public static string OperaUserAgent()
        {
            string windowsVersion = RandomWindowsVersion();
            string chromeVersion = string.Empty;
            string operaVersion = string.Empty;
            string systemType;

            if (windowsVersion.Contains("NT 5.1") || windowsVersion.Contains("NT 6.0"))
            {
                chromeVersion = "49.0.2623.112";
                operaVersion = "36.0.2130.80";
                systemType = "WOW64";
            }
            else
            {
                systemType = "Win64; x64";

                switch (MainForm._random.Next(2))
                {
                    case 0:
                        {
                            chromeVersion = "76.0.3809.132";
                            operaVersion = "63.0.3368.71";
                            break;
                        }

                    case 1:
                        {
                            chromeVersion = "76.0.3809.132";
                            operaVersion = "63.0.3368.54789";
                            break;
                        }
                }
            }

            return string.Format(UserAgents[1], windowsVersion, systemType, chromeVersion, operaVersion);
        }

        public static string ChromeUserAgent()
        {
            return string.Format(UserAgents[2], RandomWindowsVersion());
        }

        public static string FirefoxUserAgent()
        {
            return string.Format(UserAgents[3], RandomWindowsVersion());
        }

        public static string EdgeUserAgent()
        {
            return string.Format(UserAgents[4], RandomWindowsVersion());
        }

        public static string BraveUserAgent()
        {
            return string.Format(UserAgents[5], RandomWindowsVersion());
        }

        public static string ChromiumEdgeUserAgent()
        {
            return string.Format(UserAgents[6], RandomWindowsVersion());
        }

        private static string RandomWindowsVersion()
        {
            string windowsVersion = "Windows NT ";

            switch (MainForm._random.Next(4))
            {
                case 0:
                    {
                        windowsVersion += "6.1";
                        break;
                    }

                case 1:
                    {
                        windowsVersion += "6.2";
                        break;
                    }

                case 2:
                    {
                        windowsVersion += "6.3";
                        break;
                    }

                case 3:
                    {
                        windowsVersion += "10.0";
                        break;
                    }
            }

            return windowsVersion;
        }
    }

}
