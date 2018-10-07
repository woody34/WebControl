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
                //start the bot
            }
        }
    }
}
