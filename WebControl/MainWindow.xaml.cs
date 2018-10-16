using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        string ipCSVPath;

        string remoteCodeCSVPath;

		Thread t;

		List<string> ipList = new List<string>();

        List<string> codeList = new List<string>();

		List<string> Completed = new List<string>();

		List<string> Failed = new List<string>();

		public MainWindow()
        {

            InitializeComponent();			

            //this.Height = Screen.PrimaryScreen.Bounds.Height - 40;

            //this.Width = Screen.PrimaryScreen.Bounds.Width;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

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
			t = new Thread(startSelenium);
			t.Start();
        }

		public void startSelenium()
		{
			foreach (string ip in ipList)
			{
				string loginURL = "http://" + ip + "/web/guest/en/websys/webArch/authForm.cgi";

				string rcGateURL = "http://" + ip + "/web/entry/en/websys/atRemote/atRemoteSetupGet.cgi";

				ChromeOptions options = new ChromeOptions();

				options.AddArgument("test-type");

				IWebDriver driver = new ChromeDriver(options);

				try
				{
					driver.Navigate().GoToUrl(loginURL);

					if (driver.Url.EndsWith("message.cgi"))
					{
						//driver.FindElement(By.XPath("//a[@href = 'javascript:jumpButtonURL()']")).Click();
						driver.Navigate().GoToUrl(loginURL);
					}

					driver.FindElement(By.Name("userid_work")).SendKeys("admin");

					driver.FindElement(By.XPath("//input[@type = 'submit']")).Click();

					driver.Navigate().GoToUrl(rcGateURL);
				}
				catch
				{
					Dispatcher.Invoke(new Action(() => {

						listError.Items.Add(new MyItem { Ip = ip.ToString(), Code = "Na" });

					}));
				}

				try
				{
					var registered = driver.FindElement(By.XPath("//td[text()='Registered']"));

					if (registered != null)
					{
						//todo:Already Registered Handling
						Dispatcher.Invoke(new Action(() => {

							listCompleted.Items.Add(new MyItem { Ip = ip, Code = "Na" });

						}));

					}
				}
				catch
				{
					
				}

				try
				{
					var notProg = driver.FindElement(By.XPath("//td[text()='Not Programmed']"));

					if (notProg != null)
					{
						string code = codeList.Last();

						driver.FindElement(By.Name("letterNo")).Clear();

						driver.FindElement(By.Name("letterNo")).SendKeys(code);

						codeList.Remove(code);

						driver.FindElement(By.XPath("//a[@href='javascript:refer()']")).Click();

						if (driver.Url.EndsWith("atRemoteSetupRefer.cgi"))
						{
							try
							{
								var error = driver.FindElement(By.XPath("//a[@href='javascript:okInput()']"));

								if (error != null)
								{
									driver.FindElement(By.XPath("//a[@href='javascript:okInput()']")).Click();

									Dispatcher.Invoke(new Action(() => {

										listError.Items.Add(new MyItem { Ip = ip, Code = "Na" });

									}));
									//todo:add error handling
								}
							}
							catch
							{
							
							}

							try
							{
								var success = driver.FindElement(By.XPath("//a[@href='javascript:ok()']"));

								if (success != null)
								{
									driver.FindElement(By.XPath("//a[@href='javascript:ok()']")).Click();

									//javascript:regist()

									driver.FindElement(By.XPath("//a[@href='javascript:regist()']")).Click();

									//todo: add to complete list

									Dispatcher.Invoke(new Action(() => {

										listCompleted.Items.Add(new MyItem { Ip = ip, Code = code });

									}));

								}
							}
							catch
							{
							
							}
						}
						driver.FindElement(By.XPath("//a[@href='javascript:regist()']")).Click();
					}
				}
				catch
				{

				}

				driver.Dispose();
			}

			t.Abort();

			System.Windows.MessageBox.Show("Complete");
		}

		private void AddToCompleted(string ip)
		{
			Completed.Add(ip);
		}

		private void AddtoFailedIP(string ip)
		{
			Failed.Add(ip);
		}

		private void saveListToCSV(List <string> list)
		{
			string csv = String.Join("/n", list);

		}
    }
}
public class MyItem
{
	public string Ip { get; set; }

	public string Code { get; set; }
}
