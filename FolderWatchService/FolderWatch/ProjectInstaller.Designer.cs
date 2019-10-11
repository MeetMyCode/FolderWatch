namespace FolderWatch
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.FolderWatchServiceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.FolderWatchServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			// 
			// FolderWatchServiceProcessInstaller1
			// 
			this.FolderWatchServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.FolderWatchServiceProcessInstaller1.Password = null;
			this.FolderWatchServiceProcessInstaller1.Username = null;
			//this.FolderWatchServiceProcessInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.FolderWatchServiceProcessInstaller1_AfterInstall);
			// 
			// FolderWatchServiceInstaller1
			// 
			this.FolderWatchServiceInstaller1.DisplayName = "Folder Watch Service";
			this.FolderWatchServiceInstaller1.ServiceName = "FolderWatchService";
			//this.FolderWatchServiceInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.FolderWatchServiceInstaller1_AfterInstall);
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FolderWatchServiceProcessInstaller1,
            this.FolderWatchServiceInstaller1});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller FolderWatchServiceProcessInstaller1;
		private System.ServiceProcess.ServiceInstaller FolderWatchServiceInstaller1;
	}
}