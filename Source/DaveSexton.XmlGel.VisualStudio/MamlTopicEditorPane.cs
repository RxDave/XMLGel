using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DaveSexton.XmlGel.Maml;
using DaveSexton.XmlGel.Maml.Editors;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	/// <summary>
	/// This control host the editor (an extended RichTextBox) and is responsible for
	/// handling the commands targeted to the editor as well as saving and loading
	/// the document. This control also implement the search and replace functionalities.
	/// </summary>

	///////////////////////////////////////////////////////////////////////////////
	// Having an entry in the new file dialog.
	//
	// For our file type should appear under "General" in the new files dialog, we need the following:-
	//     - A .vsdir file in the same directory as NewFileItems.vsdir (generally under Common7\IDE\NewFileItems).
	//       In our case the file name is MAMLTopics.vsdir but we only require a file with .vsdir extension.
	//     - An empty aml file in the same directory as NewFileItems.vsdir. In
	//       our case we chose Topic.aml. Note this file name appears in MAMLTopics.vsdir
	//       (see vsdir file format below)
	//     - Three text strings in our language specific resource. File Resources.resx :-
	//          - "Rich Text file" - this is shown next to our icon.
	//          - "A blank rich text file" - shown in the description window
	//             in the new file dialog.
	//          - "Topic" - This is the base file name. New files will initially
	//             be named as Topic1.aml, Topic2.aml... etc.
	///////////////////////////////////////////////////////////////////////////////
	// MAMLTopics.vsdir
	// http://msdn.microsoft.com/en-us/library/2sc7ft4a(v=vs.110).aspx
	// 
	// The fields in order are as follows:-
	//    - Topic.aml - our empty aml file
	//    - {fd6a47a9-0b6d-4a3d-b71d-b7f7b856ba7d} - our MamlPackage package guid
	//    - #106 - Optional. This is the localizable name of the template and the name that appears in the Add Item dialog box. 
	//    - 80 - Required. An integer representing the sort order and relative priority of the template, with 1 being highest. 
	//    - #109 - Required. A localizable description of the template as it will appear in the Add Item dialog box when the item is selected.
	//    - 0 - resource dll string (we don't use this)
	//    - 401 - the ID of our icon
	//    - 0 - various flags (we don't use this - se vsshell.idl)
	//    - #107 - Required. The default (suggested) name for the new item, displayed in the Name field in the dialog box.
	///////////////////////////////////////////////////////////////////////////////

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	//This is required for Find In files scenario to work properly. This provides a connection point 
	//to the event interface
	[ComVisible(true)]
	public sealed partial class MamlTopicEditorPane : Microsoft.VisualStudio.Shell.WindowPane,
		IVsFileBackup		// Support backup of files.  Visual Studio File Recovery backs up all objects in the Running Document Table that support IVsFileBackup and have unsaved changes.
	{
		/// <summary>
		/// This is a required override from the Microsoft.VisualStudio.Shell.WindowPane class.
		/// It returns the extended rich text box that we host.
		/// </summary>
		public override object Content
		{
			get
			{
				return editor;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		private ITrackSelection TrackSelection
		{
			get
			{
				if (trackSelection == null)
				{
					trackSelection = (ITrackSelection) GetService(typeof(ITrackSelection));
				}

				return trackSelection;
			}
		}

		private const uint MyFormat = 0;
		private const string defaultFileExtension = MamlDocument.DefaultFileExtension;

		private readonly DispatcherTimer fileChangedTrigger = new DispatcherTimer();
		private readonly DispatcherTimer statusBarTrigger = new DispatcherTimer();
		private readonly MamlTopicEditor editor;
		private readonly DocumentProperties properties;
		private readonly MamlPackage package;

		private string fileName = string.Empty;
		// This flag is true when we are asking the QueryEditQuerySave service if we can edit the
		// file. It is used to avoid to have more than one request queued.
		private bool gettingCheckoutStatus;
		private Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer;
		private ITrackSelection trackSelection;
		private IVsFileChangeEx vsFileChangeEx;
		private bool fileChangedTimerSet;
		private int ignoreFileChangeLevel;
		private bool backupObsolete = true;
		private uint vsFileChangeCookie;
		private ArrayList textSpanArray = new ArrayList();

		/// <summary>
		/// Constructor that calls the Microsoft.VisualStudio.Shell.WindowPane constructor then
		/// our initialization functions.
		/// </summary>
		/// <param name="package">Our Package instance.</param>
		public MamlTopicEditorPane(MamlPackage package)
			: base(null)
		{
			this.package = package;

			editor = new MamlTopicEditor(this);

			fileChangedTrigger.Tick += fileChangedTrigger_Tick;
			fileChangedTrigger.Interval = TimeSpan.FromSeconds(1);

			statusBarTrigger.Tick += statusBarTrigger_Tick;
			statusBarTrigger.Interval = TimeSpan.FromSeconds(1);

			// Create the object that will show the document's properties
			// on the properties window.
			properties = new DocumentProperties(() => fileName, null);

			var selectables = new[] { properties };

			selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer(true, false);
			selectionContainer.SelectableObjects = selectables;
			selectionContainer.SelectedObjects = selectables;

			editor.TextBoxChanged += editor_TextBoxChanged;
			editor.DocumentChanged += editor_DocumentChanged;
			editor.DocumentContentChanged += editor_DocumentContentChanged;

			SetupCommands();
		}

		/// <summary>
		/// This function asks to the QueryEditQuerySave service if it is possible to
		/// edit the file.
		/// </summary>
		private bool CanEditFile()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t**** CanEditFile called ****"));

			// Check the status of the recursion guard
			if (gettingCheckoutStatus)
				return false;

			try
			{
				// Set the recursion guard
				gettingCheckoutStatus = true;

				// Get the QueryEditQuerySave service
				IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2) GetService(typeof(SVsQueryEditQuerySave));

				// Now call the QueryEdit method to find the edit status of this file
				string[] documents = { this.fileName };
				uint result;
				uint outFlags;

				// Note that this function can popup a dialog to ask the user to checkout the file.
				// When this dialog is visible, it is possible to receive other request to change
				// the file and this is the reason for the recursion guard.
				int hr = queryEditQuerySave.QueryEditFiles(
						0,              // Flags
						1,              // Number of elements in the array
						documents,      // Files to edit
						null,           // Input flags
						null,           // Input array of VSQEQS_FILE_ATTRIBUTE_DATA
						out result,     // result of the checkout
						out outFlags    // Additional flags
				);
				if (ErrorHandler.Succeeded(hr) && (result == (uint) tagVSQueryEditResult.QER_EditOK))
				{
					// In this case (and only in this case) we can return true from this function.
					return true;
				}
			}

			finally
			{
				gettingCheckoutStatus = false;
			}
			return false;
		}

		private void SetReadOnly(bool isFileReadOnly)
		{
			editor.IsReadOnly = isFileReadOnly;

			var frame = (IVsWindowFrame) GetService(typeof(SVsWindowFrame));

			string editorCaption = "";

			if (isFileReadOnly)
			{
				editorCaption = this.GetResourceString("@100");
			}

			ErrorHandler.ThrowOnFailure(frame.SetProperty((int) __VSFPROPID.VSFPROPID_EditorCaption, editorCaption));
			backupObsolete = true;
		}

		/// <summary>
		/// This method loads a localized string based on the specified resource.
		/// </summary>
		/// <param name="resourceName">Resource to load</param>
		/// <returns>String loaded for the specified resource</returns>
		private string GetResourceString(string resourceName)
		{
			string resourceValue;
			IVsResourceManager resourceManager = (IVsResourceManager) GetService(typeof(SVsResourceManager));
			if (resourceManager == null)
			{
				throw new InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method");
			}
			Guid packageGuid = package.GetType().GUID;
			int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
			return resourceValue;
		}

		/// <summary>
		/// Function that we use to display our context menu.  This function
		/// makes use of the IMenuCommandService's ShowContextMenu function.
		/// </summary>
		/// <param name="point"> The point that we want to display the context menu at.
		/// Note that this must be in screen coordinates.</param>
		private void DisplayContextMenuAt(Point point)
		{
			// Pass in the GUID:ID pair for the context menu.
			CommandID contextMenuID = new CommandID(GuidList.guidDaveSexton_XmlGel_VisualStudioCmdSet, PkgCmdIDList.IDMX_RTF);

			// Get the OleMenuCommandService from the package
			IMenuCommandService menuService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;

			if (null != menuService)
			{
				// Note: point must be in screen coordinates
				menuService.ShowContextMenu(contextMenuID, (int) point.X, (int) point.Y);
			}
		}

		/// <summary>
		/// This method is used to Persist the data to a single file. On a successful backup this 
		/// should clear up the backup dirty bit
		/// </summary>
		/// <param name="pszBackupFileName">Name of the file to persist</param>
		/// <returns>S_OK if the data can be successfully persisted.
		/// This should return STG_S_DATALOSS or STG_E_INVALIDCODEPAGE if there is no way to 
		/// persist to a file without data loss
		/// </returns>
		int IVsFileBackup.BackupFile(string pszBackupFileName)
		{
			try
			{
				editor.Save(pszBackupFileName);

				backupObsolete = false;
			}
			catch (ArgumentException)
			{
				return VSConstants.E_FAIL;
			}
			catch (IOException)
			{
				return VSConstants.E_FAIL;
			}

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Used to set the backup dirty bit. This bit should be set when the object is modified 
		/// and cleared on calls to BackupFile and any Save method
		/// </summary>
		/// <param name="pbObsolete">the dirty bit to be set</param>
		/// <returns>returns 1 if the backup dirty bit is set, 0 otherwise</returns>
		int IVsFileBackup.IsBackupFileObsolete(out int pbObsolete)
		{
			if (backupObsolete)
				pbObsolete = 1;
			else
				pbObsolete = 0;
			return VSConstants.S_OK;
		}

		protected override void OnClose()
		{
			// editor.StopRecorder();

			base.OnClose();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					editor.TextBoxChanged -= editor_TextBoxChanged;
					editor.DocumentChanged -= editor_DocumentChanged;
					editor.DocumentContentChanged -= editor_DocumentContentChanged;

					if (editor.TextBox != null)
					{
						editor.TextBox.GotFocus -= TextBox_GotFocus;
						editor.TextBox.MouseDown -= TextBox_MouseDown;
						editor.TextBox.SelectionChanged -= TextBox_SelectionChanged;
						editor.TextBox.KeyDown -= TextBox_KeyDown;
					}

					SetFileChangeNotification(null, false);

					if (extensibleObjectSite != null)
					{
						extensibleObjectSite.NotifyDelete(this);
						extensibleObjectSite = null;
					}

					GC.SuppressFinalize(this);
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// This event is triggered when one of the files loaded into the environment has changed outside of the
		/// editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void fileChangedTrigger_Tick(object sender, EventArgs e)
		{
			//Disable the timer
			fileChangedTrigger.Stop();

			string message = this.GetResourceString("@101");    //get the message string from the resource
			IVsUIShell VsUiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
			int result = 0;
			Guid tempGuid = Guid.Empty;
			if (VsUiShell != null)
			{
				//Show up a message box indicating that the file has changed outside of VS environment
				ErrorHandler.ThrowOnFailure(VsUiShell.ShowMessageBox(0, ref tempGuid, fileName, message, null, 0,
						OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
						OLEMSGICON.OLEMSGICON_QUERY, 0, out result));
			}
			//if the user selects "Yes", reload the current file
			if (result == (int) System.Windows.Forms.DialogResult.Yes)
			{
				ErrorHandler.ThrowOnFailure(((IVsPersistDocData) this).ReloadDocData(0));
			}

			fileChangedTimerSet = false;
		}

		private void statusBarTrigger_Tick(object sender, EventArgs e)
		{
			statusBarTrigger.Stop();

			ErrorHandler.ThrowOnFailure(((IVsStatusbarUser) this).SetInfo());
		}

		/// <summary>
		/// This is an extra command handler that we will use to intercept right
		/// mouse click events so that we can call our function to display the
		/// context menu.
		/// </summary>
		private void TextBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				var mouseDownLocation = e.GetPosition(editor.TextBox);
				var screenCoordinates = editor.TextBox.PointToScreen(mouseDownLocation);

				DisplayContextMenuAt(screenCoordinates);
			}
		}

		/// <summary>
		/// This is an extra command handler that we will use to check when the insert
		/// key is pressed.  Note that even if we detect that the insert key is pressed
		/// we are not setting the handled property to true, so other event handlers will
		/// also see it.
		/// </summary>
		/// <param name="sender"> Not used.</param>
		/// <param name="e"> KeyEventArgs instance that we will use to get the key that was pressed.</param>
		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			// If the key pressed is the insert key...
			if (e.Key == Key.Insert)
			{
				// Toggle our stored insert value
				this.editor.Overstrike = !this.editor.Overstrike;

				// Call the function to update the status bar insert mode
				SetStatusBarInsertMode();
			}
		}

		/// <summary>
		/// This event is triggered when the control's GotFocus event is fired.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBox_GotFocus(object sender, EventArgs e)
		{
			statusBarTrigger.Start();
		}

		/// <summary>
		/// This is an added command handler that will make it so the ITrackSelection.OnSelectChange
		/// function gets called whenever the cursor position is changed and also so the position 
		/// displayed on the status bar will update whenever the cursor position changes.
		/// </summary>
		/// <param name="sender"> Not used.</param>
		/// <param name="e"> Not used.</param>
		private void TextBox_SelectionChanged(object sender, EventArgs e)
		{
			// Call the function that will update the position displayed on the status bar.
			this.SetStatusBarPosition();

			// Now call the OnSelectChange function using our stored TrackSelection and
			// selectionContainer variables.
			ITrackSelection track = TrackSelection;
			if (null != track)
			{
				ErrorHandler.ThrowOnFailure(track.OnSelectChange((ISelectionContainer) selectionContainer));
			}
		}

		private void editor_TextBoxChanged(object sender, EventArgs e)
		{
			editor.TextBox.GotFocus += TextBox_GotFocus;
			editor.TextBox.MouseDown += TextBox_MouseDown;
			editor.TextBox.SelectionChanged += TextBox_SelectionChanged;
			editor.TextBox.KeyDown += TextBox_KeyDown;
		}

		private void editor_DocumentChanged(object sender, EventArgs e)
		{
			properties.Document = editor.Document;
		}

		private void editor_DocumentContentChanged(object sender, EventArgs e)
		{
			DocumentContentChanged();
		}
	}
}