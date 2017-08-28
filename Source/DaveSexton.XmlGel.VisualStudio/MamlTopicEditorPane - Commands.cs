using System;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditorPane
	{
		// private static string[] fontSizeArray = { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };
		// private string[] fontListArray;

		private void SetupCommands()
		{
			// Now get the IMenuCommandService; this object is the one
			// responsible for handling the collection of commands implemented by the package.

			IMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (null != mcs)
			{
				// Now create one object derived from MenuCommnad for each command defined in
				// the CTC file and add it to the command service.

				// For each command we have to define its id that is a unique Guid/integer pair, then
				// create the OleMenuCommand object for this command. The EventHandler object is the
				// function that will be called when the user will select the command. Then we add the 
				// OleMenuCommand to the menu service.  The AddCommand helper function does all this for us.

				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.SelectAll,
												new EventHandler(onSelectAll), null);
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Copy,
												new EventHandler(onCopy), new EventHandler(onQueryCopy));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Cut,
												new EventHandler(onCut), new EventHandler(onQueryCutOrDelete));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Paste,
												new EventHandler(onPaste), new EventHandler(onQueryPaste));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Delete,
												new EventHandler(onDelete), new EventHandler(onQueryCutOrDelete));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Undo,
												new EventHandler(onUndo), new EventHandler(onQueryUndo));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Redo,
												new EventHandler(onRedo), new EventHandler(onQueryRedo));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Bold,
												new EventHandler(onBold), new EventHandler(onQueryBold));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Italic,
												new EventHandler(onItalic), new EventHandler(onQueryItalic));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.Underline,
												new EventHandler(onUnderline), new EventHandler(onQueryUnderline));
				AddCommand(mcs, GuidList.guidDaveSexton_XmlGel_VisualStudioCmdSet, (int) PkgCmdIDList.icmdStrike,
												new EventHandler(onStrikethrough), new EventHandler(onQueryStrikethrough));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.JustifyCenter,
												new EventHandler(onJustifyCenter), new EventHandler(onQueryJustifyCenter));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.JustifyLeft,
												new EventHandler(onJustifyLeft), new EventHandler(onQueryJustifyLeft));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.JustifyRight,
												new EventHandler(onJustifyRight), new EventHandler(onQueryJustifyRight));
				AddCommand(mcs, VSConstants.VSStd2K, (int) VSConstants.VSStd2KCmdID.BULLETEDLIST,
												new EventHandler(onBulletedList), new EventHandler(onQueryBulletedList));

				//AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.FontNameGetList,
				//								new EventHandler(onFontNameGetList), null);
				//AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.FontName,
				//								new EventHandler(onFontName), null);
				//AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.FontSizeGetList,
				//								new EventHandler(onFontSizeGetList), null);
				//AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.FontSize,
				//								new EventHandler(onFontSize), null);

				// Support clipboard rings
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.PasteNextTBXCBItem,
												new EventHandler(onPasteNextTBXCBItem), new EventHandler(onQueryPasteNextTBXCBItem));

				// These two commands enable Visual Studio's default undo/redo toolbar buttons.  When these
				// buttons are clicked it triggers a multi-level undo/redo (even when we are undoing/redoing
				// only one action.  Note that we are not implementing the multi-level undo/redo functionality,
				// we are just adding a handler for this command so these toolbar buttons are enabled (Note that
				// we are just reusing the undo/redo command handlers).  To implement multi-level functionality
				// we would need to properly handle these two commands as well as MultiLevelUndoList and
				// MultiLevelRedoList.
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.MultiLevelUndo,
												new EventHandler(onUndo), new EventHandler(onQueryUndo));
				AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int) VSConstants.VSStd97CmdID.MultiLevelRedo,
												new EventHandler(onRedo), new EventHandler(onQueryRedo));
			}
		}

		/// <summary>
		/// Helper function used to add commands using IMenuCommandService
		/// </summary>
		/// <param name="mcs"> The IMenuCommandService interface.</param>
		/// <param name="menuGroup"> This guid represents the menu group of the command.</param>
		/// <param name="cmdID"> The command ID of the command.</param>
		/// <param name="commandEvent"> An EventHandler which will be called whenever the command is invoked.</param>
		/// <param name="queryEvent"> An EventHandler which will be called whenever we want to query the status of
		/// the command.  If null is passed in here then no EventHandler will be added.</param>
		private static void AddCommand(IMenuCommandService mcs, Guid menuGroup, int cmdID, EventHandler commandEvent, EventHandler queryEvent)
		{
			// Create the OleMenuCommand from the menu group, command ID, and command event
			CommandID menuCommandID = new CommandID(menuGroup, cmdID);
			OleMenuCommand command = new OleMenuCommand(commandEvent, menuCommandID);

			// Add an event handler to BeforeQueryStatus if one was passed in
			if (null != queryEvent)
			{
				command.BeforeQueryStatus += queryEvent;
			}

			// Add the command using our IMenuCommandService instance
			mcs.AddCommand(command);
		}

		/// <summary>
		/// Handler for out SelectAll command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onSelectAll(object sender, EventArgs e)
		{
			editor.TextBox.SelectAll();
		}

		/// <summary>
		/// Handler for when we want to query the status of the copy command.  If there
		/// is any text selected then it will set the Enabled property to true.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryCopy(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Enabled = ApplicationCommands.Copy.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Copy command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onCopy(object sender, EventArgs e)
		{
			ApplicationCommands.Copy.Execute(DataFormats.Text, editor.TextBox);

			// editor.RecordCommand("Copy");
		}

		/// <summary>
		/// Handler for when we want to query the status of the cut or delete
		/// commands.  If there is any selected text then it will set the 
		/// enabled property to true.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryCutOrDelete(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Enabled = ApplicationCommands.Cut.CanExecute(null, editor.TextBox)
										 || ApplicationCommands.Delete.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Cut command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onCut(object sender, EventArgs e)
		{
			ApplicationCommands.Cut.Execute(DataFormats.Text, editor.TextBox);

			// editor.RecordCommand("Cut");
		}

		/// <summary>
		/// Handler for our Delete command.
		/// </summary>
		private void onDelete(object sender, EventArgs e)
		{
			ApplicationCommands.Delete.Execute(DataFormats.Text, editor.TextBox);

			// editor.RecordCommand("Delete");
		}

		/// <summary>
		/// Handler for when we want to query the status of the paste command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryPaste(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Enabled = ApplicationCommands.Paste.CanExecute(DataFormats.Text, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Paste command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onPaste(object sender, EventArgs e)
		{
			ApplicationCommands.Paste.Execute(DataFormats.Text, editor.TextBox);

			// editor.RecordCommand("Paste");
		}

		/// <summary>
		/// Handler for when we want to query the status of the clipboard ring.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryPasteNextTBXCBItem(object sender, EventArgs e)
		{
			// Get the Toolbox Service from the package
			IVsToolboxClipboardCycler clipboardCycler = GetService(typeof(SVsToolbox)) as IVsToolboxClipboardCycler;

			int itemsAvailable;
			ErrorHandler.ThrowOnFailure(clipboardCycler.AreDataObjectsAvailable((IVsToolboxUser) this, out itemsAvailable));

			OleMenuCommand command = (OleMenuCommand) sender;
			command.Enabled = ((itemsAvailable > 0) ? true : false);
		}

		/// <summary>
		/// Handler for our Paste command.
		/// </summary>
		/// <param name="sender">  Not used.</param>
		/// <param name="e">  Not used.</param>
		private void onPasteNextTBXCBItem(object sender, EventArgs e)
		{
			// Get the Toolbox Service from the package
			IVsToolboxClipboardCycler clipboardCycler = GetService(typeof(SVsToolbox)) as IVsToolboxClipboardCycler;

			Microsoft.VisualStudio.OLE.Interop.IDataObject pDO;

			ErrorHandler.ThrowOnFailure(clipboardCycler.GetAndSelectNextDataObject((IVsToolboxUser) this, out pDO));

			var textSelection = editor.TextBox.Selection;

			// Get the current position of the start of the current selection. 
			// After the paste the position of the start of current selection
			// will be moved to the end of inserted text, so it needs to
			// move back to original position so that inserted text can be highlighted to 
			// allow cycling through our clipboard items.
			var originalStart = textSelection.Start;

			// This will do the actual pasting of the object
			((IVsToolboxUser) this).ItemPicked(pDO);

			editor.TextBox.Selection.Select(originalStart, editor.TextBox.CaretPosition);
		}

		/// <summary>
		/// Handler for when we want to query the status of the Undo command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryUndo(object sender, EventArgs e)
		{
			OleMenuCommand command = (OleMenuCommand) sender;
			command.Enabled = editor.TextBox.CanUndo;
		}

		/// <summary>
		/// Handler for our Undo command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onUndo(object sender, EventArgs e)
		{
			editor.TextBox.Undo();
		}

		/// <summary>
		/// Handler for when we want to query the status of the Redo command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryRedo(object sender, EventArgs e)
		{
			OleMenuCommand command = (OleMenuCommand) sender;
			command.Enabled = editor.TextBox.CanRedo;
		}

		/// <summary>
		/// Handler for our Redo command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onRedo(object sender, EventArgs e)
		{
			editor.TextBox.Redo();
		}

		/// <summary>
		/// Handler for when we want to query the status of the Bold command.  It will
		/// always be enabled, but we want to check if the current text is bold or not
		/// so we can set the Checked property which will change how the button looks
		/// in the toolbar and the context menu.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryBold(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.ToggleBold.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Bold command.  Toggles the bold state of the selected text.
		/// Or if there is no selected text then it toggles the bold state for 
		/// newly entered text.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onBold(object sender, EventArgs e)
		{
			EditingCommands.ToggleBold.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the Italic command.  It will
		/// always be enabled, but we want to check if the current text is Italic or not
		/// so we can set the Checked property which will change how the button looks
		/// in the toolbar and the context menu.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryItalic(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.ToggleItalic.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Italic command.  Toggles the italic state of the selected text.
		/// Or if there is no selected text then it toggles the italic state for 
		/// newly entered text.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onItalic(object sender, EventArgs e)
		{
			EditingCommands.ToggleItalic.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the Underline command.  It will
		/// always be enabled, but we want to check if the current text is underlined or not
		/// so we can set the Checked property which will change how the button looks
		/// in the toolbar and the context menu.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryUnderline(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.ToggleUnderline.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Underline command.  Toggles the underline state of the selected
		/// text.  Or if there is no selected text then it toggles the underline state for 
		/// newly entered text.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onUnderline(object sender, EventArgs e)
		{
			EditingCommands.ToggleUnderline.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the Strikethrough command.  It will
		/// always be enabled, but we want to check if the current text has Strikethrough or not
		/// so we can set the Checked property which will change how the button looks
		/// in the toolbar and the context menu.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryStrikethrough(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = false;
		}

		/// <summary>
		/// Handler for our Strikethrough command.  Toggles the strikethrough state of 
		/// the selected text.  Or if there is no selected text then it toggles the 
		/// strikethrough state for newly entered text.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onStrikethrough(object sender, EventArgs e)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Handler for when we want to query the status of the justify center command.  It will
		/// always be enabled, but we want to check if the current text is center-justified or not
		/// so we can set the Checked property which will change how the button looks in the toolbar.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryJustifyCenter(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.AlignCenter.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Justify Center command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onJustifyCenter(object sender, EventArgs e)
		{
			EditingCommands.AlignCenter.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the justify left command.  It will
		/// always be enabled, but we want to check if the current text is left-justified or not
		/// so we can set the Checked property which will change how the button looks in the toolbar.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryJustifyLeft(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.AlignLeft.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Justify Left command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onJustifyLeft(object sender, EventArgs e)
		{
			EditingCommands.AlignLeft.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the justify right command.  It will
		/// always be enabled, but we want to check if the current text is right-justified or not
		/// so we can set the Checked property which will change how the button looks in the toolbar.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryJustifyRight(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.AlignRight.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Justify Right command.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onJustifyRight(object sender, EventArgs e)
		{
			EditingCommands.AlignRight.Execute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for when we want to query the status of the justify right command.  It will
		/// always be enabled, but we want to check if this is active in the current text so
		/// we can change the look of the command in the toolbar and context menu.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onQueryBulletedList(object sender, EventArgs e)
		{
			var command = (OleMenuCommand) sender;

			command.Checked = EditingCommands.ToggleBullets.CanExecute(null, editor.TextBox);
		}

		/// <summary>
		/// Handler for our Bulleted List command.  This simply toggles the state
		/// of the SelectionBullet property.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  Not used.</param>
		private void onBulletedList(object sender, EventArgs e)
		{
			EditingCommands.ToggleBullets.Execute(null, editor.TextBox);
		}

		/*
		/// <summary>
		/// Helper function that fills the fontList array (of strings) with
		/// all the available fonts.
		/// </summary>
		private void fillFontList()
		{
			FontFamily[] fontFamilies;

			System.Drawing.Text.InstalledFontCollection installedFontCollection = new System.Drawing.Text.InstalledFontCollection();

			// Get the array of FontFamily objects.
			fontFamilies = installedFontCollection.Families;

			// Create the font list array and fill it with the list of available fonts.
			fontListArray = new string[fontFamilies.Length];
			for (int i = 0; i < fontFamilies.Length; ++i)
			{
				fontListArray[i] = fontFamilies[i].Name;
			}
		}
		
		/// <summary>
		/// This function is called when the drop down that lists the possible
		/// fonts is clicked.  It is responsible for populating the list of fonts
		/// with strings.  The fillFontList function is responsible for getting the
		/// list of possible fonts and will be called from here the first time
		/// this function is called.  Note that we use the EventArgs parameter to
		/// pass back the list after casting it to an OleMenuCmdEventArgs object.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  We will cast this to an OleMenuCommandEventArgs
		/// object and then use it to pass back the array of strings.</param>
		private void onFontNameGetList(object sender, EventArgs e)
		{
			// If this is the first time we are calling this function then
			// we need to set up the fontListArray
			if (this.fontListArray == null)
			{
				fillFontList();
			}

			// Cast the EventArgs to an OleMenuCmdEventArgs object
			OleMenuCmdEventArgs args = (OleMenuCmdEventArgs) e;

			// Set the out value of the OleMenuCmdEventArgs to our font list array
			Marshal.GetNativeVariantForObject(fontListArray, args.OutValue);
		}
		
		/// <summary>
		/// This function will be called for two separate reasons.  It will be called constantly
		/// to figure out what string needs to be displayed in the font name combo box.  In this
		/// case we need to cast the EventArgs to OleMenuCmdEventArgs and set the OutValue to
		/// the name of the currently used font.  It will also be called when the user selects a new
		/// font.  In this case we need to cast EventArgs to OleMenuCmdEventArgs so that we can get the
		/// name of the new font from InValue and set it for our hosted text editor.
		/// </summary>
		/// <param name="sender"> This can be cast to an OleMenuCommand.</param>
		/// <param name="e"> We will cast this to an OleMenuCommandEventArgs and use it in
		/// two ways.  If we are setting a new font we will get its name by casting the
		/// InValue to a string.  Otherwise we will just set the OutValue to the name
		/// of the current font.</param>
		private void onFontName(object sender, EventArgs e)
		{
			OleMenuCmdEventArgs args = (OleMenuCmdEventArgs) e;

			// If args.InValue is null then we just need to set the OutValue
			// to the current font.  If it is not null then that means that we
			// need to cast it to a string and set it as the font.
			if (null == args.InValue)
			{
				string currentFont = editor.TextBox.SelectionFont.FontFamily.Name;
				Marshal.GetNativeVariantForObject(currentFont, args.OutValue);
			}
			else
			{
				string fontName = (string) args.InValue;
				Font f = new Font(fontName, editor.TextBox.SelectionFont.Size, editor.TextBox.SelectionFont.Style);
				editor.TextBox.SelectionFont = f;

				if (f != null)
					f.Dispose();
			}
		}
		
		/// <summary>
		/// This function is called when the drop down that lists the possible
		/// font sizes is clicked.  It is responsible for populating the list
		/// with strings.  The static string array fontSizeArray is filled with the most
		/// commonly used font sizes, although the user can enter any number they want. 
		/// Note that we use the EventArgs parameter to pass back the list after
		/// casting it to an OleMenuCmdEventArgs object.
		/// </summary>
		/// <param name="sender">  This can be cast to an OleMenuCommand.</param>
		/// <param name="e">  We will cast this to an OleMenuCommandEventArgs
		/// object and then use it to pass back the array of strings.</param>
		private void onFontSizeGetList(object sender, EventArgs e)
		{
			// Cast the EventArgs to an OleMenuCmdEventArgs object
			OleMenuCmdEventArgs args = (OleMenuCmdEventArgs) e;

			// Set the out value of the OleMenuCmdEventArgs to our font size array
			Marshal.GetNativeVariantForObject(fontSizeArray, args.OutValue);
		}
		
		/// <summary>
		/// This function will be called for two separate reasons.  It will be called constantly
		/// to figure out what string needs to be displayed in the font size combo box.  In this
		/// case we need to cast the EventArgs to OleMenuCmdEventArgs and set the OutValue to
		/// the current font size.  It will also be called when the user changes the font size.
		/// In this case we need to cast EventArgs to OleMenuCmdEventArgs so that we can get the
		/// new font size and set it for our hosted text editor.
		/// </summary>
		/// <param name="sender"> This can be cast to an OleMenuCommand.</param>
		/// <param name="e"> We will cast this to an OleMenuCommandEventArgs and use it in
		/// two ways.  If we are setting a new font size we will get its name by casting the
		/// InValue to a string.  Otherwise we will just set the OutValue to the current 
		/// font size.</param>
		private void onFontSize(object sender, EventArgs e)
		{
			OleMenuCmdEventArgs args = (OleMenuCmdEventArgs) e;

			// If args.InValue is null then we just need to set the OutValue
			// to the current font size.  If it is not null then that means that we
			// need to cast it to a string and set it as the new font size.
			if (null == args.InValue)
			{
				string currentSize = Convert.ToString(Convert.ToInt32(editor.TextBox.SelectionFont.Size), CultureInfo.InvariantCulture);
				Marshal.GetNativeVariantForObject(currentSize, args.OutValue);
			}
			else
			{
				string fontSize = (string) args.InValue;
				Font f = new Font(editor.TextBox.SelectionFont.FontFamily, Convert.ToSingle(fontSize, CultureInfo.InvariantCulture), editor.TextBox.SelectionFont.Style);
				editor.TextBox.SelectionFont = f;

				if (f != null)
					f.Dispose();
			}
		}
		*/
	}
}