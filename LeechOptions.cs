using B3RAP_Leecher_v2.Properties;
using System.Windows.Forms;
using B3RAP_Leecher_v2.Utils;

namespace B3RAP_Leecher_v2
{
    public partial class LeechOptions : Form
    {
        public LeechOptions()
        {
            InitializeComponent();

            if (MainForm._leechOption == "emailpass")
                radioButton1.Checked = true;
            else if (MainForm._leechOption == "userpass")
                radioButton2.Checked = true;
            else if (MainForm._leechOption == "proxies")
                radioButton3.Checked = true;
            else if (MainForm._leechOption == "emailonly")
                radioButton4.Checked = true;
            else if (MainForm._leechOption == "custom")
            {
                radioButton5.Checked = true;
                textBox1.Text = MainForm._customRegex;
            }
        }

        private void LeechOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (radioButton1.Checked)
                MainForm._leechOption = "emailpass";
            if (radioButton2.Checked)
                MainForm._leechOption = "userpass";
            if (radioButton3.Checked)
                MainForm._leechOption = "proxies";
            if (radioButton4.Checked)
                MainForm._leechOption = "emailonly";
            if (radioButton5.Checked)
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                    ProgramUtils.ShowErrorMessage("Please insert a valid custom regex.", false);
                else
                {
                    MainForm._leechOption = "custom";
                    MainForm._customRegex = textBox1.Text;
                }
            }
        }
    }
}
