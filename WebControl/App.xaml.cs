using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace WebControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		public App()
		{
		ServicePointManager.ServerCertificateValidationCallback +=
			(sender, cert, chain, error) =>
			{
				return true;
			};
		}
    }
}
