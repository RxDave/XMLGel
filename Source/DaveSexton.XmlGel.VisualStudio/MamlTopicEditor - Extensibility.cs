using Microsoft.VisualStudio;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditor : IEditor
	{
		/*
		/// <summary>
		/// This property gets/sets the default tab width.
		/// </summary>
		float IEditor.DefaultTabStop
		{
			get { return editor.TextDocument.DefaultTabStop; }
			set
			{
				editor.TextDocument.DefaultTabStop = value;
			}
		}

		/// <summary>
		/// This property gets our editor's current ITextRange interface.  ITextRange is part
		/// of the rich edit control's text object model.
		/// </summary>
		ITextRange IEditor.Range
		{
			get
			{
				return editor.TextRange;
			}
		}

		/// <summary>
		/// This property gets our editor's current ITextSelection interface.  ITextSelection
		/// is part of the rich edit control's text object model.
		/// </summary>
		ITextSelection IEditor.Selection
		{
			get
			{
				return editor.TextSelection;
			}
		}

		/// <summary>
		/// This property gets/sets the selection properties that contain certain information
		/// about our editor's current selection.
		/// </summary>
		int IEditor.SelectionProperties
		{
			get { return editor.TextSelection.Flags; }
			set { editor.TextSelection.Flags = value; }
		}

		/// <summary>
		/// This function finds a string and returns the length of the matched string.
		/// Note that this function does not move the cursor to the string that it finds.
		/// </summary>
		/// <param name="textToFind"> The string that we want to look for.</param>
		/// <returns> The length of the matched string.</returns>
		int IEditor.FindText(string textToFind)
		{
			return TextRange.FindText(textToFind, (int) tom.tomConstants.tomForward, 0);
		}

		/// <summary>
		/// This function has the same effect as typing the passed in string into the editor.
		/// Our implementation will just call TypeText since for now we want them both to do
		/// the same thing.
		/// </summary>
		/// <param name="textToSet"> The string to set/</param>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.SetText(string textToSet)
		{
			// Just delegate to TypeText
			return TypeText(textToSet);
		}

		/// <summary>
		/// This function has the same effect as typing the passed in string into the editor.
		/// </summary>
		/// <param name="textToType"> The string to type.</param>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.TypeText(string textToType)
		{
			TextSelection.TypeText(textToType);
			return VSConstants.S_OK;
		}
		*/

		/// <summary>
		/// This function performs the cut operation in the editor.
		/// </summary>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.Cut()
		{
			TextBox.Cut();

			return VSConstants.S_OK;
		}

		/// <summary>
		/// This function performs the copy operation in the editor.
		/// </summary>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.Copy()
		{
			TextBox.Copy();

			return VSConstants.S_OK;
		}

		/// <summary>
		/// This function performs the paste operation in the editor.
		/// </summary>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.Paste()
		{
			TextBox.Paste();

			return VSConstants.S_OK;
		}

		/// <summary>
		/// This function performs a delete in the editor.
		/// </summary>
		/// <param name="unit"> The type of units that we are going to delete.  The two valid options
		/// for this are TOMWord and TOMCharacter, which are defined in the TOMConstants enumeration.</param>
		/// <param name="count"> The number of units that we are going to delete.  Passing in a negative number
		/// will be similar to pressing backspace and passing in a positive number will be similar to
		/// pressing delete.</param>
		/// <returns> HResult that indicates success/failure.</returns>
		int IEditor.Delete()
		{
			TextBox.Selection.Text = string.Empty;

			return VSConstants.S_OK;
		}

		/*
		/// <summary>
		/// This function will move up by the specified number of lines/paragraphs in the editor.
		/// </summary>
		/// <param name="unit"> The type of unit to move up by.  The two valid options for this are
		/// TOMLine and TOMParagraph, which are defined in the TOMConstants enumeration.</param>
		/// <param name="count"> The number of units to move.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of units that the cursor moved up.</returns>
		int IEditor.MoveUp(int unit, int count, int extend)
		{
			return TextSelection.MoveUp(unit, count, extend);
		}

		/// <summary>
		/// This function will move down by the specified number of lines/paragraphs in the editor.
		/// </summary>
		/// <param name="unit"> The type of unit to move down by.  The two valid options for this are
		/// TOMLine and TOMParagraph, which are defined in the TOMConstants enumeration.</param>
		/// <param name="count"> The number of units to move.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of units that the cursor moved down.</returns>
		int IEditor.MoveDown(int unit, int count, int extend)
		{
			return TextSelection.MoveDown(unit, count, extend);
		}

		/// <summary>
		/// This function will move to the left by the specified number of characters/words in the editor.
		/// </summary>
		/// <param name="unit"> The type of unit to move left by.  The two valid options for this are
		/// TOMWord and TOMCharacter, which are defined in the TOMConstants enumeration.</param>
		/// <param name="count"> The number of units to move.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of units that the cursor moved to the left.</returns>
		int IEditor.MoveLeft(int unit, int count, int extend)
		{
			return TextSelection.MoveLeft(unit, count, extend);
		}

		/// <summary>
		/// This function will move to the right by the specified number of characters/words in the editor.
		/// </summary>
		/// <param name="unit"> The type of unit to move right by.  The two valid options for this are
		/// TOMWord and TOMCharacter, which are defined in the TOMConstants enumeration.</param>
		/// <param name="count"> The number of units to move.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of units that the cursor moved to the right.</returns>
		int IEditor.MoveRight(int unit, int count, int extend)
		{
			return TextSelection.MoveRight(unit, count, extend);
		}

		/// <summary>
		/// This function will either move the cursor to either the end of the current line or the end of the document.
		/// </summary>
		/// <param name="unit"> If this value is equal to TOMLine it will move the cursor to the end of the line.  If
		/// it is set to TOMStory then it will move to the end of the document.  These values are defined in the
		/// TOMConstants enumeration.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of characters that the operation moved the cursor by.  This value
		/// should always be positive since we are moving "forward" in the text buffer.</returns>
		int IEditor.EndKey(int unit, int extend)
		{
			return TextSelection.EndKey(unit, extend);
		}

		/// <summary>
		/// This function will either move the cursor to either the beginning of the current line or
		/// the beginning of the document.
		/// </summary>
		/// <param name="unit"> If this value is equal to TOMLine it will move the cursor to the beginning of the line.
		/// If it is set to TOMStory then it will move to the beginning of the document.  These values are defined in the
		/// TOMConstants enumeration.</param>
		/// <param name="extend"> This should be set to TOMExtend if we want to select as we move
		/// or TOMMove if we don't.  These values are defined in the TOMConstants enumeration.</param>
		/// <returns> The number of characters that the operation moved the cursor by.  This value
		/// should always be negative since we are moving "backward" in the text buffer.</returns>
		int IEditor.HomeKey(int unit, int extend)
		{
			return TextSelection.HomeKey(unit, extend);
		}
		*/
	}
}