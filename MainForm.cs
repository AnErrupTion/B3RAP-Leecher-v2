using B3RAP_Leecher_v2.Utils;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace B3RAP_Leecher_v2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            _worker.WorkerSupportsCancellation = true;
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Text += $" v{Application.ProductVersion}";
            label2.Text = textBox2.Lines.Length.ToString();
            label3.Text = textBox3.Lines.Length.ToString();
            label5.Text = textBox4.Lines.Length.ToString();
            _links.MaxLength = int.MaxValue;
            _links.Multiline = true;
            _links.ReadOnly = true;
        }

        private int _programErrors;
        private int _scraperErrors;

        public static int _retries = 3;
        private int _currentRetry = 1;

        public static string _leechOption = "emailpass";
        public static string _customRegex;

        public static Random _random = new Random();

        public static bool _stop;
        public static bool _useProxies;
        public static bool _useRetries = true;
        public static bool _showScrapingErrors;
        public static bool _showProgramErrors;
        public static bool _useCustomLinks;
        public static bool _past24Hours;

        public static int _timeout = 5000;

        public static string _proxyType;

        private readonly TextBox _links = new TextBox();

        private static AbortableWorker _worker = new AbortableWorker();

        public static string[] _proxies;
        public static string[] _customLinks;

        private void button1_Click(object sender, EventArgs e)
        {
            SetGuiElements(false);
            SetInformations(true);
            try
            {
                _worker.DoWork += new DoWorkEventHandler(GetLinks);
                _worker.RunWorkerCompleted += (object send, RunWorkerCompletedEventArgs ev) =>
                {
                    SetGuiElements(true);
                    SetInformations(false);
                    button2.Enabled = false;
                };
                _worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                if (_showProgramErrors) ProgramUtils.ShowErrorMessage(ex.Message + ex.StackTrace, false);
                label13.Text = (++_programErrors).ToString();
            }
        }

        private void GetLinks(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (_useCustomLinks) ScrapeResult(_customLinks, null);
                else
                {
                    foreach (string engine in textBox3.Lines)
                        foreach (string website in textBox4.Lines)
                            foreach (string keyword in textBox2.Lines)
                            {
                                _currentRetry = 1;
                                Scrape(engine, website, keyword, _currentRetry);
                            }
                }
            }
            catch (Exception ex)
            {
                if (_showScrapingErrors) ProgramUtils.ShowErrorMessage(ex.Message + ex.StackTrace, true);
                label13.Text = (++_scraperErrors).ToString();
            }
        }

        private ProxyClient RandomProxy()
        {
            string proxy = _proxies[_random.Next(_proxies.Length)];

            ProxyType type = ProxyType.HTTP;
            if (_proxyType.ToLower() == "http") type = ProxyType.HTTP;
            else if (_proxyType.ToLower() == "socks4") type = ProxyType.Socks4;
            else if (_proxyType.ToLower() == "socks5") type = ProxyType.Socks5;

            bool result = ProxyClient.TryParse(type, proxy, out ProxyClient client);
            if (result) return client; else return null;
        }

        private void Scrape(string engine, string website, string keyword, int retry)
        {
            if (_past24Hours)
            {
                label18.Text = "Adjusting search engines...";
                if (engine.Contains("bing")) engine = "https://www.bing.com/search?filters=ex1%3a%22ez1%22&q=";
                else if (engine.Contains("yahoo")) engine = "https://search.yahoo.com/search?age=1d&btf=d&q=";
                else if (engine.Contains("yandex")) engine = "https://yandex.com/search/?within=77&text=";
                else if (engine.Contains("google")) engine = "https://www.google.com/search?tbs=qdr:d&q=";
                else if (engine.Contains("duckduckgo")) engine = "https://duckduckgo.com/?df=d&ia=web&q=";
                else if (engine.Contains("aol")) engine = "https://search.aol.com/aol/search?age=1d&btf=d&q=";
                else if (engine.Contains("rambler")) engine = "https://nova.rambler.ru/search?period=day&query=";
            }

            again: try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    req.UserAgent = Utils.Randomizer.RandomUserAgent();
                    if (_useProxies)
                    {
                        ProxyClient client = RandomProxy();
                        if (client != null) req.Proxy = client; else goto again;
                    }
                    req.EnableEncodingContent = true;
                    req.IgnoreInvalidCookie = true;
                    req.IgnoreProtocolErrors = true;
                    if (_useRetries)
                    {
                        req.Reconnect = true;
                        req.ReconnectDelay = _timeout;
                        req.ReconnectLimit = _retries;
                    }
                    req.SslProtocols = SslProtocols.Tls12;
                    req.SslCertificateValidatorCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    req.UseCookies = true;
                    req.ConnectTimeout = _timeout;
                    req.ReadWriteTimeout = _timeout;
                    req.Cookies = new CookieStorage();
                    req.AllowAutoRedirect = true;
                    req.MaximumAutomaticRedirections = 10;

                    req.AddHeader("Upgrade-Insecure-Requests", "1");
                    req.AddHeader("Accept", "*/*");

                    label16.Text = keyword;
                    label15.Text = website;
                    label18.Text = "Scraping links...";
                    label19.Text = retry.ToString();
                    label21.Text = engine;

                    string response = req.Get($"{engine}{keyword}+site:{website}").ToString();
                    MatchCollection regex = Regex.Matches(response, $@"(https:\/\/{website}\/\w+)");
                    if (regex.Count != 0)
                    {
                        string[] arr = regex.OfType<Match>().Select(m => m.Value).ToArray();
                        ScrapeResult(arr, req);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_showScrapingErrors) ProgramUtils.ShowErrorMessage(ex.Message + ex.StackTrace, true);
                label14.Text = (++_scraperErrors).ToString();
                if (_useRetries)
                    if (retry <= _retries)
                    {
                        retry++;
                        goto again;
                    }
            }
        }

        private void ScrapeResult(string[] links, HttpRequest req)
        {
            try
            {
                if (req == null)
                {
                    again: using (req = new HttpRequest())
                    {
                        req.UserAgent = Utils.Randomizer.RandomUserAgent();
                        if (_useProxies)
                        {
                            ProxyClient client = RandomProxy();
                            if (client != null) req.Proxy = client; else goto again;
                        }
                        req.EnableEncodingContent = true;
                        req.IgnoreInvalidCookie = true;
                        req.IgnoreProtocolErrors = true;
                        if (_useRetries)
                        {
                            req.Reconnect = true;
                            req.ReconnectDelay = _timeout;
                            req.ReconnectLimit = _retries;
                        }
                        req.SslProtocols = SslProtocols.Tls12;
                        req.SslCertificateValidatorCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                        req.UseCookies = true;
                        req.ConnectTimeout = _timeout;
                        req.ReadWriteTimeout = _timeout;
                        req.Cookies = new CookieStorage();
                        req.AllowAutoRedirect = true;
                        req.MaximumAutomaticRedirections = 10;

                        req.AddHeader("Upgrade-Insecure-Requests", "1");
                        req.AddHeader("Accept", "*/*");

                        _links.Text = string.Join(Environment.NewLine, links.Distinct());

                        label18.Text = $"Got {_links.Lines.Length} links, scraping result...";
                        foreach (string link in _links.Lines)
                        {
                            if (_useCustomLinks) label24.Text = link;
                            string response = req.Get(link).ToString();
                            if (link.Contains("anonfiles.com"))
                            {
                                MatchCollection regex = Regex.Matches(response, @"(https:\/\/.*.anonfiles.com\/.*)");
                                List<string> arr = regex.OfType<Match>().Select(m => m.Value).Distinct().ToList();
                                List<string> anonlinks = new List<string>();
                                if (arr.Count > 0 && arr.Last() != Environment.NewLine)
                                {
                                    arr.Add(Environment.NewLine);
                                    anonlinks.Add(string.Join(Environment.NewLine, arr).Replace(">                    <img", string.Empty).Replace("\"", string.Empty));
                                }
                                foreach (string anonlink in anonlinks)
                                {
                                    string respo = req.Get(anonlink).ToString();
                                    AppendResult(respo);
                                }
                            }
                            else
                            {
                                if (_useCustomLinks) label24.Text = link;
                                AppendResult(response);
                            }
                        }
                    }
                }
                else
                {
                    _links.Text = string.Join(Environment.NewLine, links.Distinct());

                    label18.Text = $"Got {_links.Lines.Length} links, scraping result...";
                    foreach (string link in _links.Lines)
                    {
                        if (_useCustomLinks) label24.Text = link;
                        string response = req.Get(link).ToString();
                        if (link.Contains("anonfiles.com"))
                        {
                            MatchCollection regex = Regex.Matches(response, @"(https:\/\/.*.anonfiles.com\/.*)");
                            List<string> arr = regex.OfType<Match>().Select(m => m.Value).Distinct().ToList();
                            List<string> anonlinks = new List<string>();
                            if (arr.Count > 0 && arr.Last() != Environment.NewLine)
                            {
                                arr.Add(Environment.NewLine);
                                anonlinks.Add(string.Join(Environment.NewLine, arr).Replace(">                    <img", string.Empty).Replace("\"", string.Empty));
                            }
                            foreach (string anonlink in anonlinks)
                            {
                                string respo = req.Get(anonlink).ToString();
                                AppendResult(respo);
                            }
                        }
                        else
                        {
                            if (_useCustomLinks) label24.Text = link;
                            AppendResult(response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_showScrapingErrors) ProgramUtils.ShowErrorMessage(ex.Message + ex.StackTrace, true);
                label14.Text = (++_scraperErrors).ToString();
            }
        }

        private void GetResult(string response, string regexx, string type)
        {
            MatchCollection regex = Regex.Matches(response, regexx);
            List<string> arr = regex.OfType<Match>().Select(m => m.Value).Distinct().ToList();
            if (arr.Count > 0 && arr.Last() != Environment.NewLine)
            {
                arr.Add(Environment.NewLine);
                textBox1.AppendText(string.Join(Environment.NewLine, arr));
                label18.Text = $"Scraped {arr.Count} {type}.";
            }
        }

        private void AppendResult(string response)
        {
            try
            {
                if (_leechOption == "emailpass") GetResult(response, 
                    @"([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}):([a-zA-Z0-9_\-\.]+)", 
                    "combos");
                else if (_leechOption == "userpass") GetResult(response, 
                    @"[a-z0-9_-]{3,16}:([a-zA-Z0-9_\-\.]+)", 
                    "combos");
                else if (_leechOption == "proxies") GetResult(response, 
                    @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?=[^\d])\s*:?\s*(\d{2,5})", 
                    "proxies");
                else if (_leechOption == "emailonly") GetResult(response, 
                    @"([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})", 
                    "emails");
                else if (_leechOption == "custom") GetResult(response, _customRegex, 
                    "result");
            }
            catch (Exception ex)
            {
                if (_showScrapingErrors) ProgramUtils.ShowErrorMessage(ex.Message + ex.StackTrace, true);
                label14.Text = (++_scraperErrors).ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{Application.ProductName} v{Application.ProductVersion} " +
                $"made by AnErrupTion from {Application.CompanyName}.", "About this software");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label17.Text = textBox1.Lines.Length.ToString();
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                button4.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                button4.Enabled = true;
                button5.Enabled = true;
            }
        }

        private void SetGuiElements(bool enable)
        {
            button1.Enabled = enable;
            button3.Enabled = enable;
            button6.Enabled = enable;
            button7.Enabled = enable;
            textBox2.Enabled = enable;
            textBox3.Enabled = enable;
            textBox4.Enabled = enable;
        }

        private void SetInformations(bool result)
        {
            if (result)
            {
                textBox1.Clear();
                label17.Text = "0";
                label19.Text = "0";
                label13.Text = "0";
                _programErrors = 0;
                label14.Text = "0";
                _scraperErrors = 0;
            }
            label18.Text = "IDLE";
            label16.Text = "None";
            label15.Text = "None";
            label21.Text = "None";
            label24.Text = "None";
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int retriesBefore = 0;
            if (_useRetries)
            {
                retriesBefore = _retries;
                _retries = 1;
            }

            _stop = true;
            _worker.Abort();
            if (_useRetries) _retries = retriesBefore;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> result = new List<string>();
            foreach (string line in textBox1.Lines)
                if (line.Length > 5 && line != Environment.NewLine && !string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line))
                    result.Add(line);

            textBox1.Text = string.Join(Environment.NewLine, result.Distinct());
            MessageBox.Show("Sucessfully cleaned the result!", "Success!");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
            MessageBox.Show("Successfully saved your result!", "Success!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new LeechOptions().Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new MoreOptions().Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label2.Text = textBox2.Lines.Length.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox3.Lines.Length.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label5.Text = textBox4.Lines.Length.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox2.Lines = File.ReadAllLines(openFileDialog1.FileName);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog2.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog2.FileName = string.Empty;
            openFileDialog2.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            openFileDialog3.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog3.FileName = string.Empty;
            openFileDialog3.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            textBox4.Lines = File.ReadAllLines(openFileDialog2.FileName);
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            textBox3.Lines = File.ReadAllLines(openFileDialog3.FileName);
        }
    }
}
