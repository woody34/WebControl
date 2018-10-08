using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using mshtml;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Reflection;
using System.Threading;


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
                if (ip != "")
                {
                    list.Add(ip);
                    Console.WriteLine(ip);
                }

            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            clickLogin();
            login();
            navConfig();
            navRCGateSetup();

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

            foreach (HtmlElement a in anchors)
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

            foreach (HtmlElement a in anchors)
            {
                string href = a.GetAttribute("href");

                if (href.Contains(loginURL))
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
        private bool clickLogin()
        {
            bool success = false;

            string loginURL = "/web/guest/en/websys/webArch/authForm.cgi";

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                foreach (IHTMLElement a in anchors)
                {
                    string href = a.getAttribute("href");

                    if (href.Contains(loginURL))
                    {
                        a.click();
                        break;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }

            return success;
        }

        private bool login()
        {
            bool success = false;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection inputs = (IHTMLElementCollection) html.getElementsByTagName("input");

                //input admin login
                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("name").Equals("userid_work"))
                    {
                        i.innerText = "admin";
                        break;
                    }
                }
                //click submit
                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("value").Equals("Login"))
                    {
                        i.click();
                        break;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }

            return success;
        }

        private bool navConfig()
        {
            bool success = false;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                //input admin login
                foreach (IHTMLElement a in anchors)
                {
                    if (a.innerText.Equals("Configuration"))
                    {
                        a.click();
                        break;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }

            return success;
        }

        private bool navRCGateSetup()
        {
            bool success = false;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                //input admin login
                foreach (IHTMLElement a in anchors)
                {
                    if (a.innerText.Equals("Setup RC Gate"))
                    {
                        a.click();
                        break;
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }
            return success;
        }

        private bool gooTest()
        {
            bool success = false;

            while (webBrowser.IsLoaded.Equals(false))
            {
                System.Threading.Thread.Sleep(100);
            }

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection inputs = (IHTMLElementCollection) html.getElementsByTagName("input");

                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("title") == "Search")
                    {
                        i.innerText = "Matt";
                        
                    }
                }

                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("value") == "Google Search")
                    {
                        i.click();
                        success = true;
                    }
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }

            return success;
        }
    }
}
