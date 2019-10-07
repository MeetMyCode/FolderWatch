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

			PopulateRecipientStackPanel();
		}

		private void AddNotificationRecipient(object sender, RoutedEventArgs e)
		{
			NotificationRecipient NotificationRecipient = new NotificationRecipient();

			RecipientEmailsStack.Children.Add(NotificationRecipient);

		}

		private void PopulateRecipientStackPanel()
		{
			SettingsPropertyCollection settings = mySettings.Default.Properties;

			for (int i = 0; i < settings.Count; i++)
			{
				if (i == 0)
				{
					SenderName.Content = settings[i].Name;
					SenderEmail.Text = settings[i].DefaultValue;
				}
				else
				{
					NotificationRecipient recipient = new NotificationRecipient();
					recipient.RecipientName = settings[i].Name;
					recipient.RecipientEmail = settings[i].DefaultValue;
				}


			}
			



		}
	}
}
