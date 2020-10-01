using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace B3RAP_Leecher_v2
{
    public partial class MoreOptions : Form
    {
        public MoreOptions()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void MoreOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox1.Checked)
            {
                MainForm._useProxies = true;
                MainForm._proxyType = comboBox1.SelectedItem.ToString();
            }
            else
            {
                MainForm._useProxies = false;
                MainForm._proxyType = string.Empty;
            }

            if (checkBox2.Checked)
            {
                MainForm._useRetries = true;
                MainForm._retries = (int)numericUpDown1.Value;
            }
            else
            {
                MainForm._useRetries = false;
                MainForm._retries = 1;
            }

            if (checkBox7.Checked)
                MainForm._past24Hours = true;
            else MainForm._past24Hours = false;

            MainForm._timeout = (int)numericUpDown2.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            MainForm._proxies = File.ReadAllLines(openFileDialog1.FileName);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Enabled = true;
                comboBox1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                comboBox1.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                numericUpDown1.Enabled = true;
            else numericUpDown1.Enabled = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                MainForm._showScrapingErrors = true;
            else MainForm._showScrapingErrors = false;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                MainForm._showProgramErrors = true;
            else MainForm._showProgramErrors = false;
        }

        private void MoreOptions_Load(object sender, EventArgs e)
        {
            if (MainForm._useProxies)
                checkBox1.Checked = true;
            else checkBox1.Checked = false;

            if (MainForm._useRetries)
                checkBox2.Checked = true;
            else checkBox2.Checked = false;

            if (MainForm._showScrapingErrors)
                checkBox4.Checked = true;
            else checkBox4.Checked = false;

            if (MainForm._showProgramErrors)
                checkBox5.Checked = true;
            else checkBox5.Checked = false;

            if (MainForm._useCustomLinks)
                checkBox6.Checked = true;
            else checkBox6.Checked = false;

            if (MainForm._past24Hours)
                checkBox7.Checked = true;
            else checkBox7.Checked = false;

            numericUpDown2.Value = MainForm._timeout;
            numericUpDown1.Value = MainForm._retries;

            if (MainForm._proxyType == "HTTP") comboBox1.SelectedIndex = 0;
            else if (MainForm._proxyType == "SOCKS4") comboBox1.SelectedIndex = 1;
            else if (MainForm._proxyType == "SOCKS5") comboBox1.SelectedIndex = 2;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            MainForm._customLinks = File.ReadAllLines(openFileDialog2.FileName);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                MainForm._useCustomLinks = true;
                button2.Enabled = true;
            }
            else
            {
                MainForm._useCustomLinks = false;
                button2.Enabled = false;
                if (MainForm._customLinks.Length != 0) MainForm._customLinks = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog2.FileName = string.Empty;
            openFileDialog2.ShowDialog();
        }
    }
}
