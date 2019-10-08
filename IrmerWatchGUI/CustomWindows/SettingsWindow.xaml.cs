using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;
using IrmerWatchGUI.CustomViews;

namespace IrmerWatchGUI.CustomWindows
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			SetSenderNamAndEmail();
			PopulateRecipientStackPanel();
		}

		private void AddNotificationRecipient(object sender, RoutedEventArgs e)
		{
			NotificationRecipient NotificationRecipient = new NotificationRecipient();

			RecipientEmailsStack.Children.Add(NotificationRecipient);

		}

		private void SetSenderNamAndEmail()
		{
			SettingsPropertyCollection settings = Properties.SenderSettings.Default.Properties;

			foreach (SettingsProperty setting in settings)
			{
				SenderEmail.Text = setting.DefaultValue.ToString();
			}
		}

		private void PopulateRecipientStackPanel()
		{
			//First clear the stackpanel.
			RecipientEmailsStack.Children.Clear();

			SettingsPropertyCollection settings = Properties.RecipientSettings.Default.Properties;

			foreach (SettingsProperty setting in settings)
			{
				NotificationRecipient recipient = new NotificationRecipient();
				recipient.RecipientEmail.Text = setting.DefaultValue.ToString();
				RecipientEmailsStack.Children.Add(recipient);
			}
		}

		private void CloseWindow(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void SaveSettings(object sender, RoutedEventArgs e)
		{
			//clear old settings before replacing with new ones.
			Properties.RecipientSettings.Default.Properties.Clear(); 	   

			//save sender email
			Properties.SenderSettings.Default.Sender = SenderEmail.Text;

			//remove empty items
			RemoveEmptyEmailTextBoxes();

			//save recipient emails
			List<string> recipients = GetRecipientsFromStackPanel();

			int index = 1;
			foreach (string email in recipients)
			{
				SettingsProperty property = new SettingsProperty(@"Recipient" + index);
				property.DefaultValue = email;

				Properties.RecipientSettings.Default.Properties.Add(property);

				index++;
			}

			Properties.SenderSettings.Default.Save();
			Properties.RecipientSettings.Default.Save();

			MessageBox.Show(@"Saved!");

		}

		private void RemoveEmptyEmailTextBoxes()
		{
			for (int i = 0; i < RecipientEmailsStack.Children.Count; i++)
			{
				if ((RecipientEmailsStack.Children[i] as NotificationRecipient).RecipientEmail.Text == @"")
				{
					RecipientEmailsStack.Children.RemoveAt(i);
				}
			}
		}

		private List<string> GetRecipientsFromStackPanel()
		{
			List<string> RecipientEmailsArray = new List<string>();

			foreach (NotificationRecipient email in RecipientEmailsStack.Children)
			{
				if (email.RecipientEmail.Text != @"")
				{
					RecipientEmailsArray.Add(email.RecipientEmail.Text);
				}
			}

			return RecipientEmailsArray;
		}
	}
}
