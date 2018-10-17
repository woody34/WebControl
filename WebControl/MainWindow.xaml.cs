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
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

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

		Thread t0;
		Thread t1;
		Thread t2;
		Thread t3;
		Thread t4;
		Thread t5;
		Thread t6;
		Thread t7;
		Thread t8;
		Thread t9;

		string username = "admin";

		string password = "";

		List<string> ipList = new List<string>();

        List<string> codeList = new List<string>();

		public MainWindow()
        {

            InitializeComponent();			

            //this.Height = Screen.PrimaryScreen.Bounds.Height - 40;

            //this.Width = Screen.PrimaryScreen.Bounds.Width;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			//Logging
			var outputter = new TextBoxOutputter(TestBox);

			Console.SetOut(outputter);

			Console.WriteLine("Log Initialized...");

		}

        private void btnIP_Click(object sender, RoutedEventArgs e)
        {
			try
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
			catch
			{

			}
        }

        private void btnRemoteCodes_Click(object sender, RoutedEventArgs e)
        {
			try
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
			catch
			{

			}
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
			username = textboxUsername.Text;

			password = textboxPassword.Text;

			if(codeList.Count == 0)
			{
				string ipCSVPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\codes.csv";

				string temp = File.ReadAllText(@ipCSVPath);

				ParseCsv(temp, codeList);
			}

			if (ipList.Count == 0)
			{
				string ipCSVPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\ipList.csv";

				string temp = File.ReadAllText(@ipCSVPath);

				ParseCsv(temp, ipList);
			}

			//Thread.Sleep() implemented to prevent threads from modifying the ipList<> simultaniously
			t0 = new Thread(startSelenium);
			t0.Start();
			Thread.Sleep(100);

			t1 = new Thread(startSelenium);
			t1.Start();
			Thread.Sleep(100);

			t2 = new Thread(startSelenium);
			t2.Start();
			Thread.Sleep(100);

			t3 = new Thread(startSelenium);
			t3.Start();
			Thread.Sleep(100);

			t4 = new Thread(startSelenium);
			t4.Start();
			Thread.Sleep(100);

			t5 = new Thread(startSelenium);
			t5.Start();
			Thread.Sleep(100);

			t6 = new Thread(startSelenium);
			t6.Start();
			Thread.Sleep(100);

			t7 = new Thread(startSelenium);
			t7.Start();
			Thread.Sleep(100);

			t8 = new Thread(startSelenium);
			t8.Start();
			Thread.Sleep(100);

			t9 = new Thread(startSelenium);
			t9.Start();
		}

		private void btnSaveComplete_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				DataTable table = new DataTable();
				table.Columns.Add("Ip");
				table.Columns.Add("Code");
				foreach (MyItem item in listCompleted.Items)
				{
					table.Rows.Add(item.Ip, item.Code);
				}

				CreateCSVFile(table, System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\complete.csv");
			}
			catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
		}

		private void btnSaveError_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				DataTable table = new DataTable();

				table.Columns.Add("Ip");
				table.Columns.Add("Code");
				foreach (MyItem item in listCompleted.Items)
				{
					table.Rows.Add(item.Ip, item.Code);
				}

				CreateCSVFile(table, System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\error.csv");
			}
			catch { }
		}

		private void link_Click(object sender, RoutedEventArgs e)
		{
			ProcessStartInfo sInfo = new ProcessStartInfo("http://www.github.com/woody34/WebControl/");
			Process.Start(sInfo);
		}

		private void startSelenium()
		{
			bool exit = false;

			while(!exit)
			{
				try
				{
					ipList.Last();
				}
				catch
				{
					exit = true;
					return;
				}
				if(ipList.Count == 0)
				{
					exit = true;
					return;
				}
				string ip = ipList.Last();

				ipList.Remove(ip);

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

					driver.FindElement(By.Name("userid_work")).SendKeys(username);

					driver.FindElement(By.Name("password_work")).SendKeys(password);

					driver.FindElement(By.XPath("//input[@type = 'submit']")).Click();

					driver.Navigate().GoToUrl(rcGateURL);
				}
				catch
				{
					Dispatcher.Invoke(new Action(() => {

						listError.Items.Add(new MyItem { Ip = ip.ToString(), Code = "Nav/Auth Failed" });

						ClickWPFbutton(btnSaveError);

						string subPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\ss\";

						Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

						string screenshot = ss.AsBase64EncodedString;

						byte[] screenshotAsByteArray = ss.AsByteArray;

						if (!System.IO.Directory.Exists(subPath))
						{
							System.IO.Directory.CreateDirectory(subPath);
						}

						ss.SaveAsFile(subPath + ip + "-error.png");

						Console.WriteLine(ip + " navigation or login failed,"  + " see: " + subPath + ip + "-error.png");

					}));
				}

				try
				{
					var registered = driver.FindElement(By.XPath("//td[text()='Registered']"));

					if (registered != null)
					{
						//todo:Already Registered Handling
						Dispatcher.Invoke(new Action(() => {

							listCompleted.Items.Add(new MyItem { Ip = ip, Code = "Registered" });

							Console.WriteLine(ip + " was already Registered to @Remote");

							ClickWPFbutton(btnSaveCompleted);

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

										listError.Items.Add(new MyItem { Ip = ip, Code = "Code Error Returned" });

										ClickWPFbutton(btnSaveError);
									}));

									string subPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\ss\";

									Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

									string screenshot = ss.AsBase64EncodedString;

									byte[] screenshotAsByteArray = ss.AsByteArray;

									if (!System.IO.Directory.Exists(subPath))
									{
										System.IO.Directory.CreateDirectory(subPath);
									}

									ss.SaveAsFile(subPath + ip + "-error.png");

									Console.WriteLine(ip + " failed activation with " + code + ", see: " + subPath + ip + "-error.png");
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

										ClickWPFbutton(btnSaveCompleted);

										Console.WriteLine(ip + " activated with " + code);

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

			Thread.CurrentThread.Abort();

			//System.Windows.MessageBox.Show("Complete");
		}

		private void ParseCsv(string csv, List<string> list)
		{
			string[] values = csv.Split('\n', '\r');
			foreach (string ip in values)
			{
				if (ip != "")
				{
					list.Add(ip);
				}

			}

			Console.WriteLine(".csv file loaded into memory.");
		}

		private void CreateCSVFile(DataTable dt, string strFilePath)
		{
			StreamWriter sw = new StreamWriter(strFilePath, false);

			int iColCount = dt.Columns.Count;
			for (int i = 0; i < iColCount; i++)
			{
				sw.Write(dt.Columns[i]);
				if (i < iColCount - 1)
				{
					sw.Write(",");
				}
			}
			sw.Write(sw.NewLine);

			foreach (DataRow dr in dt.Rows)
			{
				for (int i = 0; i < iColCount; i++)
				{
					if (!Convert.IsDBNull(dr[i]))
					{
						sw.Write(dr[i].ToString());
					}
					if (i < iColCount - 1)
					{
						sw.Write(",");
					}
				}
				sw.Write(sw.NewLine);
			}
			sw.Close();

			Console.WriteLine("saved: " + strFilePath);
		}

		private void ClickWPFbutton(System.Windows.Controls.Button b)
		{
			ButtonAutomationPeer peer = new ButtonAutomationPeer(b);
			IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			invokeProv.Invoke();
		}

		private void btnSaveLog_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}

public class TextBoxOutputter : TextWriter
{
	System.Windows.Controls.TextBox textBox = null;

	public TextBoxOutputter(System.Windows.Controls.TextBox output)
	{
		textBox = output;
	}

	public override void Write(char value)
	{
		base.Write(value);

		textBox.Dispatcher.BeginInvoke(new Action(() =>
		{
			textBox.AppendText(value.ToString());
		}));
	}

	public override Encoding Encoding
	{
		get { return System.Text.Encoding.UTF8; }
	}
}


public class MyItem : ListViewItem
{
	public string Ip { get; set; }

	public string Code { get; set; }
}
