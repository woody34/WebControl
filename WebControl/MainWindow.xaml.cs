using System;
using System.Windows;
using System.Windows.Input;
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
using HtmlAgilityPack;
using System.Windows.Forms;

namespace WebControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    /// 
    ///Test
    public partial class MainWindow : Window
    {
        bool loadingStatus = false;

        string ipCSVPath;
        string remoteCodeCSVPath;

        List<string> ipList = new List<string>();
        List<string> codeList = new List<string>();

        public MainWindow()
        {

            InitializeComponent();
            webBrowser.LoadCompleted += PageLoadCompleted;
            webBrowser.Navigate("http://apycom.com/website-buttons/exframe.html");
        }
        void PageLoadCompleted(object sender, NavigationEventArgs e)
        {
            loadingStatus = true;
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
            string LoginURL = "/web/guest/en/websys/webArch/authForm.cgi";

            string rcGateURL = "/web/entry/en/websys/atRemote/atRemoteSetupGet.cgi";

            webBrowser.Navigate("http://" + "10.89.29.31" + LoginURL);

            login();

            webBrowser.Navigate("10.89.29.31" + rcGateURL);

            if (getRCGateStatus())
            {
                //todo: remove item from list
            }
            else
            {
                confirmCode("test"); //plugs in code and hits confirm

                //programCode();
                //todo: cofirm and program @remote with codes, if successful remove code from codelist
            }
        }


        public bool waitForPageLoad()//todo setup timer for timeout or hook into page timeout
        {
            while (!loadingStatus)
            {
                System.Threading.Thread.Sleep(100);
            }

            return true;
        }

        private bool login()
        {
            bool success = false;

            waitForPageLoad();

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
                    }
                }
                //click submit
                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("value").Equals("Login"))
                    {
                        loadingStatus = false;

                        i.click();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }

            return success;
        }

        private bool getRCGateStatus()
        {
            bool success = true; // returns true if it can't figure out if @ remote needs setup

            waitForPageLoad();

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection inputs = (IHTMLElementCollection) html.getElementsByTagName("td");

                foreach (IHTMLElement i in inputs)
                {
                    if (i.innerText == "Not Programmed")
                    {
                        return success;
                    }
                }

                foreach (IHTMLElement i in inputs)
                {
                    if (i.innerText == "Registered")
                    {
                        return success;
                    }
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Null html");
            }
            return success;
        }

        private bool confirmCode( string activationCode)
        {
            bool success = false;

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection inputs = (IHTMLElementCollection) html.getElementsByTagName("input");

                foreach (IHTMLElement i in inputs)
                {
                    if (i.getAttribute("name") == "letterNo")
                    {
                        i.innerText = activationCode;
                    }
                }

                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                foreach (IHTMLElement a in anchors)
                {
                    if (a.innerText == "Confirm")
                    {
                        loadingStatus = false;

                        a.click();

                        waitForPageLoad();

                        clickOk();

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

        private bool programCode()
        {
            bool success = false;

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                foreach (IHTMLElement a in anchors)
                {
                    if (a.innerText == "Program")
                    {
                        loadingStatus = false;

                        a.click();

                        waitForPageLoad();

                        clickOk();

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

        private bool clickOk()
        {
            bool success = false;

            HTMLDocument html = (HTMLDocument) webBrowser.Document;

            if (html != null)
            {
                IHTMLElementCollection anchors = (IHTMLElementCollection) html.getElementsByTagName("a");

                foreach (IHTMLElement a in anchors)
                {
                    if (a.innerText == "Ok")
                    {
                        loadingStatus = false;

                        a.click();

                        waitForPageLoad();

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
