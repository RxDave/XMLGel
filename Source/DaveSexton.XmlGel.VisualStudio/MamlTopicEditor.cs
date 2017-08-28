using System;
//using System.Windows.Input;
//using tom;

namespace DaveSexton.XmlGel.VisualStudio
{
	internal partial class MamlTopicEditor : DaveSexton.XmlGel.Maml.MamlTopicEditor
	{
		/// <summary>
		/// This value is used internally so that we know what to display on the status bar.
		/// NOTE: Setting this value will not actually change the insert/overwrite behavior
		/// of the editor, it is just used so that we can keep track of the state internally.
		/// </summary>
		public bool Overstrike
		{
			get;
			set;
		}

		protected override bool ShouldIncrementRevisionNumber
		{
			get
			{
				return pane.DocumentHasUnsavedChanges;
			}
		}

		/*
		/// <summary>
		/// This property exposes the ITextDocument interface associated with
		/// our Rich Text editor.
		/// </summary>
		internal ITextDocument TextDocument
		{
			[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				if (null != textDocument)
					return textDocument;

				// To get the IRichEditOle interface we need to call SendMessage, which
				// we imported from user32.dll
				object editOle = null;
				NativeMethods.SendMessage(textBox.Handle,                // The rich text box handle
																	getOleInterfaceCommandId,   // The command ID for EM_GETOLEINTERFACE
																	IntPtr.Zero,                // null
																	out editOle                 // This will be set to the IRichEditOle interface
																	);

				// Call GetIUnknownForObject with the IRichEditOle interface that we
				// just got so that we have an IntPtr to pass into QueryInterface
				IntPtr editOlePtr = IntPtr.Zero;
				editOlePtr = Marshal.GetIUnknownForObject(editOle);

				// Call QueryInterface to get the pointer to the ITextDocument
				IntPtr iTextDocument = IntPtr.Zero;
				Guid iTextDocumentGuid = typeof(ITextDocument).GUID;
				Marshal.QueryInterface(editOlePtr, ref iTextDocumentGuid, out iTextDocument);

				// We need to call Marshal.Release with the pointer that we got
				// from the GetIUnknownForObject call
				Marshal.Release(editOlePtr);

				// Call GetObjectForIUnknown passing in the pointer that was set
				// by QueryInterface and return it as an ITextDocument
				textDocument = Marshal.GetObjectForIUnknown(iTextDocument) as ITextDocument;
				return textDocument;
			}
		}

		/// <summary>
		/// This property will return the current ITextRange interface.
		/// </summary>
		internal ITextRange TextRange
		{
			get
			{
				return TextDocument.Range(0, (int) tom.tomConstants.tomForward);
			}
		}

		/// <summary>
		/// This property will return the current ITextSelection interface.
		/// </summary>
		internal ITextSelection TextSelection
		{
			get
			{
				return TextDocument.Selection;
			}
		}
		*/

		private const int getOleInterfaceCommandId = 1084;

		//private VSMacroRecorder recorder;
		//private ITextDocument textDocument;
		private readonly MamlTopicEditorPane pane;

		public MamlTopicEditor(MamlTopicEditorPane pane)
		{
			this.pane = pane;

			/*
			recorder = new VSMacroRecorder(GuidList.guidDaveSexton_XmlGel_VisualStudioEditorFactory);

			textBox.MouseEnter += textBox_MouseEnter;
			textBox.PreviewKeyDown += textBox_PreviewKeyDown;
			*/
		}

		protected override void OnDocumentContentChanged(EventArgs e)
		{
			// TODO: Create xmlTextBox to show the raw XML in a separate tab (like a design/code view) then uncomment the following code

			/*
				document.Update();

				xmlTextBox.Text = document.Document.ToString();
			*/
		}

		/*
		/// <summary>
		/// Returns the column number from the specified index
		/// </summary>
		/// <param name="index">index of the character</param>
		/// <returns>column number</returns>
		public int GetColumnFromIndex(int index)
		{
			//first get the index of the first char of the current line
			int currentLineIndex = textBox.GetFirstCharIndexOfCurrentLine();
			return index - currentLineIndex;
		}

		/// <summary>
		/// Returns the index from the specified line and column number
		/// </summary>
		/// <param name="line">line number</param>
		/// <param name="column">column number</param>
		/// <returns>index</returns>
		public int GetIndexFromLineAndColumn(int line, int column)
		{
			if (line < 0)
				return -1;
			//first get the index of the first char of the specified line
			int firstCharLineIndex = textBox.GetFirstCharIndexFromLine(line);
			if (firstCharLineIndex < 0)
				return -1;

			return firstCharLineIndex + column;
		}

		private void textBox_MouseEnter(object sender, MouseEventArgs e)
		{
			textBox.IgnoreMouseClicks = recorder.IsRecording();
		}

		private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (recorder.IsRecording())
			{
				RecordPrintableChar(e.Key);
			}
		}
		*/
	}
}