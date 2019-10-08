using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderWatchGUI.Custom_Classes
{
	class Recipient
	{
		private string Name { get; set; }
		private string Email { get; set; }


		public Recipient(string name, string email)
		{
			Name = name;
			Email = email;
		}


	}
}
