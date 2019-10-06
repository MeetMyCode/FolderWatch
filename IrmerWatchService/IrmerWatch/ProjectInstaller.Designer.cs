namespace IrmerWatch
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
			this.IrmerWatchServiceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.IrmerWatchServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			// 
			// IrmerWatchServiceProcessInstaller1
			// 
			this.IrmerWatchServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.IrmerWatchServiceProcessInstaller1.Password = null;
			this.IrmerWatchServiceProcessInstaller1.Username = null;
			//this.IrmerWatchServiceProcessInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.IrmerWatchServiceProcessInstaller1_AfterInstall);
			// 
			// IrmerWatchServiceInstaller1
			// 
			this.IrmerWatchServiceInstaller1.DisplayName = "Irmer Watch Service";
			this.IrmerWatchServiceInstaller1.ServiceName = "IrmerWatchService";
			//this.IrmerWatchServiceInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.IrmerWatchServiceInstaller1_AfterInstall);
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.IrmerWatchServiceProcessInstaller1,
            this.IrmerWatchServiceInstaller1});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller IrmerWatchServiceProcessInstaller1;
		private System.ServiceProcess.ServiceInstaller IrmerWatchServiceInstaller1;
	}
}