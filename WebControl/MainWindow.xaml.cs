using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WebControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    /// 
    ///Test
    public partial class MainWindow : Window
    {
        string ipCSVPath;
        string remoteCodeCSVPath;

        List<string> ipList = new List<string>();
        List<string> codeList = new List<string>();

        public MainWindow()
        {

            InitializeComponent();
            webBrowser.Navigate("http://www.google.com");
        }

        private void txtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                webBrowser.Navigate(txtUrl.Text);
        }

        private void webBrowser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((webBrowser != null) && (webBrowser.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((webBrowser != null) && (webBrowser.CanGoForward));
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.GoForward();
        }

        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.Navigate(txtUrl.Text);
        }

        private void btnIP_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();

            choofdlog.Filter = "CSV Files|*.csv";

            choofdlog.FilterIndex = 1;

            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.IO.StreamReader sr = new
                //System.IO.StreamReader(@choofdlog.FileName);
                //System.Windows.MessageBox.Show(sr.ReadToEnd());
                //sr.Close();

                ipCSVPath = @choofdlog.FileName;
            }

            string temp = File.ReadAllText(ipCSVPath);

            ParseCsv(temp, ipList);
        }



        private void btnRemoteCodes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();

            choofdlog.Filter = "CSV Files|*.csv";

            choofdlog.FilterIndex = 1;

            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.IO.StreamReader sr = new
                //System.IO.StreamReader(@choofdlog.FileName);
                //System.Windows.MessageBox.Show(sr.ReadToEnd());
                //sr.Close();

                remoteCodeCSVPath = choofdlog.FileName;
            }

            string temp = File.ReadAllText(remoteCodeCSVPath);

            ParseCsv(temp, codeList);
        }
        private void ParseCsv(string csv, List<string> list)
        {
            string[] values = csv.Split('\n', '\r');
            foreach (string ip in values)
            {
                if( ip != "")
                {
                    list.Add(ip);
                    Console.WriteLine(ip);
                }

            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (null == ipList && null == codeList)
            {
                return;
            }
            else
            {
                if (isLoginAvailable())
                {
                    login();
                }
            }
        }

        //Bot Actions
        private bool isPageWim()
        {
            HtmlDocument html = webBrowser.Document as HtmlDocument;

            bool titleHasWebImageMonitor = false;

            titleHasWebImageMonitor = (html.Title.Contains("Web Image Monitor")) ? true : false;

            if (titleHasWebImageMonitor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isConfigAvailable()
        {
            HtmlDocument html;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            html = webBrowser.Document as HtmlDocument;

            HtmlElementCollection anchors = html.GetElementsByTagName("a");

            foreach(HtmlElement a in anchors)
            {
                if (a.OuterText.Equals("Configuration"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Looks for the anchor tag to login
        /// </summary>
        /// <returns>true if anchor tag is present</returns>
        private bool isLoginAvailable()
        {
            HtmlDocument html = webBrowser.Document as HtmlDocument;

            bool isLogIn = false;

            string loginURL = "/web/guest/en/websys/webArch/authForm.cgi";

            HtmlElementCollection anchors = html.GetElementsByTagName("a");

            foreach(HtmlElement a in anchors)
            {
                string href = a.GetAttribute("href");
                
                if(href.Contains(loginURL))
                {
                    isLogIn = true; 
                }
                 
            }

            return (isLogIn) ? true : false;
        }

        /// <summary>
        /// Looks for the anchor tag to logout
        /// </summary>
        /// <returns>true if anchor tag is present</returns>
        private bool isLogoutAvailable()
        {
            HtmlDocument html = webBrowser.Document as HtmlDocument;

            bool isLoggedIn = false;

            string logoutURL = "/web/entry/en/websys/webArch/logout.cgi";

            HtmlElementCollection anchors = html.GetElementsByTagName("a");

            foreach (HtmlElement a in anchors)
            {
                string href = a.GetAttribute("href");

                if (href.Contains(logoutURL))
                {
                    isLoggedIn = true;
                }

            }

            return (isLoggedIn) ? true : false;
        }

        /// <summary>
        /// Navigates to the login screen and logs in as admin with default credentials
        /// </summary>
        /// <returns>a bool of success</returns>
        private bool login()
        {
            HtmlDocument html;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            if (isLoginAvailable())
            {
                html = webBrowser.Document as HtmlDocument;

                HtmlElementCollection anchors = html.GetElementsByTagName("a");

                string loginURL = "/web/guest/en/websys/webArch/authForm.cgi";

                //find login anchor and invoke click
                foreach (HtmlElement a in anchors)
                {
                    string href = a.GetAttribute("href");

                    if (href.Contains(loginURL))
                    {
                        a.InvokeMember("click");

                        break;
                    }
                }
            }

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            html = webBrowser.Document as HtmlDocument;

            HtmlElementCollection inputs = html.GetElementsByTagName("input");

            //find input for User Name & input default username
            foreach (HtmlElement input in inputs)
            {
                //find input for admin loging
                if (input.Name.Contains("userid_work"))
                {
                    //set value attribute to admin
                    input.SetAttribute("value", "admin");
                }

            }

            //find login input and invoke click
            foreach (HtmlElement input in inputs)
            {
                if (input.GetAttribute("value").Equals("Login"))
                {
                    input.InvokeMember("click");
                    return true;
                }
            }

            return false;
        }

        private bool navConfig()
        {
            HtmlDocument html;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            html = webBrowser.Document as HtmlDocument;

            HtmlElementCollection anchors = html.GetElementsByTagName("a");

            foreach (HtmlElement a in anchors)
            {
                if (a.OuterText.Equals("Configuration"))
                {
                    a.InvokeMember("click");

                    return true;
                }
            }

            return false;
        }

        private bool navRCGateSetup()
        {
            HtmlDocument html;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            html = webBrowser.Document as HtmlDocument;

            HtmlElementCollection anchors = html.GetElementsByTagName("a");

            foreach (HtmlElement a in anchors)
            {
                if (a.OuterText.Equals("Setup RC Gate"))
                {
                    a.InvokeMember("click");

                    return true;
                }
            }

            return false;
        }
    }
}
