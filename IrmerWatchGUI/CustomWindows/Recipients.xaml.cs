using System;
using System.Collections.Generic;
using System.IO;
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

namespace IrmerWatchGUI.CustomWindows
{
	/// <summary>
	/// Interaction logic for Recipients.xaml
	/// </summary>
	public partial class Recipients : Window
	{
		private List<Tuple<string, string>> messageRecipients = new List<Tuple<string, string>>();

		public Recipients()
		{
			InitializeComponent();

			messageRecipients = getRecipientData();

			PopulateRecipientPicker();

		}

		private void PopulateRecipientPicker()
		{
			foreach (Tuple<string, string> tuple in messageRecipients)
			{
				Label name = new Label();
				name.Content = tuple.Item1;
				name.Height = 30;
				VerticalAlignment = VerticalAlignment.Top;

				Label email = new Label();
				email.Content = tuple.Item2;
				email.Height = 30;
				VerticalAlignment = VerticalAlignment.Top;

				Button deleteButton = new Button();
				deleteButton.Content = "Remove";
				deleteButton.Background = Brushes.Red;
				deleteButton.Height = 30;
				VerticalAlignment = VerticalAlignment.Top;
				deleteButton.Click += DeleteButtonClicked;

				RowDefinition row = new RowDefinition();
				row.Height = new GridLength(35);
				RecipientGrid.RowDefinitions.Add(row);

				RecipientGrid.Children.Add(name);
				RecipientGrid.Children.Add(email);
				RecipientGrid.Children.Add(deleteButton);

				Grid.SetRow(name, RecipientGrid.RowDefinitions.Count - 1);
				Grid.SetRow(email, RecipientGrid.RowDefinitions.Count - 1);
				Grid.SetRow(deleteButton, RecipientGrid.RowDefinitions.Count - 1);

				Grid.SetColumn(name, 0);
				Grid.SetColumn(email, 1);
				Grid.SetColumn(deleteButton, 2);

			}
		}

		private void DeleteButtonClicked(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Button Clicked!");
		}

		private List<Tuple<string, string>> getRecipientData()
		{
			string line;
			string name;
			string email;
			List<Tuple<string, string>> recipientData = new List<Tuple<string, string>>();
			
			// Read the file and display it line by line.  
			StreamReader file = new StreamReader(@"./Settings/Recipients.txt");
			while ((line = file.ReadLine()) != null)
			{
				string[] recipient = line.Split(',');
				name = recipient[0].Trim();
				email = recipient[1].Trim();
				Tuple<string, string> tuple = Tuple.Create(name, email);
				recipientData.Add(tuple);
			}

			return recipientData;
		}


	}
}
