using System;

namespace B3RAP_Leecher_v2.Utils
{
    public class Randomizer
    {
        public static string RandomUserAgent()
        {
            switch (MainForm._random.Next(6))
            {
                case 0:
                    {
                        return UpdatedUserAgents.OperaUserAgent();
                    }

                case 1:
                    {
                        return UpdatedUserAgents.ChromeUserAgent();
                    }

                case 2:
                    {
                        return UpdatedUserAgents.FirefoxUserAgent();
                    }

                case 3:
                    {
                        return UpdatedUserAgents.EdgeUserAgent();
                    }

                case 4:
                    {
                        return UpdatedUserAgents.BraveUserAgent();
                    }

                case 5:
                    {
                        return UpdatedUserAgents.ChromiumEdgeUserAgent();
                    }
            }
            return null;
        }

    }
}
