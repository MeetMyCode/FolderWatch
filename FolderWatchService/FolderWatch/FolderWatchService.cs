﻿using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FolderWatch
{
	public partial class FolderWatchService : ServiceBase
	{
		private static string SenderPassword;
		private static string SenderEmail;
		private static FileSystemWatcher xlsWatcher;
		private static FileSystemWatcher xlsxWatcher;
		private static bool ServiceStatus = false;
		private static string WatchedDirectory;

		private static List<string> recipients;


		public FolderWatchService(string[] args)
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			//startBatchTimer();

			try
			{
				//System.Diagnostics.Debugger.Launch();

				SenderEmail = args[0];
				SenderPassword = args[1];

				//Email Recipients
				string[] recipientList;
				string recipientString = args[2];
				recipientList = recipientString.Split(',');
				recipients = new List<string>(recipientList);

				//Get Directory to watch.
				WatchedDirectory = args[3];

			}
			catch (Exception e)
			{
				Console.WriteLine(@"Inner Exception: " + e.InnerException);
				Console.WriteLine(@"Data: " + e.Data);
				Console.WriteLine(@"Message: " + e.Message);
				Console.WriteLine(@"Target Site: " + e.TargetSite);

			}

			//Watches .xls files
			xlsWatcher = getWatcher("xls");

			//Watches .xlsx files
			xlsxWatcher = getWatcher("xlsx");

			//Inform User of Service Status
			ServiceStatus = true;
			InformOfServiceStoppingOrStarting(ServiceStatus);
		}

		protected override void OnStop()
		{
			//Stop Watching
			xlsWatcher.EnableRaisingEvents = false;
			xlsxWatcher.EnableRaisingEvents = false;

			//Inform User of Service Status
			ServiceStatus = false;
			InformOfServiceStoppingOrStarting(ServiceStatus);
		}

		private static FileSystemWatcher getWatcher(string fileExtension)
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = WatchedDirectory;
			watcher.NotifyFilter = NotifyFilters.FileName;
			watcher.Filter = string.Format("*.{0}", fileExtension);
			watcher.Created += new FileSystemEventHandler(OnChanged);
			watcher.Error += new ErrorEventHandler(OnError);
			watcher.EnableRaisingEvents = true;

			return watcher;
		}

		//Occurs when the instance of FileSystemWatcher is unable to continue monitoring changes 
		//or when the internal buffer overflows.
		//Source: https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher.error?view=netframework-4.7.2
		private static void OnError(object sender, ErrorEventArgs e) {

			FileSystemWatcher watcher = (FileSystemWatcher)sender;
			string fileExtension = watcher.Filter;

			switch (fileExtension)
			{
				case @"xls":
					xlsWatcher = new FileSystemWatcher(fileExtension);
					break;

				case @"xlsx":
					xlsWatcher = new FileSystemWatcher(fileExtension);
					break;

				default:
					string errorMsg = string.Format(@"Error: Unknown File Extension Received ({0}).", fileExtension);
					Console.WriteLine();
					break;
			}
		}
		
		private static void OnChanged(object sender, FileSystemEventArgs e)
		{
			string file = e.FullPath;
			EmailTheCreatedFile(file);
		}

		private static void InformOfServiceStoppingOrStarting(bool serviceStatus)
		{
			// Create instance of IEWSClient class by giving credentials
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
			service.Credentials = new WebCredentials(SenderEmail, SenderPassword);
			service.Url = new Uri("https://mail.nhs.net/ews/exchange.asmx");
			ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

			//**DON'T DELETE** Using AutodiscoverUrl seems to fail intermittently. Error of "Cannot locate AutoDiscover Service".
			//service.AutodiscoverUrl(OrdererEmailAddress, RedirectionUrlValidationCallback);

			EmailMessage email = new EmailMessage(service);
			email.ToRecipients.AddRange(new List<string> { SenderEmail });
			email.Subject = @"**TEST** FolderWatch Email";

			string emailBody;
			switch (serviceStatus)
			{
				case true:
					emailBody = @"**TEST** The FolderWatch Service has been STARTED.";
					break;

				case false:
					emailBody = string.Format(@"**TEST** The FolderWatch Service has been STOPPED or UNINSTALLED. {0}Please contact someone with Administrator Privileges to check that the service has been restarted/reinstalled.", Environment.NewLine);
					break;

				default:
					emailBody = string.Format(@"**TEST** The FolderWatch Service status is INVALID. Expecting true/false, received: {0}. {1}Please contact someone with Administrator Privileges to investigate.", serviceStatus, Environment.NewLine);
					break;
			}

			email.Body = new MessageBody(emailBody);
			email.Send();
		}

		private static void EmailTheCreatedFile(string file)
		{
			// Create instance of IEWSClient class by giving credentials
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
			service.Credentials = new WebCredentials(SenderEmail, SenderPassword);
			service.Url = new Uri("https://mail.nhs.net/ews/exchange.asmx");
			ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

			//**DON'T DELETE** Using AutodiscoverUrl seems to fail intermittently. Error of "Cannot locate AutoDiscover Service".
			//service.AutodiscoverUrl(OrdererEmailAddress, RedirectionUrlValidationCallback);
			EmailMessage email = new EmailMessage(service);
			email.ToRecipients.AddRange(recipients);
			email.Subject = @"**TEST** FolderWatch Email";
			string emailBody = @"**TEST** FolderWatch Email: Please see attached.";
			email.Body = new MessageBody(emailBody);
			email.Attachments.AddFileAttachment(file);
			email.Send();
		}

		private static bool CertificateValidationCallBack(object sender,
	System.Security.Cryptography.X509Certificates.X509Certificate certificate,
	System.Security.Cryptography.X509Certificates.X509Chain chain,
	System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
			{
				return true;
			}

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
			{
				if (chain != null && chain.ChainStatus != null)
				{
					foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
					{
						if ((certificate.Subject == certificate.Issuer) &&
						   (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
						{
							// Self-signed certificates with an untrusted root are valid. 
							continue;
						}
						else
						{
							if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
							{
								// If there are any other errors in the certificate chain, the certificate is invalid,
								// so the method returns false.
								return false;
							}
						}
					}
				}

				// When processing reaches this line, the only errors in the certificate chain are 
				// untrusted root errors for self-signed certificates. These certificates are valid
				// for default Exchange server installations, so return true.
				return true;
			}
			else
			{
				// In all other cases, return false.
				return false;
			}
		}

	}
}


