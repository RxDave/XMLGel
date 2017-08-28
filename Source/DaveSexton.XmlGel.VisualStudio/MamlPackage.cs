using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using DaveSexton.XmlGel.Maml;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
	// a package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	// This attribute is used to register the information needed to show this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	// This attribute registers a tool window exposed by this package.
	[ProvideToolWindow(typeof(TopicExplorerToolWindow))]
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = VSConstants.CLSID.MiscellaneousFilesProject_string)]
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = VSConstants.CLSID.SolutionFolderProject_string)]
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{7CF6DF6D-3B04-46f8-A40B-537D21BCA0B4}")]		// SHFB
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")]		// C#
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}")]		// VB
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}")]		// F#
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}")]		// C++
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")]		// Web Site
	[ProvideEditorExtension(typeof(MamlTopicEditorFactory), MamlDocument.DefaultFileExtension, priority: 0x61, NameResourceID = 105, DefaultName = "MAML Topic", TemplateDir = "Templates", ProjectGuid = "{349C5851-65DF-11DA-9384-00065B846F21}")]		// Web App
	// Our Editor supports Find and Replace therefore we need to declare support for LOGVIEWID_TextView.
	// This attribute declares that your MamlTopicEditorPane class implements IVsCodeWindow interface
	// used to navigate to find results from a "Find in Files" type of operation.
	//[ProvideEditorLogicalView(typeof(MamlTopicEditorFactory), VSConstants.LOGVIEWID.TextView_string)]
	[ProvideKeyBindingTable(GuidList.guidDaveSexton_XmlGel_VisualStudioEditorFactoryString, 102)]
	[Guid(GuidList.guidDaveSexton_XmlGel_VisualStudioPkgString)]
	public sealed class MamlPackage : Package
	{
		private bool servicesAvailable;

		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public MamlPackage()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
		}

		internal void Log(string format, params object[] args)
		{
			if (!servicesAvailable)
			{
				throw new InvalidOperationException("The VS activity log is unavailable until the package is initialized.");
			}

			var log = GetService(typeof(SVsActivityLog)) as IVsActivityLog;

			if (log != null)
			{
				log.LogEntry((uint) __ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
						this.ToString(),
						string.Format(CultureInfo.CurrentCulture, format, args));
			}
		}

		/// <summary>
		/// This function is called when the user clicks the menu item that shows the 
		/// tool window. See the Initialize method to see how the menu item is associated to 
		/// this function using the OleMenuCommandService service and the MenuCommand class.
		/// </summary>
		private void ShowToolWindow(object sender, EventArgs e)
		{
			// Get the instance number 0 of this tool window. This window is single instance so this instance
			// is actually the only one.
			// The last flag is set to true so that if the tool window does not exists it will be created.
			ToolWindowPane window = this.FindToolWindow(typeof(TopicExplorerToolWindow), 0, true);
			if ((null == window) || (null == window.Frame))
			{
				throw new NotSupportedException(Resources.CanNotCreateWindow);
			}
			IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			servicesAvailable = true;

			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

			base.Initialize();

			//Create Editor Factory. Note that the base Package class will call Dispose on it.
			base.RegisterEditorFactory(new MamlTopicEditorFactory(this));

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				// Create the command for the tool window
				CommandID toolwndCommandID = new CommandID(GuidList.guidDaveSexton_XmlGel_VisualStudioCmdSet, (int) PkgCmdIDList.cmdidMamlTopicExplorer);
				MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
				mcs.AddCommand(menuToolWin);
			}
		}
	}
}