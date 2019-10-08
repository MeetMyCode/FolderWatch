using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IrmerWatchGUI.CustomWindows;
using System.IO;
using Path = System.IO.Path;
using Application = System.Windows.Application;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Threading;
using System.Text.RegularExpressions;
using Label = System.Windows.Controls.Label;
using System.Configuration;

namespace IrmerWatchGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private StackPanel recipientStackPanel = new StackPanel();
		private static string ServiceName = @"IrmerWatchService";
		private static string baseDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		private static string scExePath = baseDirectory + @"\IrmerWatchService\sc.exe";
		private static string serviceName = @"IrmerWatchService";
		

		public MainWindow()
		{
			InitializeComponent();

			CheckIfServiceIsRunning();
			SenderEmailAddress.Content = GetSenderEmail();
			Recipients.Content = GetRecipientStackPanel();
		}

		private void CheckIfServiceIsRunning()
		{
			ServiceController[] services = ServiceController.GetServices();
			foreach (ServiceController service in services)
			{
				if (service.ServiceName == ServiceName)
				{
					ServiceStatusText.Content = service.Status;
					if (service.Status == ServiceControllerStatus.Running)
					{
						StopServiceButton.IsEnabled = true;
						StartServiceButton.IsEnabled = false;
						ServiceStatusText.Foreground = Brushes.Green;
					}
					else if(service.Status == ServiceControllerStatus.Stopped)
					{
						ServiceStatusText.Foreground = Brushes.Red;
						StopServiceButton.IsEnabled = false;
						StartServiceButton.IsEnabled = true;
					}
				}
				else
				{
					ServiceStatusText.Content = string.Format(@"{0} not installed.", ServiceName);
					ServiceStatusText.Foreground = Brushes.Red;
					StopServiceButton.IsEnabled = false;
					StartServiceButton.IsEnabled = true;
				}
			}
		}

		private StackPanel GetRecipientStackPanel()
		{
			recipientStackPanel.Children.Clear();

			string email;

			Recipients.Content = null;

			// Read the file and display it line by line. 
			foreach (SettingsProperty setting in Properties.RecipientSettings.Default.Properties)
			{
				if (setting.Name != @"SenderEmail")
				{
					email = setting.DefaultValue.ToString();
					Label recipient = new Label();
					recipient.Content = email;
					recipientStackPanel.Children.Add(recipient);
				}
			}
			return recipientStackPanel;
		}

		private string GetSenderEmail()
		{
			return Properties.SenderSettings.Default.Sender.ToString();
		}

		private void BrowseDirectoryButtonClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			DialogResult result = dialog.ShowDialog();

			if (result.ToString() == @"OK")
			{
				FolderToMonitor.Content = dialog.SelectedPath;
			}

		}

		private void OpenSettings(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			settingsWindow.Show();
		}


		private void StopService(object sender, RoutedEventArgs e)
		{
			try
			{
				string baseDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
				string scExePath = baseDirectory + @"\IrmerWatchService\sc.exe";
				string serviceName = @"IrmerWatchService";

				////**************************************8

				ProcessStartInfo procInfo = new ProcessStartInfo();
				procInfo.Verb = @"runas";
				procInfo.UseShellExecute = true;
				procInfo.FileName = scExePath;

				procInfo.Arguments = string.Format(@" stop {0}", serviceName);

				Process.Start(procInfo);

				ServiceController[] services = ServiceController.GetServices();
				foreach (ServiceController service in services)
				{
					if (service.ServiceName == @"IrmerWatchService")
					{
						ServiceControllerStatus status = service.Status;

						while (status != ServiceControllerStatus.Stopped)
						{
							ServiceStatusText.Content = status.ToString();
							StopServiceButton.IsEnabled = false;
							StartServiceButton.IsEnabled = false;
							Thread.Sleep(1000);
							service.Refresh();
							status = service.Status;
						}

						if (status == ServiceControllerStatus.Stopped)
						{
							ServiceStatusText.Content = status.ToString();
							ServiceStatusText.Foreground = Brushes.Red;
							StopServiceButton.IsEnabled = false;
							StartServiceButton.IsEnabled = true;
						}
						else
						{
							ServiceStatusText.Content = status.ToString();
							ServiceStatusText.Foreground = Brushes.Red;
							StopServiceButton.IsEnabled = true;
							StartServiceButton.IsEnabled = false;
						}

					}
				}

				//**************************************8

			}
			catch (InvalidOperationException stopError)
			{
				MessageBox.Show(@"Error: " + stopError.InnerException.ToString());
			}

		}

		private void StartService(object sender, RoutedEventArgs e)
		{
			if (ServiceExists("IrmerWatchService"))
			{
				if (ValidationPassedBeforeStartingService())
				{
					StartIrmerWatchService();
				}
				else
				{
					//Inform User to amend invalid inputs.
					MessageBox.Show(@"Inputs in red are invalid. Please correct and start the service again.");
				}
			}
			else
			{
				if (ValidationPassedBeforeStartingService())
				{
					InstallIrmerWatchService();
					StartIrmerWatchService();
				}
				else
				{
					//Inform User to amend invalid inputs.
					MessageBox.Show(@"Inputs in red are invalid. Please correct and start the service again.");
				}
			}
		}

		private bool ValidationPassedBeforeStartingService()
		{
			bool isValid = true;
			List<string> invalidEmails = GetInvalidEmails();

			if (SenderPassword.Password == @"")
			{
				isValid = false;
				SenderPassword.Background = Brushes.Red;
			}

			if (!Directory.Exists(FolderToMonitor.Content.ToString()))
			{
				isValid = false;
				FolderToMonitor.Foreground = Brushes.Red;
			}

			if (invalidEmails.Count != 0)
			{
				isValid = false;

				foreach (string email in invalidEmails)
				{
					//Amend sender address if necessary
					if (email.ToLower() == SenderEmailAddress.Content.ToString().ToLower())
					{
						SenderEmailAddress.Foreground = Brushes.Red;
					}

					//Amend any recipient addresses if necessary.
					foreach (Label emailAddy in recipientStackPanel.Children)
					{
						if (emailAddy.Content.ToString().ToLower() == email.ToLower())
						{
							emailAddy.Foreground = Brushes.Red;
						}
					}
				}
			}			

			return isValid;
		}

		private List<string> GetInvalidEmails()
		{
			//Collate addresses into a single list
			List<string> invalidEmails = new List<string>();
			List<string> addresses = new List<string>();
			addresses.Add(SenderEmailAddress.Content.ToString());

			foreach (Label email in recipientStackPanel.Children)
			{
				string address = email.Content.ToString();
				addresses.Add(address);
			}

			//Iterate through the list of addresses and validate.
			foreach (string email in addresses)
			{
				string beforeAtSymbol = @"";
				string theRestOfTheString = @"";

				string[] twoStrings = email.Split('@');

				beforeAtSymbol = twoStrings[0];
				theRestOfTheString = twoStrings[1];

				//Validate
				var regex = new Regex(@"[^a-zA-Z0-9.]");
				Match result = regex.Match(beforeAtSymbol); //If invalid char is found, result is true.

				if (result.Success == true || theRestOfTheString != @"nhs.net")
				{
					invalidEmails.Add(email);
				}
			}

			return invalidEmails;
		}

		private void InstallIrmerWatchService()
		{
			string baseDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			string IrmerWatchServicePath = baseDirectory + @"\IrmerWatchService\IrmerWatch.exe";
			string InstallUtilPath = baseDirectory + @"\IrmerWatchService\InstallUtil.exe";

			Process process = new Process();
			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.FileName = InstallUtilPath;

			processInfo.UseShellExecute = true;
			processInfo.Verb = @"runas";
			processInfo.Arguments = IrmerWatchServicePath;
			process.StartInfo = processInfo;

			try
			{
				process.Start();
			}
			catch (Exception)
			{
				return;
			}

			ServiceStatusText.Content = @"Installing Service...";

			Thread.Sleep(3000);
			process.Close();

			ServiceStatusText.Content = @"Installing Service...Done!";


		}


		private void StartIrmerWatchService()
		{
			ServiceStatusText.Content = @"Starting Service...";

			try
			{
				ProcessStartInfo procInfo = new ProcessStartInfo();
				procInfo.Verb = @"runas";
				procInfo.UseShellExecute = true;
				procInfo.FileName = scExePath;

				string[] args = {
					SenderEmailAddress.Content.ToString(),
					SenderPassword.Password,
					GetRecipientsFromStackPanel(),
					FolderToMonitor.Content.ToString()
				};

				procInfo.Arguments = string.Format(@" start {0} {1} {2} {3} {4}", serviceName, args[0], args[1], args[2], args[3]);

				try
				{
					Process.Start(procInfo);
				}
				catch (Exception)
				{
					//MessageBox.Show(@"Error: " + e.InnerException.ToString());
					return;
				}

				ServiceController[] services = ServiceController.GetServices();
				foreach (ServiceController service in services)
				{
					if (service.ServiceName == @"IrmerWatchService")
					{
						ServiceControllerStatus status = service.Status;

						while (status != ServiceControllerStatus.Running)
						{
							ServiceStatusText.Content = status.ToString();
							StopServiceButton.IsEnabled = false;
							StartServiceButton.IsEnabled = false;
							Thread.Sleep(1000);
							service.Refresh();
							status = service.Status;
						}

						if (status == ServiceControllerStatus.Running)
						{
							ServiceStatusText.Content = status.ToString();
							ServiceStatusText.Foreground = Brushes.Green;
							StopServiceButton.IsEnabled = true;
							StartServiceButton.IsEnabled = false;
						}
						else
						{
							ServiceStatusText.Content = status.ToString();
							ServiceStatusText.Foreground = Brushes.Red;
							StopServiceButton.IsEnabled = false;
							StartServiceButton.IsEnabled = true;
						}

					}
				}
			}
			catch(InvalidOperationException startError)
			{
				MessageBox.Show(@"Error: "+ startError.InnerException.ToString());
			}			
		}

		private string GetRecipientsFromStackPanel()
		{
			string recipientList = "";

			for (int i = 0; i < recipientStackPanel.Children.Count; i++)
			{
				System.Windows.Controls.Label label = (System.Windows.Controls.Label)recipientStackPanel.Children[i];
				recipientList += @"," + label.Content.ToString();
			}

			string finalList = recipientList.Remove(0,1);

			return finalList;

		}


		/// Verify if a service exists
		private bool ServiceExists(string ServiceName)
		{
			return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(ServiceName));
		}

		private void RefreshSenderAndRecipients(object sender, RoutedEventArgs e)
		{
			SenderPassword.Background = Brushes.White;
			SenderEmailAddress.Content = GetSenderEmail();
			SenderEmailAddress.Foreground = Brushes.Black;
			Recipients.Content = GetRecipientStackPanel();
			SenderPassword.Background = Brushes.White;
			FolderToMonitor.Foreground = Brushes.Black;

		}

	}
}
