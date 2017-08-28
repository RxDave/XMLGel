using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditorPane :
		IVsFileChangeEvents,	// Notify the client when file changes on disk
		IVsDocDataFileChangeControl,	// Determine whether changes to files made outside of the editor should be ignored
		IPersistFileFormat,		// Enables the programmatic loading or saving of an object in a format specified by the user
		IVsPersistDocData			// Enables persistence functionality for document data
	{
		/// <summary>
		/// Returns whether the contents of file have changed since the last save.
		/// </summary>
		public bool DocumentHasUnsavedChanges
		{
			get
			{
				return isDirty;
			}
		}

		// Flag true when we are loading the file. It is used to avoid to change the isDirty flag
		// when the changes are related to the load operation.
		private bool loading;
		private bool isDirty;

		private void DocumentContentChanged()
		{
			// During the load operation the text of the control will change, but
			// this change must not be stored in the status of the document.
			if (!loading)
			{
				// The only interesting case is when we are changing the document
				// for the first time
				if (!isDirty)
				{
					// Check if the QueryEditQuerySave service allow us to change the file
					if (!CanEditFile())
					{
						// We can not change the file (e.g. a checkout operation failed),
						// so undo the change and exit.
						editor.TextBox.Undo();
						return;
					}

					// It is possible to change the file, so update the status.
					isDirty = true;

					var track = TrackSelection;

					if (null != track)
					{
						// Note: here we don't need to check the return code.
						track.OnSelectChange((ISelectionContainer) selectionContainer);
					}

					backupObsolete = true;
				}
			}
		}

		/// <summary>
		/// Gets an instance of the RunningDocumentTable (RDT) service which manages the set of currently open 
		/// documents in the environment and then notifies the client that an open document has changed
		/// </summary>
		private void NotifyDocumentChanged()
		{
			// Make sure that we have a file name
			if (fileName.Length == 0)
				return;

			// Get a reference to the Running Document Table
			IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable) GetService(typeof(SVsRunningDocumentTable));

			uint docCookie;
			IVsHierarchy hierarchy;
			uint itemID;
			IntPtr docData = IntPtr.Zero;

			try
			{
				// Lock the document
				int hr = runningDocTable.FindAndLockDocument(
						(uint) _VSRDTFLAGS.RDT_ReadLock,
						fileName,
						out hierarchy,
						out itemID,
						out docData,
						out docCookie
				);

				ErrorHandler.ThrowOnFailure(hr);

				// Send the notification
				hr = runningDocTable.NotifyDocumentChanged(docCookie, (uint) __VSRDTATTRIB.RDTA_DocDataReloaded);

				// Unlock the document.
				// Note that we have to unlock the document even if the previous call failed.
				ErrorHandler.ThrowOnFailure(runningDocTable.UnlockDocument((uint) _VSRDTFLAGS.RDT_ReadLock, docCookie));

				// Check if the call to NotifyDocumentChanged failed.
				ErrorHandler.ThrowOnFailure(hr);
			}
			finally
			{
				if (docData != IntPtr.Zero)
					Marshal.Release(docData);
			}
		}

		/// <summary>
		/// In this function we inform the shell when we wish to receive 
		/// events when our file is changed or we inform the shell when 
		/// we wish not to receive events anymore.
		/// </summary>
		/// <param name="pszFileName">File name string</param>
		/// <param name="fStart">TRUE indicates advise, FALSE indicates unadvise.</param>
		/// <returns>Result of the operation</returns>
		private int SetFileChangeNotification(string pszFileName, bool fStart)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside SetFileChangeNotification ****"));

			int result = VSConstants.E_FAIL;

			//Get the File Change service
			if (null == vsFileChangeEx)
				vsFileChangeEx = (IVsFileChangeEx) GetService(typeof(SVsFileChangeEx));
			if (null == vsFileChangeEx)
				return VSConstants.E_UNEXPECTED;

			// Setup Notification if fStart is TRUE, Remove if fStart is FALSE.
			if (fStart)
			{
				if (vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
				{
					//Receive notifications if either the attributes of the file change or 
					//if the size of the file changes or if the last modified time of the file changes
					result = vsFileChangeEx.AdviseFileChange(pszFileName,
							(uint) (_VSFILECHANGEFLAGS.VSFILECHG_Attr | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time),
							(IVsFileChangeEvents) this,
							out vsFileChangeCookie);
					if (vsFileChangeCookie == VSConstants.VSCOOKIE_NIL)
						return VSConstants.E_FAIL;
				}
			}
			else
			{
				if (vsFileChangeCookie != VSConstants.VSCOOKIE_NIL)
				{
					result = vsFileChangeEx.UnadviseFileChange(vsFileChangeCookie);
					vsFileChangeCookie = VSConstants.VSCOOKIE_NIL;
				}
			}
			return result;
		}

		/// <summary>
		/// In this function we suspend receiving file change events for
		/// a file or we reinstate a previously suspended file depending
		/// on the value of the given fSuspend flag.
		/// </summary>
		/// <param name="pszFileName">File name string</param>
		/// <param name="fSuspend">TRUE indicates that the events needs to be suspended</param>
		/// <returns></returns>
		private int SuspendFileChangeNotification(string pszFileName, int fSuspend)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside SuspendFileChangeNotification ****"));

			if (null == vsFileChangeEx)
				vsFileChangeEx = (IVsFileChangeEx) GetService(typeof(SVsFileChangeEx));
			if (null == vsFileChangeEx)
				return VSConstants.E_UNEXPECTED;

			if (0 == fSuspend)
			{
				// we are transitioning from suspended to non-suspended state - so force a
				// sync first to avoid asynchronous notifications of our own change
				if (vsFileChangeEx.SyncFile(pszFileName) == VSConstants.E_FAIL)
					return VSConstants.E_FAIL;
			}

			//If we use the VSCOOKIE parameter to specify the file, then pszMkDocument parameter 
			//must be set to a null reference and vice versa 
			return vsFileChangeEx.IgnoreFile(vsFileChangeCookie, null, fSuspend);
		}

		#region IVsDocDataFileChangeControl
		/// <summary>
		/// Used to determine whether changes to DocData in files should be ignored or not
		/// </summary>
		/// <param name="fIgnore">a non zero value indicates that the file changes should be ignored
		/// </param>
		/// <returns></returns>
		int IVsDocDataFileChangeControl.IgnoreFileChanges(int fIgnore)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t **** Inside IgnoreFileChanges ****"));

			if (fIgnore != 0)
			{
				ignoreFileChangeLevel++;
			}
			else
			{
				if (ignoreFileChangeLevel > 0)
					ignoreFileChangeLevel--;

				// We need to check here if our file has changed from "Read Only"
				// to "Read/Write" or vice versa while the ignore level was non-zero.
				// This may happen when a file is checked in or out under source
				// code control. We need to check here so we can update our caption.
				FileAttributes fileAttrs = File.GetAttributes(fileName);
				int isReadOnly = (int) fileAttrs & (int) FileAttributes.ReadOnly;
				SetReadOnly(isReadOnly != 0);
			}
			return VSConstants.S_OK;
		}
		#endregion

		#region IVsFileChangeEvents
		/// <summary>
		/// Notify the editor of the changes made to one or more files
		/// </summary>
		/// <param name="cChanges">Number of files that have changed</param>
		/// <param name="rgpszFile">array of the files names that have changed</param>
		/// <param name="rggrfChange">Array of the flags indicating the type of changes</param>
		/// <returns></returns>
		int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t**** Inside FilesChanged ****"));

			//check the different parameters
			if (0 == cChanges || null == rgpszFile || null == rggrfChange)
				return VSConstants.E_INVALIDARG;

			//ignore file changes if we are in that mode
			if (ignoreFileChangeLevel != 0)
				return VSConstants.S_OK;

			for (uint i = 0; i < cChanges; i++)
			{
				if (!String.IsNullOrEmpty(rgpszFile[i]) && String.Compare(rgpszFile[i], fileName, true, CultureInfo.CurrentCulture) == 0)
				{
					// if the readonly state (file attributes) have changed we can immediately update
					// the editor to match the new state (either readonly or not readonly) immediately
					// without prompting the user.
					if (0 != (rggrfChange[i] & (int) _VSFILECHANGEFLAGS.VSFILECHG_Attr))
					{
						FileAttributes fileAttrs = File.GetAttributes(fileName);
						int isReadOnly = (int) fileAttrs & (int) FileAttributes.ReadOnly;
						SetReadOnly(isReadOnly != 0);
					}
					// if it looks like the file contents have changed (either the size or the modified
					// time has changed) then we need to prompt the user to see if we should reload the
					// file. it is important to not synchronously reload the file inside of this FilesChanged
					// notification. first it is possible that there will be more than one FilesChanged 
					// notification being sent (sometimes you get separate notifications for file attribute
					// changing and file size/time changing). also it is the preferred UI style to not
					// prompt the user until the user re-activates the environment application window.
					// this is why we use a timer to delay prompting the user.
					if (0 != (rggrfChange[i] & (int) (_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size)))
					{
						if (!fileChangedTimerSet)
						{
							fileChangedTimerSet = true;
							fileChangedTrigger.Start();
						}
					}
				}
			}

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Notify the editor of the changes made to a directory
		/// </summary>
		/// <param name="pszDirectory">Name of the directory that has changed</param>
		/// <returns></returns>
		int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
		{
			//Nothing to do here
			return VSConstants.S_OK;
		}
		#endregion

		#region IPersistFileFormat
		int Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID(out Guid pClassID)
		{
			pClassID = GuidList.guidDaveSexton_XmlGel_VisualStudioEditorFactory;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Notifies the object that it has concluded the Save transaction
		/// </summary>
		/// <param name="pszFilename">Pointer to the file name</param>
		/// <returns>S_OK if the function succeeds</returns>
		int IPersistFileFormat.SaveCompleted(string pszFilename)
		{
			// TODO:  Add Editor.SaveCompleted implementation
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Returns the path to the object's current working file 
		/// </summary>
		/// <param name="ppszFilename">Pointer to the file name</param>
		/// <param name="pnFormatIndex">Value that indicates the current format of the file as a zero based index
		/// into the list of formats. Since we support only a single format, we need to return zero. 
		/// Subsequently, we will return a single element in the format list through a call to GetFormatList.</param>
		/// <returns></returns>
		int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
		{
			// We only support 1 format so return its index
			pnFormatIndex = MyFormat;
			ppszFilename = fileName;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Initialization for the object 
		/// </summary>
		/// <param name="nFormatIndex">Zero based index into the list of formats that indicates the current format 
		/// of the file</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.InitNew(uint nFormatIndex)
		{
			if (nFormatIndex != MyFormat)
			{
				return VSConstants.E_INVALIDARG;
			}
			// until someone change the file, we can consider it not dirty as
			// the user would be annoyed if we prompt him to save an empty file
			isDirty = false;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Returns the class identifier of the editor type
		/// </summary>
		/// <param name="pClassID">pointer to the class identifier</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.GetClassID(out Guid pClassID)
		{
			ErrorHandler.ThrowOnFailure(((Microsoft.VisualStudio.OLE.Interop.IPersist) this).GetClassID(out pClassID));
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Provides the caller with the information necessary to open the standard common "Save As" dialog box. 
		/// This returns an enumeration of supported formats, from which the caller selects the appropriate format. 
		/// Each string for the format is terminated with a newline (\n) character. 
		/// The last string in the buffer must be terminated with the newline character as well. 
		/// The first string in each pair is a display string that describes the filter, such as "Text Only 
		/// (*.txt)". The second string specifies the filter pattern, such as "*.txt". To specify multiple filter 
		/// patterns for a single display string, use a semicolon to separate the patterns: "*.htm;*.html;*.asp". 
		/// A pattern string can be a combination of valid file name characters and the asterisk (*) wildcard character. 
		/// Do not include spaces in the pattern string. The following string is an example of a file pattern string: 
		/// "HTML File (*.htm; *.html; *.asp)\n*.htm;*.html;*.asp\nText File (*.txt)\n*.txt\n."
		/// </summary>
		/// <param name="ppszFormatList">Pointer to a string that contains pairs of format filter strings</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.GetFormatList(out string ppszFormatList)
		{
			char Endline = (char) '\n';
			string FormatList = string.Format(CultureInfo.InvariantCulture, "My Editor (*{0}){1}*{0}{1}{1}", defaultFileExtension, Endline);
			ppszFormatList = FormatList;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Loads the file content into the textbox
		/// </summary>
		/// <param name="pszFilename">Pointer to the full path name of the file to load</param>
		/// <param name="grfMode">file format mode</param>
		/// <param name="fReadOnly">determines if the file should be opened as read only</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
		{
			if (pszFilename == null)
			{
				return VSConstants.E_INVALIDARG;
			}

			loading = true;
			int hr = VSConstants.S_OK;
			try
			{
				// Show the wait cursor while loading the file
				IVsUIShell VsUiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
				if (VsUiShell != null)
				{
					// Note: we don't want to throw or exit if this call fails, so
					// don't check the return code.
					hr = VsUiShell.SetWaitCursor();
				}

				editor.Load(pszFilename);

				isDirty = false;

				//Determine if the file is read only on the file system
				FileAttributes fileAttrs = File.GetAttributes(pszFilename);

				int isReadOnly = (int) fileAttrs & (int) FileAttributes.ReadOnly;

				//Set readonly if either the file is readonly for the user or on the file system
				if (0 == isReadOnly && 0 == fReadOnly)
					SetReadOnly(false);
				else
					SetReadOnly(true);


				// Notify to the property window that some of the selected objects are changed
				ITrackSelection track = TrackSelection;
				if (null != track)
				{
					hr = track.OnSelectChange((ISelectionContainer) selectionContainer);
					if (ErrorHandler.Failed(hr))
						return hr;
				}

				// Hook up to file change notifications
				if (String.IsNullOrEmpty(fileName) || 0 != String.Compare(fileName, pszFilename, true, CultureInfo.CurrentCulture))
				{
					fileName = pszFilename;
					SetFileChangeNotification(pszFilename, true);

					// Notify the load or reload
					NotifyDocumentChanged();
				}
			}
			finally
			{
				loading = false;
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Determines whether an object has changed since being saved to its current file
		/// </summary>
		/// <param name="pfIsDirty">true if the document has changed</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.IsDirty(out int pfIsDirty)
		{
			if (isDirty)
			{
				pfIsDirty = 1;
			}
			else
			{
				pfIsDirty = 0;
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Save the contents of the textbox into the specified file. If doing the save on the same file, we need to
		/// suspend notifications for file changes during the save operation.
		/// </summary>
		/// <param name="pszFilename">Pointer to the file name. If the pszFilename parameter is a null reference 
		/// we need to save using the current file
		/// </param>
		/// <param name="remember">Boolean value that indicates whether the pszFileName parameter is to be used 
		/// as the current working file.
		/// If remember != 0, pszFileName needs to be made the current file and the dirty flag needs to be cleared after the save.
		///                   Also, file notifications need to be enabled for the new file and disabled for the old file 
		/// If remember == 0, this save operation is a Save a Copy As operation. In this case, 
		///                   the current file is unchanged and dirty flag is not cleared
		/// </param>
		/// <param name="nFormatIndex">Zero based index into the list of formats that indicates the format in which 
		/// the file will be saved</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
		{
			int hr = VSConstants.S_OK;
			bool doingSaveOnSameFile = false;
			// If file is null or same --> SAVE
			if (pszFilename == null || pszFilename == fileName)
			{
				fRemember = 1;
				doingSaveOnSameFile = true;
			}

			//Suspend file change notifications for only Save since we don't have notifications setup
			//for SaveAs and SaveCopyAs (as they are different files)
			if (doingSaveOnSameFile)
				this.SuspendFileChangeNotification(pszFilename, 1);

			try
			{
				editor.Save(pszFilename);
			}
			catch (ArgumentException)
			{
				hr = VSConstants.E_FAIL;
			}
			catch (IOException)
			{
				hr = VSConstants.E_FAIL;
			}
			finally
			{
				//restore the file change notifications
				if (doingSaveOnSameFile)
					this.SuspendFileChangeNotification(pszFilename, 0);
			}

			if (VSConstants.E_FAIL == hr)
				return hr;

			//Save and Save as
			if (fRemember != 0)
			{
				//Save as
				if (null != pszFilename && !fileName.Equals(pszFilename))
				{
					SetFileChangeNotification(fileName, false); //remove notification from old file
					SetFileChangeNotification(pszFilename, true); //add notification for new file
					fileName = pszFilename;     //cache the new file name
				}
				isDirty = false;
				SetReadOnly(false);             //set read only to false since you were successfully able
				//to save to the new file                                                    
			}

			ITrackSelection track = TrackSelection;
			if (null != track)
			{
				hr = track.OnSelectChange((ISelectionContainer) selectionContainer);
			}

			// Since all changes are now saved properly to disk, there's no need for a backup.
			backupObsolete = false;
			return hr;
		}
		#endregion

		#region IVsPersistDocData
		/// <summary>
		/// Used to determine if the document data has changed since the last time it was saved
		/// </summary>
		/// <param name="pfDirty">Will be set to 1 if the data has changed</param>
		/// <returns>S_OK if the function succeeds</returns>
		int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
		{
			return ((IPersistFileFormat) this).IsDirty(out pfDirty);
		}

		/// <summary>
		/// Saves the document data. Before actually saving the file, we first need to indicate to the environment
		/// that a file is about to be saved. This is done through the "SVsQueryEditQuerySave" service. We call the
		/// "QuerySaveFile" function on the service instance and then proceed depending on the result returned as follows:
		/// If result is QSR_SaveOK - We go ahead and save the file and the file is not read only at this point.
		/// If result is QSR_ForceSaveAs - We invoke the "Save As" functionality which will bring up the Save file name 
		///                                dialog 
		/// If result is QSR_NoSave_Cancel - We cancel the save operation and indicate that the document could not be saved
		///                                by setting the "pfSaveCanceled" flag
		/// If result is QSR_NoSave_Continue - Nothing to do here as the file need not be saved
		/// </summary>
		/// <param name="dwSave">Flags which specify the file save options:
		/// VSSAVE_Save        - Saves the current file to itself.
		/// VSSAVE_SaveAs      - Prompts the User for a filename and saves the file to the file specified.
		/// VSSAVE_SaveCopyAs  - Prompts the user for a filename and saves a copy of the file with a name specified.
		/// VSSAVE_SilentSave  - Saves the file without prompting for a name or confirmation.  
		/// </param>
		/// <param name="pbstrMkDocumentNew">Pointer to the path to the new document</param>
		/// <param name="pfSaveCanceled">value 1 if the document could not be saved</param>
		/// <returns></returns>
		int IVsPersistDocData.SaveDocData(Microsoft.VisualStudio.Shell.Interop.VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
		{
			pbstrMkDocumentNew = null;
			pfSaveCanceled = 0;
			int hr = VSConstants.S_OK;

			switch (dwSave)
			{
				case VSSAVEFLAGS.VSSAVE_Save:
				case VSSAVEFLAGS.VSSAVE_SilentSave:
					{
						IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2) GetService(typeof(SVsQueryEditQuerySave));

						// Call QueryEditQuerySave
						uint result = 0;
						hr = queryEditQuerySave.QuerySaveFile(
										fileName,        // filename
										0,    // flags
										null,            // file attributes
										out result);    // result
						if (ErrorHandler.Failed(hr))
							return hr;

						// Process according to result from QuerySave
						switch ((tagVSQuerySaveResult) result)
						{
							case tagVSQuerySaveResult.QSR_NoSave_Cancel:
								// Note that this is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
								// two tags have the same value.
								pfSaveCanceled = ~0;
								break;

							case tagVSQuerySaveResult.QSR_SaveOK:
								{
									// Call the shell to do the save for us
									IVsUIShell uiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
									hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat) this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
									if (ErrorHandler.Failed(hr))
										return hr;
								}
								break;

							case tagVSQuerySaveResult.QSR_ForceSaveAs:
								{
									// Call the shell to do the SaveAS for us
									IVsUIShell uiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
									hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, (IPersistFileFormat) this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
									if (ErrorHandler.Failed(hr))
										return hr;
								}
								break;

							case tagVSQuerySaveResult.QSR_NoSave_Continue:
								// In this case there is nothing to do.
								break;

							default:
								throw new NotSupportedException("Unsupported result from QEQS");
						}
						break;
					}
				case VSSAVEFLAGS.VSSAVE_SaveAs:
				case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
					{
						// Make sure the file name as the right extension
						if (String.Compare(defaultFileExtension, System.IO.Path.GetExtension(fileName), true, CultureInfo.CurrentCulture) != 0)
						{
							fileName += defaultFileExtension;
						}
						// Call the shell to do the save for us
						IVsUIShell uiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
						hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat) this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
						if (ErrorHandler.Failed(hr))
							return hr;
						break;
					}
				default:
					throw new ArgumentException("Unsupported Save flag");
			};

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Loads the document data from the file specified
		/// </summary>
		/// <param name="pszMkDocument">Path to the document file which needs to be loaded</param>
		/// <returns>S_Ok if the method succeeds</returns>
		int IVsPersistDocData.LoadDocData(string pszMkDocument)
		{
			return ((IPersistFileFormat) this).Load(pszMkDocument, 0, 0);
		}

		/// <summary>
		/// Used to set the initial name for unsaved, newly created document data
		/// </summary>
		/// <param name="pszDocDataPath">String containing the path to the document. We need to ignore this parameter
		/// </param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
		{
			return ((IPersistFileFormat) this).InitNew(MyFormat);
		}

		/// <summary>
		/// Returns the Guid of the editor factory that created the IVsPersistDocData object
		/// </summary>
		/// <param name="pClassID">Pointer to the class identifier of the editor type</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
		{
			return ((IPersistFileFormat) this).GetClassID(out pClassID);
		}

		/// <summary>
		/// Close the IVsPersistDocData object
		/// </summary>
		/// <returns>S_OK if the function succeeds</returns>
		int IVsPersistDocData.Close()
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Determines if it is possible to reload the document data
		/// </summary>
		/// <param name="pfReloadable">set to 1 if the document can be reloaded</param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
		{
			// Allow file to be reloaded
			pfReloadable = 1;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Renames the document data
		/// </summary>
		/// <param name="grfAttribs"></param>
		/// <param name="pHierNew"></param>
		/// <param name="itemidNew"></param>
		/// <param name="pszMkDocumentNew"></param>
		/// <returns></returns>
		int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
		{
			// TODO:  Add MamlTopicEditorPane.RenameDocData implementation
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Reloads the document data
		/// </summary>
		/// <param name="grfFlags">Flag indicating whether to ignore the next file change when reloading the document data.
		/// This flag should not be set for us since we implement the "IVsDocDataFileChangeControl" interface in order to 
		/// indicate ignoring of file changes
		/// </param>
		/// <returns>S_OK if the method succeeds</returns>
		int IVsPersistDocData.ReloadDocData(uint grfFlags)
		{
			return ((IPersistFileFormat) this).Load(fileName, grfFlags, 0);
		}

		/// <summary>
		/// Called by the Running Document Table when it registers the document data. 
		/// </summary>
		/// <param name="docCookie">Handle for the document to be registered</param>
		/// <param name="pHierNew">Pointer to the IVsHierarchy interface</param>
		/// <param name="itemidNew">Item identifier of the document to be registered from VSITEM</param>
		/// <returns></returns>
		int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
		{
			//Nothing to do here
			return VSConstants.S_OK;
		}
		#endregion
	}
}