using System.Windows.Forms;

namespace B3RAP_Leecher_v2.Utils
{
    public class ProgramUtils
    {
        public static void ShowErrorMessage(string msg, bool scraping)
        {
            if (scraping)
                MessageBox.Show(msg, "Uh-oh, looks like a scraping error happened...");
            else MessageBox.Show(msg, "Uh-oh, looks like a program error happened...");
        }
    }
}
