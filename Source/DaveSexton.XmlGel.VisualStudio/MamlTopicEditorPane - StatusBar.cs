using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditorPane : IVsStatusbarUser
	{
		/// <summary>
		/// This is the IVsStatusBarUser function that will update our status bar.
		/// Note that the IDE calls this function only when our document window is
		/// initially activated.
		/// </summary>
		/// <returns> HResult that represents success or failure.</returns>
		int IVsStatusbarUser.SetInfo()
		{
			// Call the helper function that updates the status bar insert mode
			int hrSetInsertMode = SetStatusBarInsertMode();

			// Call the helper function that updates the status bar selection mode
			int hrSetSelectionMode = SetStatusBarSelectionMode();

			// Call the helper function that updates the status bar position
			int hrSetPosition = SetStatusBarPosition();

			return (hrSetInsertMode == VSConstants.S_OK &&
							hrSetSelectionMode == VSConstants.S_OK &&
							hrSetPosition == VSConstants.S_OK) ? VSConstants.S_OK : VSConstants.E_FAIL;
		}

		/// <summary>
		/// Helper function that updates the insert mode displayed on the status bar.
		/// This is the text that is displayed in the right side of the status bar that
		/// will either say INS or OVR.
		/// </summary>
		/// <returns> HResult that represents success or failure.</returns>
		private int SetStatusBarInsertMode()
		{
			// Get the IVsStatusBar interface
			IVsStatusbar statusBar = GetService(typeof(SVsStatusbar)) as IVsStatusbar;
			if (statusBar == null)
				return VSConstants.E_FAIL;

			// Set the insert mode based on our editor.textBox.Overstrike value.  If 1 is passed
			// in then it will display OVR and if 0 is passed in it will display INS.
			object insertMode = (object) (this.editor.Overstrike ? 1 : 0);
			return statusBar.SetInsMode(ref insertMode);
		}

		/// <summary>
		/// Helper function that updates the selection mode displayed on the status
		/// bar.  Right now we only support stream selection.
		/// </summary>
		/// <returns> HResult that represents success or failure.</returns>
		private int SetStatusBarSelectionMode()
		{
			// Get the IVsStatusBar interface.
			IVsStatusbar statusBar = GetService(typeof(SVsStatusbar)) as IVsStatusbar;
			if (statusBar == null)
				return VSConstants.E_FAIL;

			// Set the selection mode.  Since we only support stream selection we will
			// always pass in zero here.  Passing in one would make "COL" show up
			// just to the left of the insert mode on the status bar.
			object selectionMode = 0;
			return statusBar.SetSelMode(ref selectionMode);
		}

		/// <summary>
		/// Helper function that updates the cursor position displayed on the status bar.
		/// </summary>
		/// <returns> HResult that represents success or failure.</returns>
		private int SetStatusBarPosition()
		{
			// Get the IVsStatusBar interface.
			IVsStatusbar statusBar = GetService(typeof(SVsStatusbar)) as IVsStatusbar;
			if (statusBar == null)
				return VSConstants.E_FAIL;
			/*
			// If there is no selection then textBox1.SelectionStart will tell us
			// the position of the cursor.  If there is a selection then this value will tell
			// us the position of the "left" side of the selection (the side of the selection that
			// has the smaller index value).
			int startIndex = editor.TextBox.SelectionStart;

			// If the cursor is at the end of the selection then we need to add the selection
			// length to the index value.
			if ((editor.TextSelection.Flags & (int) tom.tomConstants.tomSelStartActive) == 0)
				startIndex += editor.TextBox.SelectionLength;

			// Call the function that gets the (zero-based) line index based on the buffer index.
			int lineNumber = editor.TextBox.GetLineFromCharIndex(startIndex);

			// To get the (zero-based) character number subtract the index of the first character
			// on this line from the buffer index.
			int charNumber = startIndex - editor.TextBox.GetFirstCharIndexFromLine(lineNumber);
			
			// Call the SetLineChar function, making sure to add one to our line and
			// character values since the values we get from the RichTextBox calls
			// are zero based.
			object line = (object) (lineNumber + 1);
			object chr = (object) (charNumber + 1);
			*/

			// Call the IVsStatusBar's SetLineChar function and return it's HResult
			return 0; // statusBar.SetLineChar(ref line, ref chr);
		}
	}
}